using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Playnite.SDK;

namespace PlayniteServices.Controllers.IGDB;

[Route("igdb")]
public class IgdbController : Controller
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private readonly IgdbApi igdbApi;

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
        if (gameId > 0)
        {
            var game = await igdbApi.Games.GetItem(gameId);
            if (game != null)
            {
                return new DataResponse<Game>(game);
            }
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

        var games = await igdbApi.Games.collection.
                Find(Builders<Game>.Filter.Text(searchRequest!.SearchTerm!, gameSearchOptons)).
                Project<Game>(Builders<Game>.Projection.MetaTextScore("textScore")).
                Sort(Builders<Game>.Sort.MetaTextScore("textScore")).
                Limit(50).
                ToListAsync();

        return new DataResponse<List<Game>>(games);
    }

    [HttpPost("metadata")]
    public async Task<ResponseBase> GetMetadata([FromBody] MetadataRequest? metadataRequest)
    {
        if (metadataRequest is null)
        {
            return new ErrorResponse("Missing metadata data.");
        }

        return new DataResponse<Game>(default);
    }
}
