//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using MongoDB.Driver;
//using Playnite.Common;
//using Playnite.SDK;
//using PlayniteServices.Filters;
//using PlayniteServices.Models;
//using PlayniteServices.Models.IGDB;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using SdkModels = Playnite.SDK.Models;

//namespace PlayniteServices.Controllers.IGDB
//{
//    [ServiceFilter(typeof(PlayniteVersionFilter))]
//    [Route("igdb")]
//    public partial class MetadataController : Controller
//    {
//        private readonly static ILogger logger = LogManager.GetLogger();
//        private readonly UpdatableAppSettings settings;
//        private readonly IgdbApi igdbApi;
//        private static readonly char[] bracketsMatchList = new char[] { '[', ']', '(', ')', '{', '}' };
//        private static readonly char[] whereQueryBlacklist = new char[] { ':', '-' };
//        private readonly GamesController gamesController;
//        private readonly ExpandedGameController expandedController;
//        private readonly GameParsedController parsedController;

//        [GeneratedRegex(",\\s*(the|a|an|der|das|die)$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
//        private static partial Regex noIntroArticleRegex();

//        [GeneratedRegex("\\s*(:|-)\\s*", RegexOptions.Compiled)]
//        private static partial Regex separatorRegex();

//        [GeneratedRegex("\\s+(RHCP|RU)$", RegexOptions.IgnoreCase)]
//        private static partial Regex customRegionRegex();

//        [GeneratedRegex("\\s+and\\s+", RegexOptions.IgnoreCase)]
//        private static partial Regex andMatchRegex();

//        [GeneratedRegex("\\d+")]
//        private static partial Regex digitsRegex();

//        public MetadataController(UpdatableAppSettings settings, IgdbApi igdbApi)
//        {
//            this.settings = settings;
//            this.igdbApi = igdbApi;
//            gamesController = new GamesController(settings, igdbApi);
//            expandedController = new ExpandedGameController(igdbApi);
//            parsedController = new GameParsedController(igdbApi);
//        }

//        [HttpPost("metadata_v3")]
//        public async Task<ServicesResponse<ExpandedGame>> PostMetadataV3([FromBody] PlayniteGame game)
//        {
//            return await GetMetadata(game, expandedController.GetExpandedGame);
//        }

//        [HttpPost("metadata_v2")]
//        public async Task<ServicesResponse<ExpandedGame>> PostMetadataV2([FromBody] PlayniteGame_OldV2 game)
//        {
//            return await GetMetadata(game, expandedController.GetExpandedGame);
//        }

//        [HttpPost("metadata")]
//        public async Task<ServicesResponse<ExpandedGameLegacy>> PostMetadata([FromBody] PlayniteGame_OldV2 game)
//        {
//            return await GetMetadata(game, parsedController.GetExpandedGame);
//        }

//        private async Task<ServicesResponse<T>> GetMetadata<T>(PlayniteGame_OldV2 game, Func<ulong, Task<T>> expandFunc) where T : new()
//        {
//            var tempGame = new PlayniteGame
//            {
//                Name = game.Name,
//                PluginId = game.PluginId,
//                GameId = game.GameId
//            };

//            if (game.ReleaseDate != null)
//            {
//                tempGame.ReleaseDate = new SdkModels.ReleaseDate(game.ReleaseDate.Value);
//            }

//            return await GetMetadata(tempGame, expandFunc);
//        }

//        private async Task<ServicesResponse<T>> GetMetadata<T>(PlayniteGame game, Func<ulong, Task<T>> expandFunc) where T : new()
//        {
//            var isKnownPlugin = game.PluginId != Guid.Empty;
//            var isSteamPlugin = BuiltinExtensions.GetIdFromExtension(BuiltinExtension.SteamLibrary) == game.PluginId;
//            ulong igdbId = 0;
//            var matchId = $"{game.GameId}{game.PluginId}".MD5();
//            var searchId = $"{game.Name}{game.ReleaseDate?.Year}".MD5();

