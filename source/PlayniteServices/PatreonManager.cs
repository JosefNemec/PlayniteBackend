using Playnite;
using System.IO;
using System.Net.Http;
using JsonApiSerializer;
using JsonApiSerializer.JsonApi;

namespace Playnite.Backend.Patreon;

public class PatreonManager : IDisposable
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private static bool instantiated = false;
    private readonly UpdatableAppSettings settings;
    private readonly HttpClient httpClient;
    private readonly System.Threading.Timer? patronsFetchTimer;
    public List<string> PatronsList { get; } = new();

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

        if (settings.Settings.Patreon.PatronsFetchEnabled)
        {
            patronsFetchTimer = new System.Threading.Timer(
                PatronsFetchCallback,
                null,
                new TimeSpan(0),
                new TimeSpan(0, 20, 0));
        }
    }

    public void Dispose()
    {
        httpClient.Dispose();
        patronsFetchTimer?.Dispose();
    }

    private async void PatronsFetchCallback(object? _)
    {
        try
        {
            await UpdatePatronsList();
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to update patrons list.");
        }
    }

    private async Task UpdatePatronsList()
    {
        PatronsList.Clear();
        var nextLink = "api/campaigns/1400397/pledges?include=patron.null&page%5Bcount%5D=9999";

        do
        {
            var stringData = await SendStringRequest(nextLink);
            var document = Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentRoot<Pledge[]>>(stringData, new JsonApiSerializerSettings());
            if (document == null)
            {
                logger.Error("Failed to get list of patrons, no data from API.");
                return;
            }

            if (document.Errors.HasItems())
            {
                logger.Error("Failed to get list of patrons.");
                document.Errors.ForEach(a => logger.Error(a.Detail));
                return;
            }

            PatronsList.AddRange(document.Data.Where(a => a.declined_since == null).Select(a => a.patron?.full_name ?? string.Empty));
            if (document.Links.TryGetValue("next", out var value))
            {
                nextLink = value.Href;
            }
            else
            {
                break;
            }
        }
        while (!nextLink.IsNullOrEmpty());

        PatronsList.AddRange(GetKofiMembers());
        PatronsList.Sort();
    }

    private static async Task SaveTokens(string accessToken, string refreshToken)
    {
        await Task.Delay(2000);
        var path = Path.Combine(PlaynitePaths.RuntimeDataDir, PlaynitePaths.PatreonConfigFileName);
        var config = new Dictionary<string, Dictionary<string, string>>
        {
            ["Patreon"] = new()
            {
                ["AccessToken"] = accessToken,
                ["RefreshToken"] = refreshToken
            }
        };

        try
        {
            await File.WriteAllTextAsync(path, Serialization.ToJson(config));
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

        if (response.IsSuccessStatusCode)
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

    public List<string> GetKofiMembers()
    {
        var members = new List<string>();
        var kofiMembersFile = Path.Combine(PlaynitePaths.RuntimeDataDir, "kofi.csv");
        if (!File.Exists(kofiMembersFile))
        {
            return members;
        }

        var lines = File.ReadAllLines(kofiMembersFile);
        if (lines.Length <= 1)
        {
            logger.Error("Can't parse Kofi members, no members in csv.");
            return members;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.IsNullOrWhiteSpace())
            {
                break;
            }

            var vars = line.Split(',');
            if (vars.Length > 0)
            {
                members.Add(vars[0].Trim('"'));
            }
        }

        return members;
    }
}
