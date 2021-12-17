using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Databases;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SdkModels = Playnite.SDK.Models;

namespace PlayniteServices.Controllers.IGDB
{
    [Route("igdb")]
    public class SearchController : Controller
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private static readonly Regex separatorRegex = new Regex(@"\s*(:|-)\s*", RegexOptions.Compiled);
        private static readonly Regex noIntroArticleRegEx = new Regex(@",\s*(the|a|an|der|das|die)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly char[] bracketsMatchList = new char[] { '[', ']', '(', ')', '{', '}' };

        private readonly UpdatableAppSettings settings;
        private readonly IgdbApi igdbApi;

        public SearchController(UpdatableAppSettings settings, IgdbApi igdbApi)
        {
            this.settings = settings;
            this.igdbApi = igdbApi;
        }

        [HttpPost("search")]
        public ServicesResponse<List<Game>> Search([FromBody] SearchRequest search)
        {
            return new ServicesResponse<List<Game>>(SearchGames(search.Name));
        }

        [HttpPost("matchgame")]
        public async Task<ServicesResponse<ExpandedGame>> MatchGame([FromBody] GameMatchRequest match)
        {
            var isPluginGame = match.PluginId != Guid.Empty;
            // Only GOG and Steam has reasonable results on IGDB at the moment
            var isSteamPlugin = BuiltinExtensions.GetIdFromExtension(BuiltinExtension.SteamLibrary) == match.PluginId;
            var isGogPlugin = BuiltinExtensions.GetIdFromExtension(BuiltinExtension.GogLibrary) == match.PluginId;

            ulong gameId = 0;
            if (isPluginGame && (isSteamPlugin || isGogPlugin))
            {
                var category = isSteamPlugin ? ExternalGameCategory.Steam : ExternalGameCategory.Gog;
                gameId = igdbApi.ExternalGames.GetExternalGame(category, match.GameId)?.id ?? 0;
                if (gameId != 0)
                {
                    return new ServicesResponse<ExpandedGame>(await igdbApi.Games.GetExpanded(gameId));
                }
            }

            var matchId = match.GetMatchId();
            if (gameId == 0)
            {
                var existingMatch = Database.Instance.IgdbGameMatches.
                    Find(Builders<IgdbGameMatch>.Filter.Eq(a => a.MatchId, matchId)).
                    FirstOrDefault();
                if (existingMatch != null)
                {
                    gameId = existingMatch.GameId;
                }
            }

            if (gameId != 0)
            {
                return new ServicesResponse<ExpandedGame>(await igdbApi.Games.GetExpanded(gameId));
            }

            var foundMetadata = new ExpandedGame();
            gameId = await TryMatchGame(game, false);
            var useAlt = settings.Settings.IGDB.AlternativeSearch && !game.Name.ContainsAny(whereQueryBlacklist);
            if (useAlt && gameId == 0)
            {
                gameId = await TryMatchGame(game, true);
            }

                if (gameId != 0)
                {
                    foundMetadata = await expandFunc(gameId);
                }

            // Update match database if match was found
            if (gameId != 0)
            {
                if (isKnownPlugin && !isSteamPlugin)
                {
                    Database.Instance.IGBDGameIdMatches.ReplaceOne(
                        Builders<GameIdMatch>.Filter.Eq(a => a.Id, matchId),
                        new GameIdMatch
                        {
                            GameId = game.GameId,
                            Id = matchId,
                            IgdbId = gameId,
                            Library = game.PluginId
                        },
                        Database.ItemUpsertOptions);
                }

                Database.Instance.IGDBSearchIdMatches.ReplaceOne(
                    Builders<SearchIdMatch>.Filter.Eq(a => a.Id, matchId),
                    new SearchIdMatch
                    {
                        Term = game.Name,
                        Id = matchId,
                        IgdbId = gameId
                    },
                    Database.ItemUpsertOptions);
            }

            return new ServicesResponse<T>(foundMetadata);

            return new ServicesResponse<ExpandedGame>(null);
        }

        private List<Game> SearchGames(string gameName)
        {
            return igdbApi.Games.Collection.
                Find(Builders<Game>.Filter.Text(gameName)).
                Project<Game>(Builders<Game>.Projection.MetaTextScore("textScore")).
                Sort(Builders<Game>.Sort.MetaTextScore("textScore")).
                Limit(50).
                ToList();
        }

        private string FixNointroNaming(string name)
        {
            var match = noIntroArticleRegEx.Match(name);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim() + " " + noIntroArticleRegEx.Replace(name, "");
            }

            return name;
        }