//            // Check if match was previously found
//            if (isKnownPlugin)
//            {
//                if (isSteamPlugin && ulong.TryParse(game.GameId, out var parsedId))
//                {
//                    igdbId = await igdbApi.GetSteamIgdbMatch(parsedId);
//                }
//                else
//                {
//                    var filter = Builders<GameIdMatch>.Filter.Eq(a => a.Id, matchId);
//                    var match = igdbApi.Database.IGBDGameIdMatches.Find(filter).FirstOrDefault();
//                    if (match != null)
//                    {
//                        igdbId = match.IgdbId;
//                    }
//                }
//            }

//            if (igdbId == 0)
//            {
//                var filter = Builders<SearchIdMatch>.Filter.Eq(a => a.Id, searchId);
//                var match = igdbApi.Database.IGDBSearchIdMatches.Find(filter).FirstOrDefault();
//                if (match != null)
//                {
//                    igdbId = match.IgdbId;
//                }
//            }

//            var foundMetadata = new T();
//            if (igdbId != 0)
//            {
//                return new ServicesResponse<T>(await expandFunc(igdbId));
//            }
//            else
//            {
//                igdbId = await TryMatchGame(game, false);
//                var useAlt = (settings.Settings.IGDB?.AlternativeSearch ?? false) && !game.Name.ContainsAny(whereQueryBlacklist);
//                if (useAlt && igdbId == 0)
//                {
//                    igdbId = await TryMatchGame(game, true);
//                }

//                if (igdbId != 0)
//                {
//                    foundMetadata = await expandFunc(igdbId);
//                }
//            }

//            // Update match database if match was found
//            if (igdbId != 0)
//            {
//                if (isKnownPlugin && !isSteamPlugin && !game.GameId.IsNullOrWhiteSpace())
//                {
//                    igdbApi.Database.IGBDGameIdMatches.ReplaceOne(
//                        Builders<GameIdMatch>.Filter.Eq(a => a.Id, matchId),
//                        new GameIdMatch
//                        {
//                            GameId = game.GameId,
//                            Id = matchId,
//                            IgdbId = igdbId,
//                            Library = game.PluginId
//                        },
//                        Database.ItemUpsertOptions);
//                }

//                igdbApi.Database.IGDBSearchIdMatches.ReplaceOne(
//                    Builders<SearchIdMatch>.Filter.Eq(a => a.Id, searchId),
//                    new SearchIdMatch
//                    {
//                        Term = game.Name!,
//                        Id = searchId,
//                        IgdbId = igdbId
//                    },
//                    Database.ItemUpsertOptions);
//            }

//            return new ServicesResponse<T>(foundMetadata);
//        }

//        private static string FixNointroNaming(string name)
//        {
//            var match = noIntroArticleRegex().Match(name);
//            if (match.Success)
//            {
//                return match.Groups[1].Value.Trim() + " " + noIntroArticleRegex().Replace(name, "");
//            }

//            return name;
//        }

//        private async Task<ulong> TryMatchGame(PlayniteGame game, bool alternativeSearch)
//        {
//            if (game.Name.IsNullOrEmpty())
//            {
//                return 0;
//            }

//            if (game.Name.ContainsAny(bracketsMatchList))
//            {
//                return 0;
//            }

//            ulong matchedGame = 0;
//            var copyGame = game.GetCopy();
//            copyGame.Name = StringExtensions.NormalizeGameName(game.Name);
//            copyGame.Name = FixNointroNaming(copyGame.Name);
//            copyGame.Name = copyGame.Name.Replace(@"\", string.Empty, StringComparison.Ordinal);
//            var name = copyGame.Name;
//            name = customRegionRegex().Replace(name, "");

//            var results = await gamesController.GetSearchResults(name, alternativeSearch);
//            results.ForEach(a => a.name = StringExtensions.NormalizeGameName(a.name!));
//            string testName = string.Empty;

//            // Direct comparison
//            matchedGame = MatchFun(game, name, results);
//            if (matchedGame > 0)
//            {
//                return matchedGame;
//            }

