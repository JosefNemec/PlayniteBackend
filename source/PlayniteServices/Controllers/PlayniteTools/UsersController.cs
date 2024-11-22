using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Playnite.Backend.Playnite;

[Route("playnite/users")]
public class UsersController : Controller
{
    private readonly Database db;

    public UsersController(Database db)
    {
        this.db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]User? user)
    {
        if (user == null)
        {
            return BadRequest(new ErrorResponse(new Exception("No user data provided.")));
        }

        user.LastLaunch = DateTime.Today;
        await db.Users.ReplaceOneAsync(
            Builders<User>.Filter.Eq(u => u.Id, user.Id),
            user,
            Database.ItemUpsertOptions);
        return Ok();
    }
}
