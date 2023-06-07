using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PlayniteServices.Playnite;

[ServiceFilter(typeof(ServiceKeyFilter))]
[Route("playnite/stats")]
public class StatsController : Controller
{
    private readonly Database db;

    public StatsController(Database db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<DataResponse<ServiceStats>> GetStats()
    {
        var now = DateTime.Now;
        var stats = new ServiceStats();
        await db.Users.Find(new BsonDocument()).ForEachAsync(user =>
        {
            stats.UserCount++;

            var activeUser = (now - user.LastLaunch).Days <= 7;
            if (activeUser)
            {
                var playniteVer = user.PlayniteVersion ?? "uknown";
                stats.LastWeekUserCount++;
                if (stats.UsersByVersion.TryGetValue(playniteVer, out var pC))
                {
                    stats.UsersByVersion[playniteVer] = pC + 1;
                }
                else
                {
                    stats.UsersByVersion.Add(playniteVer, 1);
                }

                var winVer = user.WinVersion ?? "uknown";
                if (stats.UsersByWinVersion.TryGetValue(winVer, out var wC))
                {
                    stats.UsersByWinVersion[winVer] = wC + 1;
                }
                else
                {
                    stats.UsersByWinVersion.Add(winVer, 1);
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

        return new DataResponse<ServiceStats>(stats);
    }

    [HttpPost("drop")]
    public async Task<ActionResult> DropStats()
    {
        await db.MongoDb.DropCollectionAsync("Users");
        return Ok();
    }
}
