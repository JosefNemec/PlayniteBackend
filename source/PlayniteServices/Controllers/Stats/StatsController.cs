using Microsoft.AspNetCore.Mvc;
using PlayniteServices.Models;
using PlayniteServices.Filters;
using PlayniteServices.Models.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PlayniteServices.Controllers.Stats
{
    [ServiceFilter(typeof(ServiceKeyFilter))]
    public class StatsController : Controller
    {
        private readonly Database db;

        public StatsController(Database db)
        {
            this.db = db;
        }

        [HttpGet("stats")]
        public async Task<GenericResponse> GetStarts()
        {
            var now = DateTime.Now;
            var stats = new ServiceStats();
            await db.Users.Find(new BsonDocument()).ForEachAsync(user =>
            {
                stats.UserCount++;

                var activeUser = (now - user.LastLaunch).Days <= 7;
                if (activeUser)
                {
                    stats.LastWeekUserCount++;
                    if (stats.UsersByVersion.TryGetValue(user.PlayniteVersion, out var pC))
                    {
                        stats.UsersByVersion[user.PlayniteVersion] = pC + 1;
                    }
                    else
                    {
                        stats.UsersByVersion.Add(user.PlayniteVersion, 1);
                    }

                    if (stats.UsersByWinVersion.TryGetValue(user.WinVersion, out var wC))
                    {
                        stats.UsersByWinVersion[user.WinVersion] = wC + 1;
                    }
                    else
                    {
                        stats.UsersByWinVersion.Add(user.WinVersion, 1);
                    }

                    if (user.Is64Bit)
                    {
                        stats.X64Count++;
                    }
                    else
                    {
                        stats.X86Count++;
                    }
                }
            });

            return new ServicesResponse<ServiceStats>(stats);
        }
    }
}
