using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PlayniteServices.Controllers.PlayniteTools
{
    [Route("playnite/users")]
    public class UsersController : Controller
    {
        private readonly Database db;

        public UsersController(Database db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Create([FromBody]Models.User? user)
        {
            if (user == null)
            {
                return BadRequest(new ErrorResponse(new Exception("No user data provided.")));
            }

            user.LastLaunch = DateTime.Today;
            db.Users.ReplaceOne(
                Builders<Models.User>.Filter.Eq(u => u.Id, user.Id),
                user,
                Database.ItemUpsertOptions);
            return Ok();
        }
    }
}
