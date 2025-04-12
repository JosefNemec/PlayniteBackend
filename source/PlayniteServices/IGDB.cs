using ComposableAsync;
using Playnite;
using RateLimiter;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Playnite.Backend.IGDB;

public partial class IgdbManager : IDisposable
{
    public class AuthResponse
    {
        public string? access_token { get; set; }
    }

    private static bool instantiated = false;
    private static readonly ILogger logger = LogManager.GetLogger();
    public readonly UpdatableAppSettings Settings;
    public readonly Database Database;
    private readonly Timer? webhookTimer;
    private readonly SemaphoreSlim authSemaphore = new SemaphoreSlim(1, 1);
    private bool isAuthenticated = false;

    public HttpClient HttpClient { get; }
    public List<IIgdbCollection> DataCollections { get; } = [];

    public IgdbManager(UpdatableAppSettings settings, Database db)
    {
        if (settings.Settings.IGDB == null)
        {
            throw new Exception("IGDB settings missing");
        }

        TestAssert.IsFalse(instantiated, $"{nameof(IgdbManager)} already instantiated");
        instantiated = true;

        Settings = settings;

        if (Settings.Settings.IGDB?.ApiEndpoint.IsNullOrWhiteSpace() == true)
        {
            throw new Exception("IGDB API endpoint not configured.");
        }

        Database = db;
        var requestLimiterHandler = TimeLimiter.
            GetFromMaxCountByInterval(4, TimeSpan.FromSeconds(1)).
            AsDelegatingHandler();
        HttpClient = new HttpClient(requestLimiterHandler);
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        HttpClient.Timeout = new TimeSpan(0, 0, 50);
        InitCollections();

        if (settings.Settings.IGDB.RegisterWebhooks && !settings.Settings.IGDB.WebHookSecret.IsNullOrEmpty())
        {
            webhookTimer = new Timer(
                RegiserWebhooksCallback,
                null,
                new TimeSpan(0),
                new TimeSpan(1, 0, 0));
        }
    }

    public void Dispose()
    {
        webhookTimer?.Dispose();
    }

    private async void RegiserWebhooksCallback(object? _)
    {
        var webhooks = new List<Webhook>();

        try
        {
            webhooks = await GetWebhooks();
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to get current status of IGDB webhooks.");
        }

        foreach (var collection in DataCollections)
        {
            try
            {
                await collection.ConfigureWebhooks(webhooks);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to register {collection.EndpointPath} webhooks.");
            }
        }
    }

    public async Task<List<Webhook>> GetWebhooks()
    {
        var webhooksString = await SendStringRequest("webhooks", null, HttpMethod.Get, true);
        return Serialization.FromJson<List<Webhook>>(webhooksString) ?? [];
    }

    public async Task DeleteWebhook(int webhookId)
    {
        await SendStringRequest($"webhooks/{webhookId}", null, HttpMethod.Delete, false);
    }

    public async Task CloneAllCollections()
    {
        await Parallel.ForEachAsync(
            DataCollections,
            new ParallelOptions { MaxDegreeOfParallelism = 4 },
            async (collection, _) =>
            {
                await collection.CloneCollection();
            });
    }

    private static async Task SaveTokens(string accessToken)
    {
        var path = Path.Combine(PlaynitePaths.RuntimeDataDir, PlaynitePaths.TwitchConfigFileName);
        var config = new Dictionary<string, Dictionary<string, string>>
        {
            ["IGDB"] = new()
            {
                ["AccessToken"] = accessToken
            }
        };

        try
        {
            await File.WriteAllTextAsync(path, Serialization.ToJson(config));
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to save new twitch API token.");
        }
    }

    private async Task Authenticate()
    {
        await authSemaphore.WaitAsync();
        if (isAuthenticated)
        {
            authSemaphore.Release();
            return;
        }

        try
        {
            logger.Info("Getting new IGDB auth token.");
            var clientId = Settings.Settings.IGDB!.ClientId;
            var clientSecret = Settings.Settings.IGDB.ClientSecret;
            var authUrl = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials";
            var response = await HttpClient.PostAsync(authUrl, null);
            var auth = Serialization.FromJson<AuthResponse>(await response.Content.ReadAsStringAsync());
            if (auth?.access_token == null)
            {
                throw new Exception("Failed to authenticate IGDB.");
            }

            logger.Info("New IGDB auth token generated.");
            Settings.Settings.IGDB.AccessToken = auth.access_token;
            await SaveTokens(auth.access_token);
            isAuthenticated = true;
        }
        catch
        {
            isAuthenticated = false;
            throw;
        }
        finally
        {
            authSemaphore.Release();
        }
    }

    private HttpRequestMessage CreateRequest(string url, HttpContent? content, HttpMethod method)
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(Settings.Settings.IGDB!.ApiEndpoint!.UriCombine(url)),
            Method = method,
            Content = content
        };

        request.Headers.Add("Authorization", $"Bearer {Settings.Settings.IGDB.AccessToken}");
        request.Headers.Add("Client-ID", Settings.Settings.IGDB.ClientId);
        return request;
    }

    public async Task<string> SendStringRequest(string url, string query, bool allowRetry = true, bool log = true)
    {
        if (log) { logger.Debug($"IGDB live request: {url}, {query}"); }
        return await SendStringRequest(url, new StringContent(query), HttpMethod.Post, allowRetry);
    }

    public async Task<string> SendStringRequest(string url, HttpContent? content, HttpMethod method, bool allowRetry)
    {
        var sharedRequest = CreateRequest(url, content, method);
        var response = await HttpClient.SendAsync(sharedRequest);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return await response.Content.ReadAsStringAsync();
        }

        var tooManyRequest = response.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
        var authFailed = response.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden;
        if (authFailed)
            isAuthenticated = false;

        if (!allowRetry)
        {
            if (authFailed)
            {
                throw new Exception($"Failed to authenticate IGDB {response.StatusCode}.");
            }

            if (tooManyRequest)
            {
                throw new Exception("IGDB failed due to too many requests.");
            }

            throw new Exception($"Uknown IGDB API response {response.StatusCode}.");
        }

        if (authFailed)
        {
            logger.Error($"IGDB request failed on authentication {response.StatusCode}.");
            await Authenticate();
            return await SendStringRequest(url, content, method, false);
        }

        if (tooManyRequest)
        {
            await Task.Delay(500);
            return await SendStringRequest(url, content, method, false);
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        logger.Error(errorMessage);
        // Request sometimes fails on generic error, but then works when sent again...
        if (errorMessage.Contains("Internal server error", StringComparison.OrdinalIgnoreCase) && allowRetry)
        {
            await Task.Delay(2_000);
            return await SendStringRequest(url, content, method, false);
        }

        throw new Exception($"Uknown IGDB API response {response.StatusCode}.");

    }
}
