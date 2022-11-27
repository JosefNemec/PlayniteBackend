using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class Patreon : IDisposable
    {
        private static bool instantiated = false;
        private readonly UpdatableAppSettings settings;
        private readonly HttpClient httpClient;

        public Patreon(UpdatableAppSettings settings)
        {
            TestAssert.IsFalse(instantiated, $"{nameof(Patreon)} already instantiated");
            instantiated = true;
            this.settings = settings;
            httpClient = new HttpClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private static void SaveTokens(string accessToken, string refreshToken)
        {
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

            File.WriteAllText(path, DataSerialization.ToJson(config));
        }

        private HttpRequestMessage CreateGetRequest(string url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = url.StartsWith("https") ? new Uri(url) : new Uri(settings.Settings.Patreon.ApiEndpoint + url),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", $"Bearer {settings.Settings.Patreon.AccessToken}");
            return request;
        }

        private async Task UpdateTokens()
        {
            var refreshToken = settings.Settings.Patreon.RefreshToken;
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
            var data = DataSerialization.FromJson<Dictionary<string, string>>(stringData);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SaveTokens(data["access_token"], data["refresh_token"]);
            }
        }

        public async Task<string> SendStringRequest(string url)
        {
            var request = CreateGetRequest(url);
            var response = await httpClient.SendAsync(request);

            // Access token probably expired (once every month)
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await UpdateTokens();
                request = CreateGetRequest(url);
                response = await httpClient.SendAsync(request);
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