//            // Try replacing roman numerals: 3 => III
//            testName = digitsRegex().Replace(name, ReplaceNumsForRomans);
//            matchedGame = MatchFun(game, testName, results);
//            if (matchedGame > 0)
//            {
//                return matchedGame;
//            }

//            // Try adding The
//            testName = "The " + name;
//            matchedGame = MatchFun(game, testName, results);
//            if (matchedGame > 0)
//            {
//                return matchedGame;
//            }

//            // Try chaning & / and
//            testName = andMatchRegex().Replace(name, " & ");
//            matchedGame = MatchFun(game, testName, results);
//            if (matchedGame > 0)
//            {
//                return matchedGame;
//            }

//            // Try removing apostrophes
//            var resCopy = results.GetCopy();
//            resCopy.ForEach(a => a.name = a.name!.Replace("'", "", StringComparison.Ordinal));
//            matchedGame = MatchFun(game, name, resCopy);
//            if (matchedGame > 0)
//            {
//                return matchedGame;
//            }

//            // Try removing all ":" and "-"
//            testName = separatorRegex().Replace(name, " ");
//            resCopy = results.GetCopy();
//            foreach (var res in resCopy)
//            {
//                res.name = separatorRegex().Replace(res.name!, " ");
//                if (res.alternative_names.HasItems())
//                {
//                    res.alternative_names = res.alternative_names.Where(a => !a.name.IsNullOrEmpty()).ToList();
//                    res.alternative_names.ForEach(a => a.name = separatorRegex().Replace(a.name!, " "));
//                }
//            }

//            matchedGame = MatchFun(game, testName, resCopy);
//            if (matchedGame > 0)
//            {
//                return matchedGame;
//            }

//            // Try without subtitle
//            if (!alternativeSearch)
//            {
//                var testResult = results.OrderBy(a => a.first_release_date).FirstOrDefault(a =>
//                {
//                    if (a.first_release_date == 0)
//                    {
//                        return false;
//                    }

//                    if (!string.IsNullOrEmpty(a.name) && a.name.Contains(':', StringComparison.InvariantCultureIgnoreCase))
//                    {
//                        return string.Equals(name, a.name.Split(':')[0], StringComparison.InvariantCultureIgnoreCase);
//                    }

//                    return false;
//                });

//                if (testResult != null)
//                {
//                    return testResult.id;
//                }
//            }

//            return 0;
//        }

//        private ulong MatchFun(PlayniteGame game, string matchName, List<ExpandedGameLegacy> list)
//        {
//            var res = list.Where(a => string.Equals(matchName, a.name, StringComparison.InvariantCultureIgnoreCase));
//            if (!res.Any())
//            {
//                res = list.Where(a =>
//                a.alternative_names.HasItems() &&
//                a.alternative_names.Select(b => b.name ?? string.Empty).ContainsString(matchName) == true);
//            }

//            if (res.Any())
//            {
//                if (res.Count() == 1)
//                {
//                    return res.First().id;
//                }
//                else
//                {
//                    if (game.ReleaseDate != null)
//                    {
//                        var igdbGame = res.FirstOrDefault(a => a.first_release_date.ToDateFromUnixMs().Year == game.ReleaseDate.Value.Year);
//                        if (igdbGame != null)
//                        {
//                            return igdbGame.id;
//                        }
//                    }
//                    else
//                    {
//                        // If multiple matches are found and we don't have release date then prioritize older game
//                        if (res.All(a => a.first_release_date == 0))
//                        {
//                            return res.First().id;
//                        }
//                        else
//                        {
//                            var igdbGame = res.OrderBy(a => a.first_release_date).First(a => a.first_release_date > 0);
//                            return igdbGame.id;
//                        }
//                    }
//                }
//            }

//            return 0;
//        }

//        private string ReplaceNumsForRomans(Match m)
//        {
//            if (int.TryParse(m.Value, out var intVal))
//            {
//                return Roman.To(intVal);
//            }
//            else
//            {
//                return m.Value;
//            }
//        }
//    }
//}
