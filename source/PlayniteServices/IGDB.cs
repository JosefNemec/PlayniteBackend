using ComposableAsync;
using MongoDB.Driver;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Controllers.IGDB.DataGetter;
using PlayniteServices.Models.IGDB;
using RateLimiter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices
{
    public class IgdbApi : IDisposable
    {
        public class AuthResponse
        {
            public string access_token { get; set; }
        }

        private static bool instantiated = false;
        private static readonly ILogger logger = LogManager.GetLogger();
        private static readonly char[] arrayTrim = new char[] { '[', ']' };
        private readonly UpdatableAppSettings settings;
        public readonly Database Database;
        private readonly DelegatingHandler requestLimiterHandler;
        private readonly System.Threading.Timer webhookTimer;

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
        public Platforms Platforms;

        public HttpClient HttpClient { get; }

        public IgdbApi(UpdatableAppSettings settings, Database db)
        {
            TestAssert.IsFalse(instantiated, $"{nameof(IgdbApi)} already instantiated");
            instantiated = true;
            this.settings = settings;
            this.Database = db;
            requestLimiterHandler = TimeLimiter
                .GetFromMaxCountByInterval(4, TimeSpan.FromSeconds(1))
                .AsDelegatingHandler();
            HttpClient = new HttpClient(requestLimiterHandler);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.Timeout = new TimeSpan(0, 0, 50);

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
            Platforms = new Platforms(this);

            if (settings.Settings.IGDB.RegisterWebhooks)
            {
                webhookTimer = new System.Threading.Timer(
                    (_) => RegiserWebhooks(),
                    null,
                    new TimeSpan(0),
                    new TimeSpan(1, 0, 0));
            }
        }

        public void Dispose()
        {
            webhookTimer?.Dispose();
        }

        private Task RegiserWebhooks()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var webhooksString = await SendStringRequest("webhooks", null, HttpMethod.Get, true);
                    var webhooks = DataSerialization.FromJson<List<Webhook>>(webhooksString);
                    if (!webhooks.HasItems() || !webhooks[0].active)
                    {
                        logger.Error("IGDB webhook is NOT active.");
                        logger.Error(webhooksString);

                        var registeredStr = await SendStringRequest(
                            "games/webhooks",
                            new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                { "method", "update" },
                                { "secret", settings.Settings.IGDB.WebHookSecret },
                                { "url", "http://api.playnite.link/api/igdb/game" }
                            }),
                            HttpMethod.Post,
                            false);
                        var registeredHooks = DataSerialization.FromJson<List<Webhook>>(registeredStr);
                        if (!registeredHooks.HasItems() || !registeredHooks[0].active)
                        {
                            logger.Error("Failed to register IGDB webhook.");
                            logger.Error(registeredStr);
                        }
                        else
                        {
                            logger.Info("Registered new IGDB webhook.");
                            logger.Info(registeredStr);
                        }
                    }
                    else
                    {
                        logger.Info("IGDB webhook is active.");
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to register IGDB webhooks.");
                }
            });
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
            var clientId = settings.Settings.IGDB.ClientId;
            var clientSecret = settings.Settings.IGDB.ClientSecret;
            var authUrl = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials";
            var response = await HttpClient.PostAsync(authUrl, null);
            var auth = DataSerialization.FromJson<AuthResponse>(await response.Content.ReadAsStringAsync());
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

        private HttpRequestMessage CreateRequest(string url, HttpContent content, HttpMethod method)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(settings.Settings.IGDB.ApiEndpoint + url),
                Method = method,
                Content = content
            };

            request.Headers.Add("Authorization", $"Bearer {settings.Settings.IGDB.AccessToken}");
            request.Headers.Add("Client-ID", settings.Settings.IGDB.ClientId);
            return request;
        }

        public async Task<string> SendStringRequest(string url, string query, bool reTry = true)
        {
            logger.Debug($"IGDB Live: {url}, {query}");
            return await SendStringRequest(url, new StringContent(query), HttpMethod.Post, reTry);
        }

        public async Task<string> SendStringRequest(string url, HttpContent content, HttpMethod method, bool reTry = true)
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
                if (errorMessage.Contains("Internal server error") && reTry)
                {
                    return await SendStringRequest(url, content, method, false);
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
            var cache = Database.SteamIgdbMatches.Find(filter).FirstOrDefault();
            if (cache != null)
            {
                return cache.igdbId;
            }

            var libraryStringResult = await SendStringRequest("games",
                $"fields id; where external_games.uid = \"{gameId}\" & external_games.category = 1; limit 1;");
            var games = DataSerialization.FromJson<List<Game>>(libraryStringResult);
            if (games.Any())
            {
                var game = games.First();
                Database.SteamIgdbMatches.ReplaceOne(
                    Builders<SteamIdGame>.Filter.Eq(a => a.steamId, gameId),
                    new SteamIdGame()
                    {
                        steamId = gameId,
                        igdbId = game.id
                    },
                    Database.ItemUpsertOptions);

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
            var items = DataSerialization.FromJson<List<TItem>>(stringResult);

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

            collection.ReplaceOne(
                Builders<TItem>.Filter.Eq(u => u.id, item.id),
                item,
                Database.ItemUpsertOptions);
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
            var items = DataSerialization.FromJson<List<TItem>>(stringResult);
            items.ForEach(a => collection.ReplaceOne(
                Builders<TItem>.Filter.Eq(u => u.id, a.id),
                a,
                Database.ItemUpsertOptions));
            cacheItems.AddRange(items);
            return cacheItems;
        }
    }
}
