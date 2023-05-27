using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Playnite;
using RateLimiter;
using ComposableAsync;

namespace PlayniteServices.Steam;

[ServiceFilter(typeof(PlayniteVersionFilter))]
[Route("steam/library")]
public class LibraryController : Controller
{
    private static readonly HttpClient httpClient;

    private readonly UpdatableAppSettings settings;

    static LibraryController()
    {
        var requestLimiterHandler = TimeLimiter.
            GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(1)).
            AsDelegatingHandler();
        httpClient = new HttpClient(requestLimiterHandler);
    }

    public LibraryController(UpdatableAppSettings settings)
    {
        this.settings = settings;
    }

    [HttpGet("{steamId}")]
    public async Task<DataResponse<List<GetOwnedGamesResult.Game>>> Get(ulong steamId, [FromQuery]bool freeSub)
    {
        if (settings.Settings.Steam?.ApiKey.IsNullOrEmpty() == true)
        {
            return new DataResponse<List<GetOwnedGamesResult.Game>>(new List<GetOwnedGamesResult.Game>());
        }

        var libraryUrl = string.Format(
            @"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&include_appinfo=1&format=json&steamid={1}&include_played_free_games=1&skip_unvetted_apps=0",
            settings.Settings.Steam!.ApiKey, steamId);
        if (freeSub)
        {
            libraryUrl += "&include_free_sub=1";
        }

        var libraryStringResult = await httpClient.GetStringAsync(libraryUrl);
        var libraryResult = Serialization.FromJson<GetOwnedGamesResult>(libraryStringResult);
        return new DataResponse<List<GetOwnedGamesResult.Game>>(libraryResult?.response?.games ?? new List<GetOwnedGamesResult.Game>());
    }
}
