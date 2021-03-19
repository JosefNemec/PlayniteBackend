using ComposableAsync;
using MongoDB.Driver;
using Newtonsoft.Json;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Controllers.IGDB.DataGetter;
using PlayniteServices.Databases;
using PlayniteServices.Models.IGDB;
using RateLimiter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB
{
    public class IgdbApi
    {
        public class AuthResponse
        {
            public string access_token { get; set; }
        }

        private static ILogger logger = LogManager.GetLogger();
        private static readonly char[] arrayTrim = new char[] { '[', ']' };
        private static readonly JsonSerializer jsonSerializer = new JsonSerializer();
        private readonly UpdatableAppSettings settings;
        private readonly DelegatingHandler requestLimiterHandler;

        public Games Games;
        public AlternativeNames AlternativeNames;
        public InvolvedCompanies InvolvedCompanies;
        public Genres Genres;
        public Websites Websites;
        public GameModes GameModes;
        public PlayerPerspectives PlayerPerspectives;
        public Covers Covers;
        public Artworks Artworks;
        public Screenshots Screenshots;
        public AgeRatings AgeRatings;
        public Collections Collections;
        public Companies Companies;

        public HttpClient HttpClient { get; }

        public IgdbApi(UpdatableAppSettings settings)
        {
            this.settings = settings;
            requestLimiterHandler = TimeLimiter
                .GetFromMaxCountByInterval(4, TimeSpan.FromSeconds(1))
                .AsDelegatingHandler();
            HttpClient = new HttpClient(requestLimiterHandler);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            Games = new Games(this);
            AlternativeNames = new AlternativeNames(this);
            InvolvedCompanies = new InvolvedCompanies(this);
            Genres = new Genres(this);
            Websites = new Websites(this);
            GameModes = new GameModes(this);
            PlayerPerspectives = new PlayerPerspectives(this);
            Covers = new Covers(this);
            Artworks = new Artworks(this);
            Screenshots = new Screenshots(this);
            AgeRatings = new AgeRatings(this);
            Collections = new Collections(this);
            Companies = new Companies(this);
        }

        private static async Task SaveTokens(string accessToken)
        {
            await Task.Delay(2000);
            var path = Path.Combine(Paths.ExecutingDirectory, "twitchTokens.json");
            var config = new Dictionary<string, object>()
            {
                { "IGDB", new Dictionary<string, string>()
                    {
                        { "AccessToken", accessToken }
                    }
                }
            };

            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to save new twitch API token.");
            }
        }

        private async Task Authenticate()
        {
            var clientId = settings.Settings.IGDB.ClientId;
            var clientSecret = settings.Settings.IGDB.ClientSecret;
            var authUrl = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials";
            var response = await HttpClient.PostAsync(authUrl, null);
            var auth = Serialization.FromJson<AuthResponse>(await response.Content.ReadAsStringAsync());
            if (auth?.access_token.IsNullOrEmpty() != false)
            {
                throw new Exception("Failed to authenticate IGDB.");
            }
            else
            {
                logger.Info($"New IGDB auth token generated: {auth.access_token}");
                settings.Settings.IGDB.AccessToken = auth.access_token;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                SaveTokens(auth.access_token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        private HttpRequestMessage CreateRequest(string url, string query, string apiKey)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(settings.Settings.IGDB.ApiEndpoint + url),
                Method = HttpMethod.Post,
                Content = new StringContent(query)
            };

            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("Client-ID", settings.Settings.IGDB.ClientId);
            return request;
        }

        public async Task<string> SendStringRequest(string url, string query, bool reTry = true)
        {
            logger.Debug($"IGDB Live: {url}, {query}");
            var sharedRequest = CreateRequest(url, query, settings.Settings.IGDB.AccessToken);
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
                return await SendStringRequest(url, query, false);
            }
            else if (authFailed)
            {
                throw new Exception($"Failed to authenticate IGDB {response.StatusCode}.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && reTry)
            {
                await Task.Delay(250);
                return await SendStringRequest(url, query, false);
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
                if (errorMessage.Contains("Internal server error") && reTry)
                {
                    return await SendStringRequest(url, query, false);
                }
                else
                {
                    throw new Exception($"Uknown IGDB API response {response.StatusCode}.");
                }
            }
        }

        public async Task<ulong> GetSteamIgdbMatch(ulong gameId)
        {
            var filter = Builders<SteamIdGame>.Filter.Eq(a => a.steamId, gameId);
            var cache = Database.Instance.SteamIgdbMatches.Find(filter).FirstOrDefault();
            if (cache != null)
            {
                return cache.igdbId;
            }

            var libraryStringResult = await SendStringRequest("games",
                $"fields id; where external_games.uid = \"{gameId}\" & external_games.category = 1; limit 1;");
            var games = JsonConvert.DeserializeObject<List<Game>>(libraryStringResult);
            if (games.Any())
            {
                var game = games.First();
                Database.Instance.SteamIgdbMatches.InsertOne(new SteamIdGame()
                {
                    steamId = gameId,
                    igdbId = game.id
                });

                return game.id;
            }
            else
            {
                return 0;
            }
        }

        public async Task<TItem> GetItem<TItem>(ulong itemId, string endpointPath, IMongoCollection<TItem> collection) where TItem : IgdbItem
        {
            if (itemId == 0)
            {
                return null;
            }

            var filter = Builders<TItem>.Filter.Eq(nameof(IgdbItem.id), itemId);
            var cacheItem = collection.Find(filter).FirstOrDefault();
            if (cacheItem != null)
            {
                return cacheItem;
            }

            var stringResult = await SendStringRequest(endpointPath, $"fields *; where id = {itemId};");
            var items = Serialization.FromJson<List<TItem>>(stringResult);

            TItem item;
            // IGDB resturns empty results if an id is a duplicate of another game
            if (items.Count > 0)
            {
                item = items[0];
            }
            else
            {
                item = typeof(TItem).CrateInstance<TItem>();
            }

            collection.InsertOne(item);
            return item;
        }

        public async Task<List<TItem>> GetItem<TItem>(List<ulong> itemIds, string endpointPath, IMongoCollection<TItem> collection) where TItem : IgdbItem
        {
            if (!itemIds.HasItems())
            {
                return null;
            }

            var filter = Builders<TItem>.Filter.In(nameof(IgdbItem.id), itemIds);
            var cacheItems = collection.Find(filter).ToList();
            if (cacheItems.Count == itemIds.Count)
            {
                return cacheItems;
            }

            var idsToGet = ListExtensions.GetDistinctItemsP(itemIds, cacheItems.Select(a => a.id));
            var stringResult = await SendStringRequest(endpointPath, $"fields *; where id = ({string.Join(',', idsToGet)}); limit 500;");
            var items = Serialization.FromJson<List<TItem>>(stringResult);
            collection.InsertMany(items);
            cacheItems.AddRange(items);
            return cacheItems;
        }
    }
}
