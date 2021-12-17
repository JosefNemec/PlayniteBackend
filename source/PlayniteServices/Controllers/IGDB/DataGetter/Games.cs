using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class Games : DataGetter<Game>
    {
        public Games(IgdbApi igdbApi) : base(igdbApi, "games")
        {
            Collection.Indexes.CreateOne(new CreateIndexModel<Game>(Builders<Game>.IndexKeys.Text(x => x.name)));
        }

        public async Task<ExpandedGame> GetExpanded(ulong gameId)
        {
            var game = await Get(gameId);
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
                version_title = game.version_title,
                category = game.category,
                first_release_date = game.first_release_date * 1000,
                rating = game.rating,
                aggregated_rating = game.aggregated_rating,
                total_rating = game.total_rating,
                parent_game = game.parent_game
            };

            parsedGame.franchise = await igdbApi.Franchises.Get(game.franchise);
            parsedGame.collection = await igdbApi.Collections.Get(game.collection);
            parsedGame.involved_companies = await igdbApi.InvolvedCompanies.GetExpanded(game.involved_companies);
            parsedGame.genres = await igdbApi.Genres.Get(game.genres);
            parsedGame.themes = await igdbApi.Themes.Get(game.themes);
            parsedGame.game_modes = await igdbApi.GameModes.Get(game.game_modes);
            parsedGame.cover = await igdbApi.Covers.Get(game.cover);
            parsedGame.websites = await igdbApi.Websites.Get(game.websites);
            parsedGame.player_perspectives = await igdbApi.PlayerPerspectives.Get(game.player_perspectives);
            parsedGame.franchises = await igdbApi.Franchises.Get(game.franchises);
            parsedGame.alternative_names = await igdbApi.AlternativeNames.Get(game.alternative_names);
            parsedGame.screenshots = await igdbApi.Screenshots.Get(game.screenshots);
            parsedGame.artworks = await igdbApi.Artworks.Get(game.artworks);
            parsedGame.platforms = await igdbApi.Platforms.Get(game.platforms);
            parsedGame.release_dates = await igdbApi.ReleaseDates.Get(game.release_dates);
            parsedGame.age_ratings = await igdbApi.AgeRatings.Get(game.age_ratings);
            return parsedGame;
        }
    }
}
