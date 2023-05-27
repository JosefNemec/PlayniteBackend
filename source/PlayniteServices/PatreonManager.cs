using Playnite;
using System.IO;
using System.Net.Http;

namespace PlayniteServices.Patreon;

public class PatreonManager : IDisposable
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private static bool instantiated = false;
    private readonly UpdatableAppSettings settings;
    private readonly HttpClient httpClient;

    public PatreonManager(UpdatableAppSettings settings)
    {
        if (settings.Settings.Patreon == null)
        {
            throw new Exception("Patreon settings are missing.");
        }

        TestAssert.IsFalse(instantiated, $"{nameof(PatreonManager)} already instantiated");
        instantiated = true;
        this.settings = settings;
        httpClient = new HttpClient();
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }

    private static async Task SaveTokens(string accessToken, string refreshToken)
    {
        await Task.Delay(2000);
        var path = Path.Combine(ServicePaths.ExecutingDirectory, "patreonTokens.json");
        var config = new Dictionary<string, Dictionary<string, string>>
        {
            { "Patreon", new Dictionary<string, string>
                {
                    { "AccessToken", accessToken },
                    { "RefreshToken", refreshToken }
                }
            }
        };

        try
        {
            File.WriteAllText(path, Serialization.ToJson(config));
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to save new Patreon API token.");
        }
    }

    private HttpRequestMessage CreateGetRequest(string url)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = url.StartsWith("https", StringComparison.OrdinalIgnoreCase) ? new Uri(url) : new Uri(settings.Settings.Patreon!.ApiEndpoint + url),
            Method = HttpMethod.Get
        };

        request.Headers.Add("Authorization", $"Bearer {settings.Settings.Patreon!.AccessToken}");
        return request;
    }

    private async Task UpdateTokens()
    {
        var refreshToken = settings.Settings.Patreon!.RefreshToken;
        var clientId = settings.Settings.Patreon.Id;
        var clientSecret = settings.Settings.Patreon.Secret;
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(
                settings.Settings.Patreon.ApiEndpoint +
                $"token?grant_type=refresh_token&refresh_token={refreshToken}&client_id={clientId}&client_secret={clientSecret}"),
            Method = HttpMethod.Post
        };

        var response = await httpClient.SendAsync(request);
        var stringData = await response.Content.ReadAsStringAsync();
        var data = Serialization.FromJson<TokenRefreshResponse>(stringData);
        if (data == null)
        {
            logger.Debug(stringData);
            throw new Exception("Failed to update Patreon tokens.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            settings.Settings.Patreon.AccessToken = data.access_token;
            settings.Settings.Patreon.RefreshToken = data.refresh_token;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SaveTokens(data.access_token!, data.refresh_token!);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

    public async Task<string> SendStringRequest(string url)
    {
        var request = CreateGetRequest(url);
        var response = await httpClient.SendAsync(request);

        // Access token probably expired (once every month)
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            try
            {
                await UpdateTokens();
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to update Patreon tokens.");
                throw;
            }

            request = CreateGetRequest(url);
            response = await httpClient.SendAsync(request);
        }

        return await response.Content.ReadAsStringAsync();
    }
}
