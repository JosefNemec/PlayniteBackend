using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlayniteServices.Filters;
using PlayniteServices.Models.IGDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.IGDB.DataGetter
{
    public class ExternalGames : DataGetter<ExternalGame>
    {
        public ExternalGames(IgdbApi igdbApi) : base(igdbApi, "external_games")
        {
            Collection.Indexes.CreateMany(new []
            {
                new CreateIndexModel<ExternalGame>(Builders<ExternalGame>.IndexKeys.Ascending(a => a.uid)),
                new CreateIndexModel<ExternalGame>(Builders<ExternalGame>.IndexKeys.Ascending(a => a.category))
            });
        }

        public ExternalGame GetExternalGame(ExternalGameCategory category, string gameId)
        {
            var filter = Builders<ExternalGame>.Filter;
            return Collection.Find(filter.Eq(a => a.uid, gameId) & filter.Eq(a => a.category, category)).FirstOrDefault();
        }
    }
}
