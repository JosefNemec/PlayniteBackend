using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using PlayniteServices.Controllers.IGDB.DataGetter;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB
{
    [Route("igdb/game")]
    public class GameController : Controller
    {
        private static ILogger logger = LogManager.GetLogger();
        private static readonly ReplaceOptions gameReplaceOptions = new ReplaceOptions { IsUpsert = true };
        private UpdatableAppSettings settings;
        private IgdbApi igdbApi;

        public GameController(UpdatableAppSettings settings, IgdbApi igdbApi)
        {
            this.settings = settings;
            this.igdbApi = igdbApi;
        }

        [ServiceFilter(typeof(PlayniteVersionFilter))]
        [HttpGet("{gameId}")]
        public async Task<ServicesResponse<Game>> Get(ulong gameId)
        {
            return await GetItem(gameId);
        }

        public async Task<ServicesResponse<Game>> GetItem(ulong gameId)
        {
            return new ServicesResponse<Game>(await igdbApi.Games.Get(gameId));
        }

        // Only use for IGDB webhook.
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            if (Request.Headers.TryGetValue("X-Secret", out var secret))
            {
                if (secret != settings.Settings.IGDB.WebHookSecret)
                {
                    logger.Error($"X-Secret doesn't match: {secret}");
                    return BadRequest();
                }

                try
                {
                    Game game = null;
                    string jsonString = null;
                    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        jsonString = await reader.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            game = Serialization.FromJson<Game>(jsonString);
                        }
                    }

                    if (game == null)
                    {
                        logger.Error("Failed IGDB content serialization.");
                        return Ok();
                    }

                    logger.Info($"Received game webhook from IGDB: {game.id}");
                    igdbApi.Games.Collection.ReplaceOne(
                        Builders<Game>.Filter.Eq(a => a.id, game.id),
                        game,
                        gameReplaceOptions);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to process IGDB webhook.");
                }

                return Ok();
            }
            else
            {
                logger.Error("Missing X-Secret from IGDB webhook.");
                return BadRequest();
            }
        }
    }

    [ServiceFilter(typeof(PlayniteVersionFilter))]
    [Route("igdb/game_parsed")]
    public class GameParsedController : Controller
    {
        private UpdatableAppSettings settings;
        private IgdbApi igdbApi;

        public GameParsedController(UpdatableAppSettings settings, IgdbApi igdbApi)
        {
            this.settings = settings;
            this.igdbApi = igdbApi;
        }

        [HttpGet("{gameId}")]
        public async Task<ServicesResponse<ExpandedGameLegacy>> Get(ulong gameId)
        {
            return new ServicesResponse<ExpandedGameLegacy>(await GetExpandedGame(gameId));
        }

        public async Task<ExpandedGameLegacy> GetExpandedGame(ulong gameId)
        {
            var game = await igdbApi.Games.Get(gameId);
            if (game.id == 0)
            {
                new ExpandedGameLegacy();
            }

            var parsedGame = new ExpandedGameLegacy()
            {
                id = game.id,
                name = game.name,
                slug = game.slug,
                url = game.url,
                summary = game.summary,
                storyline = game.storyline,
                popularity = game.popularity,
                version_title = game.version_title,
                category = game.category,
                first_release_date = game.first_release_date * 1000,
                rating = game.rating,
                aggregated_rating = game.aggregated_rating,
                total_rating = game.total_rating
            };

            parsedGame.alternative_names = await igdbApi.AlternativeNames.Get(game.alternative_names);
            parsedGame.involved_companies = await igdbApi.InvolvedCompanies.GetExpanded(game.involved_companies);
            parsedGame.genres_v3 = await igdbApi.Genres.Get(game.genres);
            parsedGame.websites = await igdbApi.Websites.Get(game.websites);
            parsedGame.game_modes_v3 = await igdbApi.GameModes.Get(game.game_modes);
            parsedGame.player_perspectives = await igdbApi.PlayerPerspectives.Get(game.player_perspectives);
            parsedGame.cover_v3 = await igdbApi.Covers.Get(game.cover);
            parsedGame.artworks = await igdbApi.Artworks.Get(game.artworks);
            parsedGame.screenshots = await igdbApi.Screenshots.Get(game.screenshots);

            // fallback properties for 4.x
            parsedGame.cover = parsedGame.cover_v3?.url;
            parsedGame.publishers = parsedGame.involved_companies?.Where(a => a.publisher == true).Select(a => a.company.name).ToList();
            parsedGame.developers = parsedGame.involved_companies?.Where(a => a.developer == true).Select(a => a.company.name).ToList();
            parsedGame.genres = parsedGame.genres_v3?.Select(a => a.name).ToList();
            parsedGame.game_modes = parsedGame.game_modes_v3?.Select(a => a.name).ToList();
            return parsedGame;
        }
    }
}
