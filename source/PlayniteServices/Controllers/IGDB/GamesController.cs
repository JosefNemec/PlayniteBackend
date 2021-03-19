using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Playnite;
using System.Web;
using Playnite.SDK;
using Microsoft.Extensions.Options;
using PlayniteServices.Filters;
using System.Text.RegularExpressions;
using PlayniteServices.Controllers.IGDB.DataGetter;
using MongoDB.Driver;
using PlayniteServices.Databases;

namespace PlayniteServices.Controllers.IGDB
{
    [ServiceFilter(typeof(PlayniteVersionFilter))]
    [Route("igdb/games")]
    public class GamesController : Controller
    {
        private static readonly JsonSerializer jsonSerializer = new JsonSerializer();
        private static ILogger logger = LogManager.GetLogger();
        private static readonly char[] bracketsMatchList = new char[] { '[', ']', '(', ')', '{', '}' };

        private UpdatableAppSettings settings;
        private IgdbApi igdbApi;
        private AlternativeNames alternativeNames;

        public GamesController(UpdatableAppSettings settings, IgdbApi igdbApi)
        {
            this.settings = settings;
            this.igdbApi = igdbApi;
            alternativeNames = new AlternativeNames(igdbApi);
        }

        [HttpGet("{gameName}")]
        public async Task<ServicesResponse<List<ExpandedGameLegacy>>> Get(string gameName)
        {
            var search = await GetSearchResults(gameName, false);
            var altSearch = await GetSearchResults(gameName, settings.Settings.IGDB.AlternativeSearch);
            foreach (var alt in altSearch)
            {
                if (search.Any(a => a.id == alt.id))
                {
                    continue;
                }
                else
                {
                    search.Add(alt);
                }
            }

            return new ServicesResponse<List<ExpandedGameLegacy>>(search);
        }

        public async Task<List<ExpandedGameLegacy>> GetSearchResults(string searchString, bool alternativeSearch)
        {
            if (searchString.IsNullOrEmpty())
            {
                return new List<ExpandedGameLegacy>();
            }

            List<ulong> searchResult = null;
            var modifiedSearchString = ModelsUtils.GetIgdbSearchString(searchString);
            var filter = Builders<IgdbSearchResult>.Filter.Eq(a => a.Id, modifiedSearchString);
            var col = alternativeSearch ? Database.Instance.IgdbAltSearches : Database.Instance.IgdbStdSearches;

            var cached = col.Find(filter).FirstOrDefault();
            if (cached != null)
            {
                searchResult = cached.Games;
            }

            if (searchResult == null)
            {
                var matchString = HttpUtility.UrlDecode(modifiedSearchString);
                if (matchString.ContainsAny(bracketsMatchList))
                {
                    return new List<ExpandedGameLegacy>();
                }

                var whereQuery = $"where (name ~ *\"{matchString}\"*) | (alternative_names.name ~ *\"{matchString}\"*); fields id; limit 50;";
                var searchQuery = $"search \"{matchString}\"; fields id; limit 50;";
                var query = alternativeSearch ? whereQuery : searchQuery;
                var searchStringResult = await igdbApi.SendStringRequest("games", query);
                var tempRes = JsonConvert.DeserializeObject<List<Game>>(searchStringResult);
                searchResult = tempRes.Select(a => a.id).ToList();
                col.InsertOne(new IgdbSearchResult
                {
                    Id = modifiedSearchString,
                    Games = searchResult
                });
            }

            if (!searchResult.HasItems())
            {
                return new List<ExpandedGameLegacy>();
            }

            var finalResult = new List<ExpandedGameLegacy>();
            foreach (var game in await igdbApi.Games.Get(searchResult))
            {
                if (game.id == 0)
                {
                    continue;
                }

                var xpanded = new ExpandedGameLegacy()
                {
                    id = game.id,
                    name = game.name,
                    first_release_date = game.first_release_date * 1000
                };

                if (game.alternative_names?.Any() == true)
                {
                    xpanded.alternative_names = await alternativeNames.Get(game.alternative_names);
                }

                finalResult.Add(xpanded);
            }

            return finalResult;
        }
    }
}
