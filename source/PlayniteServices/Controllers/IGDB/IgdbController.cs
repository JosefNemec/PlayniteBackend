using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Playnite;
using System.Text.RegularExpressions;

namespace PlayniteServices.IGDB;

[Route("igdb")]
public partial class IgdbController : Controller
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private readonly IgdbManager igdbApi;

    private static readonly Dictionary<Guid, ExternalGameCategoryEnum> libraryIdCategories = new()
    {
        [new Guid("CB91DFC9-B977-43BF-8E70-55F46E410FAB")] = ExternalGameCategoryEnum.EXTERNALGAME_STEAM,
        [new Guid("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E")] = ExternalGameCategoryEnum.EXTERNALGAME_GOG,
        [new Guid("00000002-DBD1-46C6-B5D0-B1BA559D10E4")] = ExternalGameCategoryEnum.EXTERNALGAME_EPIC_GAME_STORE,
        [new Guid("00000001-EBB2-4EEC-ABCB-7C89937A42BB")] = ExternalGameCategoryEnum.EXTERNALGAME_ITCH_IO
    };

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"\[.+?\]|\(.+?\)|\{.+?\}")]
    private static partial Regex BracketsRegex();

    [GeneratedRegex(@"(.+),\s*(the|a|an|der|das|die)$", RegexOptions.IgnoreCase)]
    private static partial Regex ArticlesRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhiteSpacesRegex();

    [GeneratedRegex(@"\s*(:|-)\s*")]
    private static partial Regex SubtextSeparatorRegex();

    [GeneratedRegex(@"\s+and\s+", RegexOptions.IgnoreCase)]
    private static partial Regex AndRegex();

    public IgdbController(IgdbManager igdbApi)
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

        var game = await igdbApi.Games.GetItem(gameId, true);
        if (game == null)
        {
            return new ErrorResponse("Game not found.");
        }

        await game.expand_cover(igdbApi);
        await game.expand_artworks(igdbApi);
        await game.expand_screenshots(igdbApi);
        await game.expand_genres(igdbApi);
        await game.expand_websites(igdbApi);
        await game.expand_game_modes(igdbApi);
        await game.expand_age_ratings(igdbApi);
        await game.expand_collection(igdbApi);
        await game.expand_platforms(igdbApi);
        await game.expand_involved_companies(igdbApi);
        foreach (var company in game.involved_companies_expanded ?? Enumerable.Empty<InvolvedCompany>())
        {
            await company.expand_company(igdbApi);
        }

        return new DataResponse<Game>(game);
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

        var games = await SearchGame(searchRequest.SearchTerm, true);
        foreach (var game in games)
        {
            await game.Game.expand_cover(igdbApi);
            await game.Game.expand_involved_companies(igdbApi);
            foreach (var company in game.Game.involved_companies_expanded ?? Enumerable.Empty<InvolvedCompany>())
            {
                await company.expand_company(igdbApi);
            }
        }

        return new DataResponse<List<Game>>(games.Select(a => a.Game).ToList());
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
                var game = await igdbApi.Games.GetItem(externalGame.game);
                if (game == null)
                {
                    return new DataResponse<Game>(default);
                }

                await ExpandMetadatMatch(game);
                return new DataResponse<Game>(game);
            }
        }

        var match = await TryMatchGame(metadataRequest);
        if (match == null)
        {
            return new DataResponse<Game>(default);
        }

        await ExpandMetadatMatch(match);
        return new DataResponse<Game>(match);
    }

    private async Task ExpandMetadatMatch(Game game)
    {
        await game.expand_cover(igdbApi);
        await game.expand_artworks(igdbApi);
        await game.expand_screenshots(igdbApi);
        await game.expand_genres(igdbApi);
        await game.expand_websites(igdbApi);
        await game.expand_game_modes(igdbApi);
        await game.expand_age_ratings(igdbApi);
        await game.expand_collection(igdbApi);
        await game.expand_platforms(igdbApi);
        await game.expand_involved_companies(igdbApi);
        foreach (var company in game.involved_companies_expanded ?? Enumerable.Empty<InvolvedCompany>())
        {
            await company.expand_company(igdbApi);
        }
    }

    private async Task<List<TextSearchResult>> SearchGameByName(string searchTerm)
    {
        var serTerm = System.Text.Json.JsonSerializer.Serialize(searchTerm);
        var agg = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument[]>($$"""
            [
              {
                $match: {
                  category: {
                    $in: [0, 2, 3, 4, 8, 9, 10],
                  },
                  $text: {
                    $search: {{serTerm}},
                    $caseSensitive: false,
                    $diacriticSensitive: false,
                  },
                },
              },
              {
                $set: {
                  textScore: {
                    $meta: "textScore",
                  },
                },
              },
              {
                $match: {
                  textScore: { $gte: 0.7 }
                }
              },
              {
                $sort: {
                  score: {
                    $meta: "textScore",
                  },
                },
              },
              {
                $limit: 20,
              },
            ]
            """);

        var pipeline = PipelineDefinition<Game, Game>.Create(agg);
        var searchRes = await igdbApi.Games.collection.Aggregate(pipeline).ToListAsync();

        var res = new List<TextSearchResult>(20);
        foreach (var item in searchRes)
        {
            if (item.name.IsNullOrEmpty())
            {
                continue;
            }

            res.Add(new TextSearchResult(item.textScore, item.name, item));
        }

        return res;
    }

    private async Task<List<TextSearchResult>> SearchGameByAlternativeNames(string searchTerm)
    {
        var serTerm = System.Text.Json.JsonSerializer.Serialize(searchTerm);
        var agg = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument[]>($$"""
            [
              {
                $match: {
                  $text: {
                    $search: {{serTerm}},
                    $caseSensitive: false,
                    $diacriticSensitive: false,
                  },
                },
              },
              {
                $set: {
                  textScore: {
                    $meta: "textScore",
                  },
                },
              },
              {
                $match: {
              		textScore: { $gte: 0.9 }
                }
              },
              {
                $sort: {
                  score: {
                    $meta: "textScore",
                  },
                },
              },
              {
                $lookup: {
                  from: "IGDB_col_games",
                  localField: "game",
                  foreignField: "_id",
                  as: "game_ex",
                },
              },
              {
                $set: {
                  category: {
                    $arrayElemAt: ["$game_ex.category", 0],
                  },
                },
              },
              {
                $match: {
                  category: {
                    $in: [0, 2, 3, 4, 8, 9, 10],
                  },
                },
              },
              {
                $limit: 20,
              },
            ]
            """);

        var pipeline = PipelineDefinition<AlternativeName, AlternativeName>.Create(agg);
        var searchRes = await igdbApi.AlternativeNames.collection.Aggregate(pipeline).ToListAsync();

        var res = new List<TextSearchResult>(20);
        foreach (var item in searchRes)
        {
            if (item.name.IsNullOrEmpty())
            {
                continue;
            }

            await item.expand_game(igdbApi);
            if (item.game_expanded != null)
            {
                res.Add(new TextSearchResult(item.textScore, item.name, item.game_expanded));
            }
        }

        return res;
    }

    private async Task<List<TextSearchResult>> SearchGame(string searchTerm, bool removeDuplicates)
    {
        var nameResults = await SearchGameByName(searchTerm);
        var altResults = await SearchGameByAlternativeNames(searchTerm);
        var res = new List<TextSearchResult>(60);
        res.AddRange(nameResults);
        res.AddRange(altResults);
        res.Sort((a, b) => a.TextScore.CompareTo(b.TextScore) * -1);
        if (removeDuplicates)
        {
            res = res.DistinctBy(a => a.Game.id).ToList();
        }

        return res;
    }

    private async Task<Game?> TryMatchGame(MetadataRequest metadataRequest)
    {
        if (metadataRequest.Name.IsNullOrWhiteSpace())
        {
            return null;
        }

        var name = SanitizeName(metadataRequest.Name);

        var results = await SearchGame(name, false);
        results.ForEach(a => a.Name = SanitizeName(a.Name));

        // Direct comparison
        var matchedGame = TryMatchGames(metadataRequest, name, results);
        if (matchedGame != null)
        {
            return matchedGame;
        }

        // Try replacing roman numerals: 3 => III
        var testName = NumberRegex().Replace(name, ReplaceNumsForRomans);
        matchedGame = TryMatchGames(metadataRequest, testName, results);
        if (matchedGame != null)
        {
            return matchedGame;
        }

        // Try adding The
        testName = "The " + name;
        matchedGame = TryMatchGames(metadataRequest, testName, results);
        if (matchedGame != null)
        {
            return matchedGame;
        }

        // Try chaning & / and
        testName = AndRegex().Replace(name, " & ");
        matchedGame = TryMatchGames(metadataRequest, testName, results);
        if (matchedGame != null)
        {
            return matchedGame;
        }

        // Try removing apostrophes
        var resCopy = results.Select(a => new TextSearchResult(0, a.Name.Replace("'", "", StringComparison.Ordinal), a.Game)).ToList();
        matchedGame = TryMatchGames(metadataRequest, name, resCopy);
        if (matchedGame != null)
        {
            return matchedGame;
        }

        // Try removing all ":" and "-"
        testName = SubtextSeparatorRegex().Replace(name, " ");
        resCopy = results.Select(a => new TextSearchResult(0, SubtextSeparatorRegex().Replace(a.Name, " "), a.Game)).ToList();
        matchedGame = TryMatchGames(metadataRequest, testName, resCopy);
        if (matchedGame != null)
        {
            return matchedGame;
        }

        // Try without subtitle
        var testResult = results.FirstOrDefault(a =>
        {
            if (!string.IsNullOrEmpty(a.Name) && a.Name.Contains(':', StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Equals(name, a.Name.Split(':')[0], StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        });

        if (testResult != null)
        {
            return testResult.Game;
        }

        return null;
    }

    private Game? TryMatchGames(MetadataRequest metadataRequest, string matchName, List<TextSearchResult> list)
    {
        var res = list.Where(a => string.Equals(matchName, a.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();
        if (res.Count == 0)
        {
            return null;
        }

        if (res.Count == 1)
        {
            return res[0].Game;
        }

        if (res.Count > 1)
        {
            if (metadataRequest.ReleaseYear > 0)
            {
                var game = res.FirstOrDefault(a => a.Game.first_release_date.ToDateFromUnixSeconds().Year == metadataRequest.ReleaseYear);
                if (game != null)
                {
                    return game.Game;
                }
            }
            else
            {
                // If multiple matches are found and we don't have release date then prioritize older game
                if (res.All(a => a.Game.first_release_date == 0))
                {
                    return res[0].Game;
                }
                else
                {
                    var game = res.OrderBy(a => a.Game.first_release_date).FirstOrDefault(a => a.Game.first_release_date > 0);
                    if (game == null)
                    {
                        return res[0].Game;
                    }
                    else
                    {
                        return game.Game;
                    }
                }
            }
        }

        return null;
    }

    private static string SanitizeName(string name)
    {
        if (name.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        var match = ArticlesRegex().Match(name);
        if (match.Success)
        {
            name = match.Groups[2].Value + " " + match.Groups[1].Value;
        }

        name = BracketsRegex().Replace(name, string.Empty);
        name = name.RemoveTrademarks();
        name = name.Replace('_', ' ');
        name = name.Replace('.', ' ');
        name = name.Replace(',', ' ');
        name = name.Replace('’', '\'');
        name = WhiteSpacesRegex().Replace(name, " ");
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
}