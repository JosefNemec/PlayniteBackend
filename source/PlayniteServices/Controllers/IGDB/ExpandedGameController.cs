using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlayniteServices.Controllers.IGDB.DataGetter;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB
{
    [ServiceFilter(typeof(PlayniteVersionFilter))]
    [Route("igdb/game_parsed_v2")]
    public class ExpandedGameController : Controller
    {
        private UpdatableAppSettings settings;
        private IgdbApi igdbApi;

        public ExpandedGameController(UpdatableAppSettings settings, IgdbApi igdbApi)
        {
            this.settings = settings;
            this.igdbApi = igdbApi;
        }

        [HttpGet("{gameId}")]
        public async Task<ServicesResponse<ExpandedGame>> Get(ulong gameId)
        {
            return new ServicesResponse<ExpandedGame>(await GetExpandedGame(gameId));
        }

        public async Task<ExpandedGame> GetExpandedGame(ulong gameId)
        {
            var game = await igdbApi.Games.Get(gameId);
            if (game.id == 0)
            {
                new ExpandedGame();
            }

            var parsedGame = new ExpandedGame()
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
            parsedGame.genres = await igdbApi.Genres.Get(game.genres);
            parsedGame.websites = await igdbApi.Websites.Get(game.websites);
            parsedGame.game_modes = await igdbApi.GameModes.Get(game.game_modes);
            parsedGame.player_perspectives = await igdbApi.PlayerPerspectives.Get(game.player_perspectives);
            parsedGame.cover = await igdbApi.Covers.Get(game.cover);
            parsedGame.artworks = await igdbApi.Artworks.Get(game.artworks);
            parsedGame.screenshots = await igdbApi.Screenshots.Get(game.screenshots);
            parsedGame.age_ratings = await igdbApi.AgeRatings.Get(game.age_ratings);
            parsedGame.collection = await igdbApi.Collections.Get(game.collection);
            parsedGame.platforms = await igdbApi.Platforms.Get(game.platforms);
            return parsedGame;
        }
    }
}