        private async Task<ulong> TryMatchGame(SdkModels.Game game, bool alternativeSearch)
        {
            if (game.Name.IsNullOrEmpty())
            {
                return 0;
            }

            if (game.Name.ContainsAny(bracketsMatchList))
            {
                return 0;
            }

            ulong matchedGame = 0;
            var copyGame = game.GetClone();
            copyGame.Name = StringExtensions.NormalizeGameName(game.Name);
            copyGame.Name = FixNointroNaming(copyGame.Name);
            copyGame.Name = copyGame.Name.Replace(@"\", string.Empty);
            var name = copyGame.Name;
            name = Regex.Replace(name, @"\s+RHCP$", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @"\s+RU$", "", RegexOptions.IgnoreCase);

            var results = SearchGames(name);
            results.ForEach(a => a.name = StringExtensions.NormalizeGameName(a.name));
            string testName = string.Empty;

            // Direct comparison
            matchedGame = MatchFun(game, name, results);
            if (matchedGame > 0)
            {
                return matchedGame;
            }

            // Try replacing roman numerals: 3 => III
            testName = Regex.Replace(name, @"\d+", ReplaceNumsForRomans);
            matchedGame = MatchFun(game, testName, results);
            if (matchedGame > 0)
            {
                return matchedGame;
            }

            // Try adding The
            testName = "The " + name;
            matchedGame = MatchFun(game, testName, results);
            if (matchedGame > 0)
            {
                return matchedGame;
            }

            // Try chaning & / and
            testName = Regex.Replace(name, @"\s+and\s+", " & ", RegexOptions.IgnoreCase);
            matchedGame = MatchFun(game, testName, results);
            if (matchedGame > 0)
            {
                return matchedGame;
            }

            // Try removing apostrophes
            var resCopy = results.GetClone();
            resCopy.ForEach(a => a.name = a.name.Replace("'", ""));
            matchedGame = MatchFun(game, name, resCopy);
            if (matchedGame > 0)
            {
                return matchedGame;
            }

            // Try removing all ":" and "-"
            testName = separatorRegex.Replace(name, " ");
            resCopy = results.GetClone();
            foreach (var res in resCopy)
            {
                res.name = separatorRegex.Replace(res.name, " ");
                if (res.alternative_names.HasItems())
                {
                    res.alternative_names = res.alternative_names.Where(a => !a.name.IsNullOrEmpty()).ToList();
                    res.alternative_names.ForEach(a => a.name = separatorRegex.Replace(a.name, " "));
                }
            }

            matchedGame = MatchFun(game, testName, resCopy);
            if (matchedGame > 0)
            {
                return matchedGame;
            }

            // Try without subtitle
            if (!alternativeSearch)
            {
                var testResult = results.OrderBy(a => a.first_release_date).FirstOrDefault(a =>
                {
                    if (a.first_release_date == 0)
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(a.name) && a.name.Contains(":"))
                    {
                        return string.Equals(name, a.name.Split(':')[0], StringComparison.InvariantCultureIgnoreCase);
                    }

                    return false;
                });

                if (testResult != null)
                {
                    return testResult.id;
                }
            }

            return 0;
        }

        private ulong MatchFun(SdkModels.Game game, string matchName, List<ExpandedGame> list)
        {
            var res = list.Where(a => string.Equals(matchName, a.name, StringComparison.InvariantCultureIgnoreCase));
            if (!res.Any())
            {
                res = list.Where(a =>
                a.alternative_names.HasItems() &&
                a.alternative_names.Select(b => b.name).ContainsString(matchName) == true);
            }

            if (res.Any())
            {
                if (res.Count() == 1)
                {
                    return res.First().id;
                }
                else
                {
                    if (game.ReleaseDate != null)
                    {
                        var igdbGame = res.FirstOrDefault(a => a.first_release_date.ToDateFromUnixMs().Year == game.ReleaseDate.Value.Year);
                        if (igdbGame != null)
                        {
                            return igdbGame.id;
                        }
                    }
                    else
                    {
                        // If multiple matches are found and we don't have release date then prioritize older game
                        if (res.All(a => a.first_release_date == 0))
                        {
                            return res.First().id;
                        }
                        else
                        {
                            var igdbGame = res.OrderBy(a => a.first_release_date).First(a => a.first_release_date > 0);
                            return igdbGame.id;
                        }
                    }
                }
            }

            return 0;
        }

        private string ReplaceNumsForRomans(Match m)
        {
            if (int.TryParse(m.Value, out var intVal))
            {
                return Roman.To(intVal);
            }
            else
            {
                return m.Value;
            }
        }
    }
}
