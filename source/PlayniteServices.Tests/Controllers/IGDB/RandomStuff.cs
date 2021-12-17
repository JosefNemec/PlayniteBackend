using MongoDB.Driver;
using Newtonsoft.Json;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices;
using PlayniteServices.Controllers.IGDB;
using PlayniteServices.Databases;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SdkModels = Playnite.SDK.Models;

namespace PlayniteServicesTests.Controllers.IGDB
{
    [Collection("DefaultCollection")]
    public class RandomStuff
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly HttpClient client;
        private readonly Microsoft.AspNetCore.TestHost.TestServer server;

        public RandomStuff(TestFixture<Startup> fixture)
        {
            client = fixture.Client;
            server = fixture.Server;
        }

        [Fact]
        public async Task DatabaseCloneTest()
        {
            var igdb = (IgdbApi)server.Services.GetService(typeof(IgdbApi));
            //await igdb.CloneDatabase();
            var game = igdb.ExternalGames.GetExternalGame(ExternalGameCategory.Steam, "7200");
        }

        [Fact]
        public async Task SearchTest()
        {
            var search = new SearchRequest { Name = "quake" };
            var content = new StringContent(Serialization.ToJson(search), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync(@"/igdb/search", content);
            var games = Serialization.FromJson<ServicesResponse<List<Game>>>(await response.Content.ReadAsStringAsync()).Data;
        }

        //[Fact]
        public void Random()
        {
            ulong testnum = 9856914317583057114;
            //Database.Instance.SteamIgdbMatches.ReplaceOne(
            //    Builders<SteamIdGame>.Filter.Eq(a => a.steamId, testnum),
            //        new SteamIdGame()
            //        {
            //            steamId = testnum,
            //            igdbId = 5165165
            //        }, Database.ItemUpsertOptions);

            //var test = Database.Instance.SteamIgdbMatches.Find((a) => a.steamId == testnum).FirstOrDefault();

            var igdb = new IgdbApi(null);
            //igdb.Games.Collection.InsertOne(new Game { id = testnum, involved_companies = new List<ulong> { testnum } });

            var test = igdb.Games.Collection.Find((a) => a.id == testnum).FirstOrDefault();
        }

        //[Fact]
        public void ConvertCache()
        {
            //var items1 = Serialization.FromJsonFile<List<GameIdMatch>>(@"e:\Devel\igdb\IGDBGameIdMatches.json");
            //foreach (var item in items1)
            //{
            //    Database.Instance.IGBDGameIdMatches.ReplaceOne(
            //        Builders<GameIdMatch>.Filter.Eq(a => a.Id, item.Id),
            //        item,
            //        Database.ItemUpsertOptions);
            //}

            //var items2 = Serialization.FromJsonFile<List<SteamIdGame>>(@"e:\Devel\igdb\IGBDSteamIdCache.json");
            //foreach (var item in items2)
            //{
            //    Database.Instance.SteamIgdbMatches.ReplaceOne(
            //        Builders<SteamIdGame>.Filter.Eq(a => a.steamId, item.steamId),
            //        item,
            //        Database.ItemUpsertOptions);
            //}

            //var items3 = Serialization.FromJsonFile<List<SearchIdMatch>>(@"e:\Devel\igdb\IGDBSearchIdMatches.json");
            //foreach (var item in items3)
            //{
            //    Database.Instance.IGDBSearchIdMatches.ReplaceOne(
            //        Builders<SearchIdMatch>.Filter.Eq(a => a.Id, item.Id),
            //        item,
            //        Database.ItemUpsertOptions);
            //}

            void processCollection<T>(IMongoCollection<T> collection, string dir) where T : IgdbItem
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    var item = Serialization.FromJsonFile<T>(file);
                    collection.ReplaceOne(
                        Builders<T>.Filter.Eq(u => u.id, item.id),
                        item,
                        Database.ItemUpsertOptions);
                }
            }

            var igdb = new IgdbApi(null);
            foreach (var dir in Directory.GetDirectories(@"e:\Devel\igdb\"))
            {
                logger.Info(dir);
                var dirName = Path.GetFileName(dir);
                switch (dirName)
                {
                    case "age_ratings":
                        processCollection(igdb.AgeRatings.Collection, dir);
                        break;
                    case "alternative_names":
                        processCollection(igdb.AlternativeNames.Collection, dir);
                        break;
                    case "artworks":
                        processCollection(igdb.Artworks.Collection, dir);
                        break;
                    case "collections":
                        processCollection(igdb.Collections.Collection, dir);
                        break;
                    case "companies":
                        processCollection(igdb.Companies.Collection, dir);
                        break;
                    case "covers":
                        processCollection(igdb.Covers.Collection, dir);
                        break;
                    case "game_modes":
                        processCollection(igdb.GameModes.Collection, dir);
                        break;
                    case "games":
                        processCollection(igdb.Games.Collection, dir);
                        break;
                    case "genres":
                        processCollection(igdb.Genres.Collection, dir);
                        break;
                    case "involved_companies":
                        processCollection(igdb.InvolvedCompanies.Collection, dir);
                        break;
                    case "player_perspectives":
                        processCollection(igdb.PlayerPerspectives.Collection, dir);
                        break;
                    case "screenshots":
                        processCollection(igdb.Screenshots.Collection, dir);
                        break;
                    case "websites":
                        processCollection(igdb.Websites.Collection, dir);
                        break;
                    default:
                        throw new Exception($"Uknown collection {dirName}");
                }
            }
        }
    }
}
