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
    public class IgdbApi : IDisposable
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
        private System.Threading.Timer webhookTimer;

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
        public Keywords Keywords;
        public MultiplayerModes MultiplayerModes;
        public Franchises Franchises;
        public Themes Themes;
        public ExternalGames ExternalGames;
        public Videos Videos;
        public ReleaseDates ReleaseDates;

        public HttpClient HttpClient { get; }

        public IgdbApi(UpdatableAppSettings settings)
        {
            this.settings = settings;
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
            Keywords = new Keywords(this);
            MultiplayerModes = new MultiplayerModes(this);
            Franchises = new Franchises(this);
            Themes = new Themes(this);
            ExternalGames = new ExternalGames(this);
            Videos = new Videos(this);
            ReleaseDates = new ReleaseDates(this);

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
                async Task registerHook(string method, string collection)
                {
                    await SendStringRequest(
                        $"{collection}/webhooks",
                        new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "method", method },
                            { "secret", settings.Settings.IGDB.WebHookSecret },
                            { "url", $"http://api.playnite.link/api/igdb/webhooks/{collection}/{method}" }
                        }),
                        HttpMethod.Post,
                        false);
                }

                try
                {
                    var webhooksString = await SendStringRequest("webhooks", null, HttpMethod.Get, true);
                    var webhooks = Serialization.FromJson<List<Webhook>>(webhooksString);
                    if (!webhooks.HasItems() || webhooks.Any(a => a.active == false))
                    {
                        logger.Error("IGDB webhooks are NOT active.");
                        logger.Error(webhooksString);
                        await registerHook("create", "games");
                        await registerHook("delete", "games");
                        await registerHook("update", "games");

                        webhooksString = await SendStringRequest("webhooks", null, HttpMethod.Get, true);
                        webhooks = Serialization.FromJson<List<Webhook>>(webhooksString);
                        if (!webhooks.HasItems() || webhooks.Any(a => a.active == false))
                        {
                            logger.Error("Failed to register IGDB webhooks.");
                            logger.Error(webhooksString);
                        }
                        else
                        {
                            logger.Info("Registered new IGDB webhooks.");
                            logger.Info(webhooksString);
                        }
                    }
                    else
                    {
                        logger.Info("IGDB webhooks are active.");
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
                Database.Instance.SteamIgdbMatches.ReplaceOne(
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
            var items = Serialization.FromJson<List<TItem>>(stringResult);
            items.ForEach(a => collection.ReplaceOne(
                Builders<TItem>.Filter.Eq(u => u.id, a.id),
                a,
                Database.ItemUpsertOptions));
            cacheItems.AddRange(items);
            return cacheItems;
        }

        public void Add<TItem>(IEnumerable<TItem> items, IMongoCollection<TItem> collection) where TItem : IgdbItem
        {
            items.ForEach(a => collection.ReplaceOne(
              Builders<TItem>.Filter.Eq(u => u.id, a.id),
              a,
              Database.ItemUpsertOptions));
        }

        public void Add<TItem>(TItem item, IMongoCollection<TItem> collection) where TItem : IgdbItem
        {
            collection.ReplaceOne(
              Builders<TItem>.Filter.Eq(u => u.id, item.id),
              item,
              Database.ItemUpsertOptions);
        }

        private async Task<ulong> GetCollectionCount(string collectionName)
        {
            var stringResult = await SendStringRequest(collectionName + "/count", null, HttpMethod.Post);
            var response = Serialization.FromJson<Dictionary<string, ulong>>(stringResult);
            return response["count"];
        }

        public async Task CloneDatabase()
        {
            Games.DropCollection();
            AlternativeNames.DropCollection();
            InvolvedCompanies.DropCollection();
            Genres.DropCollection();
            Websites.DropCollection();
            GameModes.DropCollection();
            PlayerPerspectives.DropCollection();
            Covers.DropCollection();
            Artworks.DropCollection();
            Screenshots.DropCollection();
            AgeRatings.DropCollection();
            Collections.DropCollection();
            Companies.DropCollection();
            Platforms.DropCollection();
            Keywords.DropCollection();
            MultiplayerModes.DropCollection();
            Franchises.DropCollection();
            Themes.DropCollection();
            ExternalGames.DropCollection();
            Videos.DropCollection();
            ReleaseDates.DropCollection();

            logger.Debug("DB games clone start " + DateTime.Now.ToString("HH:mm:ss"));

            var gameCount = await GetCollectionCount("games");
            var gamesQueryBase = $"fields checksum,version_parent,name,slug,url,status,rating,aggregated_rating,total_rating,summary,storyline,first_release_date,version_title,category,age_ratings.*,alternative_names.*,artworks.*,bundles,collection.*,cover.*,dlcs,expanded_games,expansions,external_games.*,forks,franchise.*,franchises.*,game_modes.*,genres.*,involved_companies.*,keywords.*,multiplayer_modes.*,parent_game,platforms.*,player_perspectives.*,ports,release_dates.*,remakes,remasters,screenshots.*,similar_games,standalone_expansions,themes.*,videos.*,websites.*; limit 500;";

            for (ulong i = 0; i < gameCount; i += 500)
            {
                var query = $"{gamesQueryBase} offset {i};";
                var stringData = await SendStringRequest("games", query);
                var games = Serialization.FromJson<List<DbClonningGame>>(stringData);
                foreach (var game in games)
                {
                    var storeGame = new Game
                    {
                        id = game.id,
                        name = game.name,
                        slug = game.slug,
                        url = game.url,
                        checksum = game.checksum,
                        summary = game.summary,
                        storyline = game.storyline,
                        version_parent = game.version_parent,
                        category = game.category,
                        first_release_date = game.first_release_date,
                        rating = game.rating,
                        aggregated_rating = game.aggregated_rating,
                        total_rating = game.total_rating,
                        similar_games = game.similar_games,
                        parent_game = game.parent_game,
                        bundles = game.bundles,
                        dlcs = game.dlcs,
                        expanded_games = game.expanded_games,
                        expansions = game.expansions,
                        forks = game.forks,
                        ports = game.ports,
                        remakes = game.remakes,
                        remasters = game.remasters,
                        standalone_expansions = game.standalone_expansions,
                        status = game.status
                    };

                    if (game.franchise != null)
                    {
                        Franchises.Add(game.franchise);
                        storeGame.franchise = game.franchise.id;
                    }

                    if (game.collection != null)
                    {
                        Collections.Add(game.collection);
                        storeGame.collection = game.collection.id;
                    }

                    if (game.involved_companies.HasItems())
                    {
                        InvolvedCompanies.Add(game.involved_companies);
                        storeGame.involved_companies = game.involved_companies.Select(a => a.id).ToList();
                    }

                    if (game.genres.HasItems())
                    {
                        Genres.Add(game.genres);
                        storeGame.genres = game.genres.Select(a => a.id).ToList();
                    }

                    if (game.themes.HasItems())
                    {
                        Themes.Add(game.themes);
                        storeGame.themes = game.themes.Select(a => a.id).ToList();
                    }

                    if (game.game_modes.HasItems())
                    {
                        GameModes.Add(game.game_modes);
                        storeGame.game_modes = game.game_modes.Select(a => a.id).ToList();
                    }

                    if (game.cover != null)
                    {
                        Covers.Add(game.cover);
                        storeGame.cover = game.cover.id;
                    }

                    if (game.websites.HasItems())
                    {
                        Websites.Add(game.websites);
                        storeGame.websites = game.websites.Select(a => a.id).ToList();
                    }

                    if (game.player_perspectives.HasItems())
                    {
                        PlayerPerspectives.Add(game.player_perspectives);
                        storeGame.player_perspectives = game.player_perspectives.Select(a => a.id).ToList();
                    }

                    if (game.franchises.HasItems())
                    {
                        Franchises.Add(game.franchises);
                        storeGame.franchises = game.franchises.Select(a => a.id).ToList();
                    }

                    if (game.keywords.HasItems())
                    {
                        Keywords.Add(game.keywords);
                        storeGame.keywords = game.keywords.Select(a => a.id).ToList();
                    }

                    if (game.multiplayer_modes.HasItems())
                    {
                        MultiplayerModes.Add(game.multiplayer_modes);
                        storeGame.multiplayer_modes = game.multiplayer_modes.Select(a => a.id).ToList();
                    }

                    if (game.alternative_names.HasItems())
                    {
                        AlternativeNames.Add(game.alternative_names);
                        storeGame.alternative_names = game.alternative_names.Select(a => a.id).ToList();
                    }

                    if (game.external_games.HasItems())
                    {
                        ExternalGames.Add(game.external_games);
                        storeGame.external_games = game.external_games.Select(a => a.id).ToList();
                    }

                    if (game.screenshots.HasItems())
                    {
                        Screenshots.Add(game.screenshots);
                        storeGame.screenshots = game.screenshots.Select(a => a.id).ToList();
                    }

                    if (game.artworks.HasItems())
                    {
                        Artworks.Add(game.artworks);
                        storeGame.artworks = game.artworks.Select(a => a.id).ToList();
                    }

                    if (game.videos.HasItems())
                    {
                        Videos.Add(game.videos);
                        storeGame.videos = game.videos.Select(a => a.id).ToList();
                    }

                    if (game.platforms.HasItems())
                    {
                        Platforms.Add(game.platforms);
                        storeGame.platforms = game.platforms.Select(a => a.id).ToList();
                    }

                    if (game.release_dates.HasItems())
                    {
                        ReleaseDates.Add(game.release_dates);
                        storeGame.release_dates = game.release_dates.Select(a => a.id).ToList();
                    }

                    if (game.age_ratings.HasItems())
                    {
                        AgeRatings.Add(game.age_ratings);
                        storeGame.age_ratings = game.age_ratings.Select(a => a.id).ToList();
                    }

                    Games.Collection.ReplaceOne(
                        Builders<Game>.Filter.Eq(u => u.id, storeGame.id),
                        storeGame,
                        Database.ItemUpsertOptions);
                }
            }

            logger.Debug("DB games end " + DateTime.Now.ToString("HH:mm:ss"));

            logger.Debug("DB companies clone start " + DateTime.Now.ToString("HH:mm:ss"));
            var companiesCount = await GetCollectionCount("companies");
            var companiesQueryBase = $"fields name,slug,url,checksum,logo,country,description,parent,developed,published; limit 500;";
            for (ulong i = 0; i < companiesCount; i += 500)
            {
                var query = $"{companiesQueryBase} offset {i};";
                var stringData = await SendStringRequest("companies", query);
                var companies = Serialization.FromJson<List<Company>>(stringData);

                companies.ForEach(a => Companies.Collection.ReplaceOne(
                    Builders<Company>.Filter.Eq(u => u.id, a.id),
                    a,
                    Database.ItemUpsertOptions));
            }

            logger.Debug("DB companies end " + DateTime.Now.ToString("HH:mm:ss"));
        }

        //private async Task CloneCollection<T>(DataGetter<T> collection) where T : IgdbItem
        //{
        //    logger.Debug($"DB clone {collection.EndpointPath} start {DateTime.Now:HH:mm:ss}");
        //    var colCount = await GetCollectionCount(collection.EndpointPath);
        //    for (ulong i = 0; i < colCount; i += 500)
        //    {
        //        var query = $"fields *; limit 500; offset {i};";
        //        var stringData = await SendStringRequest(collection.EndpointPath, query);
        //        var items = Serialization.FromJson<List<T>>(stringData);
        //        items.ForEach(a => collection.Collection.ReplaceOne(
        //            Builders<T>.Filter.Eq(u => u.id, a.id),
        //            a,
        //            Database.ItemUpsertOptions));
        //    }

        //    logger.Debug($"DB clone {collection.EndpointPath} end {DateTime.Now:HH:mm:ss}");
        //}
    }
}
