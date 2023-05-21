using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Playnite.Common;
using Playnite.SDK;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace PlayniteServices.Controllers.IGDB;

[Route("igdb")]
public class IgdbController : Controller
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private readonly IgdbApi igdbApi;

    private static readonly List<GameCategoryEnum> defaultSearchCategories = new()
    {
        GameCategoryEnum.MAIN_GAME,
        GameCategoryEnum.REMAKE,
        GameCategoryEnum.REMASTER,
        GameCategoryEnum.STANDALONE_EXPANSION
    };

    private static readonly Dictionary<Guid, ExternalGameCategoryEnum> libraryIdCategories = new()
    {
        [new Guid("CB91DFC9-B977-43BF-8E70-55F46E410FAB")] = ExternalGameCategoryEnum.EXTERNALGAME_STEAM,
        [new Guid("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E")] = ExternalGameCategoryEnum.EXTERNALGAME_GOG,
        [new Guid("00000002-DBD1-46C6-B5D0-B1BA559D10E4")] = ExternalGameCategoryEnum.EXTERNALGAME_EPIC_GAME_STORE,
        [new Guid("00000001-EBB2-4EEC-ABCB-7C89937A42BB")] = ExternalGameCategoryEnum.EXTERNALGAME_ITCH_IO
    };

    private static readonly TextSearchOptions gameSearchOptons = new()
    {
        CaseSensitive = false,
        DiacriticSensitive = false
    };

    public IgdbController(IgdbApi igdbApi)
    {
        this.igdbApi = igdbApi;
    }

    [HttpGet("game/{gameId}")]
    public async Task<ResponseBase> GetGame(ulong gameId)
    {
        if (gameId == 0)
        {
            return new ErrorResponse("No ID specified.");
        }

        var game = await igdbApi.Games.GetItem(gameId);
        if (game != null)
        {
            return new DataResponse<Game>(game);
        }

        return new ErrorResponse("Game not found.");
    }


    [HttpPost("search")]
    public async Task<ResponseBase> SearchGames([FromBody] SearchRequest? searchRequest)
    {
        if (searchRequest is null)
        {
            return new ErrorResponse("Missing search data.");
        }

        if (searchRequest.SearchTerm.IsNullOrWhiteSpace())
        {
            return new ErrorResponse("No search term");
        }

        var games = await SearchGame(searchRequest.SearchTerm);
        return new DataResponse<List<Game>>(games);
    }

    [HttpPost("metadata")]
    public async Task<ResponseBase> GetMetadata([FromBody] MetadataRequest? metadataRequest)
    {
        if (metadataRequest is null)
        {
            return new ErrorResponse("Missing metadata data.");
        }

        if (metadataRequest.LibraryId != Guid.Empty &&
            !metadataRequest.GameId.IsNullOrWhiteSpace() &&
            libraryIdCategories.TryGetValue(metadataRequest.LibraryId, out var externalCategory))
        {
            var filter = Builders<ExternalGame>.Filter;
            var externalGame = await igdbApi.ExternalGames.collection.
                Find(filter.Eq(a => a.uid, metadataRequest.GameId) & filter.Eq(a => a.category, externalCategory)).
                FirstOrDefaultAsync();
            if (externalGame != null)
            {
                return new DataResponse<Game>(await igdbApi.Games.GetItem(externalGame.game));
            }
        }

        var match = await TryMatchGame(metadataRequest);
        if (match == 0)
        {
            return new DataResponse<Game>(default);
        }
        else
        {
            return new DataResponse<Game>(await igdbApi.Games.GetItem(match));
        }
    }

    private async Task<List<Game>> SearchGame(string searchTerm)
    {
        var filter = Builders<Game>.Filter;
        var catFilter = filter.In(a => a.category, defaultSearchCategories);
        var nameFilter = filter.Text(searchTerm, gameSearchOptons);
        return await igdbApi.Games.collection.
                Find(catFilter & nameFilter).
                //Project<Game>(Builders<Game>.Projection.MetaTextScore("textScore")).
                Sort(Builders<Game>.Sort.MetaTextScore("textScore")).
                Limit(50).
                ToListAsync();
    }

    private async Task<List<AlternativeName>> SearchGameAlternativeNames(string searchTerm)
    {
        var filter = Builders<AlternativeName>.Filter;
        return await igdbApi.AlternativeNames.collection.
                Find(filter.Text(searchTerm, gameSearchOptons)).
                //Project<AlternativeName>(Builders<AlternativeName>.Projection.MetaTextScore("textScore")).
                Sort(Builders<AlternativeName>.Sort.MetaTextScore("textScore")).
                Limit(50).
                ToListAsync();
    }

    private async Task<ulong> TryMatchGame(MetadataRequest metadataRequest)
    {
        if (metadataRequest.Name.IsNullOrWhiteSpace())
        {
            return 0;
        }

        var name = SanitizeName(metadataRequest.Name);

        var results = await SearchGame(name);
        results.ForEach(a => a.name = SanitizeName(a.name ?? string.Empty));

        var altResults = await SearchGameAlternativeNames(name);
        altResults.ForEach(a => a.name = SanitizeName(a.name ?? string.Empty));

        // TODO: All these below should return game objects not IDs

        // Direct comparison
        var matchedGame = MatchFun(metadataRequest, name, results, altResults);
        if (matchedGame > 0)
        {
            return matchedGame;
        }

        // Try replacing roman numerals: 3 => III
        var testName = Regex.Replace(name, @"\d+", ReplaceNumsForRomans);
        matchedGame = MatchFun(metadataRequest, testName, results, altResults);
        if (matchedGame > 0)
        {
            return matchedGame;
        }

        // Try adding The
        testName = "The " + name;
        matchedGame = MatchFun(metadataRequest, testName, results, altResults);
        if (matchedGame > 0)
        {
            return matchedGame;
        }

        // Try chaning & / and
        testName = Regex.Replace(name, @"\s+and\s+", " & ");
        matchedGame = MatchFun(metadataRequest, testName, results, altResults);
        if (matchedGame > 0)
        {
            return matchedGame;
        }

        // Try removing apostrophes
        var resCopy = results.GetCopy();
        resCopy.ForEach(a => a.name = a.name!.Replace("'", "", StringComparison.Ordinal));
        matchedGame = MatchFun(metadataRequest, name, resCopy, altResults);
        if (matchedGame > 0)
        {
            return matchedGame;
        }

        // Try removing all ":" and "-"
        testName = Regex.Replace(name, @"\s*(:|-)\s*", " ");
        resCopy = results.GetCopy();
        resCopy.ForEach(a => a.name = Regex.Replace(a.name!, @"\s*(:|-)\s*", " "));
        matchedGame = MatchFun(metadataRequest, testName, resCopy, altResults);
        if (matchedGame > 0)
        {
            return matchedGame;
        }

        // Try without subtitle
        var testResult = results.FirstOrDefault(a =>
        {
            if (!string.IsNullOrEmpty(a.name) && a.name.Contains(':', StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Equals(name, a.name.Split(':')[0], StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        });

        if (testResult != null)
        {
            return testResult.id;
        }

        return 0;
    }

    private ulong MatchFun(MetadataRequest metadataRequest, string matchName, List<Game> list, List<AlternativeName> altList)
    {
        var res = list.Where(a => string.Equals(matchName, a.name, StringComparison.InvariantCultureIgnoreCase)).ToList();
        if (res.Count == 1)
        {
            return res[0].id;
        }
        else if (res.Count > 1)
        {
            if (metadataRequest.ReleaseYear > 0)
            {
                var game = res.FirstOrDefault(a => a.first_release_date.ToDateFromUnixSeconds().Year == metadataRequest.ReleaseYear);
                if (game != null)
                {
                    return game.id;
                }
            }
            else
            {
                // If multiple matches are found and we don't have release date then prioritize older game
                if (res.All(a => a.first_release_date == 0))
                {
                    return res[0].id;
                }
                else
                {
                    var game = res.OrderBy(a => a.first_release_date).FirstOrDefault(a => a.first_release_date > 0);
                    if (game == null)
                    {
                        return res[0].id;
                    }
                    else
                    {
                        return game.id;
                    }
                }
            }
        }

        var altRes = altList.Where(a => string.Equals(matchName, a.name, StringComparison.InvariantCultureIgnoreCase)).ToList();
        if (altRes.Count > 0)
        {
            return altRes[0].game;
        }

        return 0;
    }

    private static string SanitizeName(string name)
    {
        if (name.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        var match = Regex.Match(name, @"(.+),\s*(the|a|an|der|das|die)$", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            name = match.Groups[2].Value + " " + match.Groups[1].Value;
        }

        name = Regex.Replace(name, @"\[.+?\]|\(.+?\)|\{.+?\}", string.Empty);
        name = name.RemoveTrademarks();
        name = name.Replace('_', ' ');
        name = name.Replace('.', ' ');
        name = name.Replace('’', '\'');
        name = Regex.Replace(name, @"\s+", " ");
        name = name.Replace(@"\", string.Empty, StringComparison.Ordinal);
        return name.Trim();
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

    //private static string GetSearchString(string gameName)
    //{
    //    return gameName.
    //        Replace(":", " ", StringComparison.InvariantCultureIgnoreCase).
    //        Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase).
    //        Trim();
    //}
}
