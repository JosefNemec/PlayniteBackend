using ComposableAsync;
using MongoDB.Driver;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Controllers.IGDB;
using RateLimiter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB;

public partial class IgdbApi : IDisposable
{
    public class AuthResponse
    {
        public string? access_token { get; set; }
    }

    private static bool instantiated = false;
    private static readonly ILogger logger = LogManager.GetLogger();
    public readonly UpdatableAppSettings Settings;
    public readonly Database Database;
    private readonly System.Threading.Timer? webhookTimer;

    public HttpClient HttpClient { get; }
    public List<IIgdbCollection> DataCollections { get; } = new List<IIgdbCollection>();

    public IgdbApi(UpdatableAppSettings settings, Database db)
    {
        if (settings.Settings.IGDB == null)
        {
            throw new Exception("IGDB settings missing");
        }

        TestAssert.IsFalse(instantiated, $"{nameof(IgdbApi)} already instantiated");
        instantiated = true;

        Settings = settings;
        Database = db;
        var requestLimiterHandler = TimeLimiter
            .GetFromMaxCountByInterval(4, TimeSpan.FromSeconds(1))
            .AsDelegatingHandler();
        HttpClient = new HttpClient(requestLimiterHandler);
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        HttpClient.Timeout = new TimeSpan(0, 0, 50);
        InitCollections();

        if (settings.Settings.IGDB.RegisterWebhooks && !settings.Settings.IGDB.WebHookSecret.IsNullOrEmpty())
        {
            webhookTimer = new System.Threading.Timer(
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

    public async Task CloneCollections()
    {
        await Parallel.ForEachAsync(
            DataCollections,
            new ParallelOptions { MaxDegreeOfParallelism = 3 },
            async (collection, token) =>
        {
            await collection.CloneCollection();
        });
    }

    private async void RegiserWebhooksCallback(object? _)
    {
        List<Webhook>? webhooks = null;

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
                await collection.ConfigureWebhooks(webhooks ?? new List<Webhook>());
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to register {collection.EndpointPath} webhooks.");
            }
        }
    }

    public async Task<List<Webhook>?> GetWebhooks()
    {
        var webhooksString = await SendStringRequest("webhooks", null, HttpMethod.Get, true);
        return DataSerialization.FromJson<List<Webhook>>(webhooksString);
    }

    private static async Task SaveTokens(string accessToken)
    {
        await Task.Delay(2000);
        var path = Path.Combine(ServicePaths.ExecutingDirectory, "twitchTokens.json");
        var config = new Dictionary<string, Dictionary<string, string>>
        {
            { "IGDB", new Dictionary<string, string>()
                {
                    { "AccessToken", accessToken }
                }
            }
        };

        try
        {
            File.WriteAllText(path, DataSerialization.ToJson(config));
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to save new twitch API token.");
        }
    }

    private async Task Authenticate()
    {
        var clientId = Settings.Settings.IGDB!.ClientId;
        var clientSecret = Settings.Settings.IGDB.ClientSecret;
        var authUrl = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials";
        var response = await HttpClient.PostAsync(authUrl, null);
        var auth = DataSerialization.FromJson<AuthResponse>(await response.Content.ReadAsStringAsync());
        if (auth?.access_token == null)
        {
            throw new Exception("Failed to authenticate IGDB.");
        }
        else
        {
            logger.Info($"New IGDB auth token generated: {auth.access_token}");
            Settings.Settings.IGDB.AccessToken = auth.access_token;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SaveTokens(auth.access_token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

    private HttpRequestMessage CreateRequest(string url, HttpContent? content, HttpMethod method)
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(Settings.Settings.IGDB!.ApiEndpoint + url),
            Method = method,
            Content = content
        };

        request.Headers.Add("Authorization", $"Bearer {Settings.Settings.IGDB.AccessToken}");
        request.Headers.Add("Client-ID", Settings.Settings.IGDB.ClientId);
        return request;
    }

    public async Task<string> SendStringRequest(string url, string query, bool reTry = true, bool log = true)
    {
        if (log) { logger.Debug($"IGDB live request: {url}, {query}"); }
        return await SendStringRequest(url, new StringContent(query), HttpMethod.Post, reTry);
    }

    public async Task<string> SendStringRequest(string url, HttpContent? content, HttpMethod method, bool reTry)
    {
        var sharedRequest = CreateRequest(url, content, method);
        var response = await HttpClient.SendAsync(sharedRequest);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return await response.Content.ReadAsStringAsync();
        }

        var authFailed = response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden;
        if (authFailed && reTry)
        {
            logger.Error($"IGDB request failed on authentication {response.StatusCode}.");
            await Authenticate();
            return await SendStringRequest(url, content, method, false);
        }
        else if (authFailed)
        {
            throw new Exception($"Failed to authenticate IGDB {response.StatusCode}.");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && reTry)
        {
            await Task.Delay(250);
            return await SendStringRequest(url, content, method, false);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            throw new Exception($"IGDB failed due to too many requests.");
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            logger.Error(errorMessage);
            // Request sometimes fails on generic error, but then works when sent again...
            if (errorMessage.Contains("Internal server error", StringComparison.OrdinalIgnoreCase) && reTry)
            {
                return await SendStringRequest(url, content, method, false);
            }
            else
            {
                throw new Exception($"Uknown IGDB API response {response.StatusCode}.");
            }
        }
    }
}
