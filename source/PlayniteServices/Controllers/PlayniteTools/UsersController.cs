using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace PlayniteServices.Playnite;

[Route("playnite/users")]
public class UsersController : Controller
{
    private readonly Database db;

    public UsersController(Database db)
    {
        this.db = db;
    }

    [HttpPost]
    public IActionResult Create([FromBody]User? user)
    {
        if (user == null)
        {
            return BadRequest(new ErrorResponse(new Exception("No user data provided.")));
        }

        user.LastLaunch = DateTime.Today;
        db.Users.ReplaceOne(
            Builders<User>.Filter.Eq(u => u.Id, user.Id),
            user,
            Database.ItemUpsertOptions);
        return Ok();
    }
}
