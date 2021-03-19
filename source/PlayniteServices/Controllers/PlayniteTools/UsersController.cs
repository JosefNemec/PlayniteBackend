using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using PlayniteServices.Databases;

namespace PlayniteServices.Controllers.PlayniteTools
{
    [Route("playnite/users")]
    public class UsersController : Controller
    {
        private static readonly ReplaceOptions userReplaceOptions = new ReplaceOptions { IsUpsert = true };

        [HttpPost]
        public IActionResult Create([FromBody]Models.User user)
        {
            if (user == null)
            {
                return BadRequest(new ErrorResponse(new Exception("No user data provided.")));
            }

            user.LastLaunch = DateTime.Today;
            Database.Instance.Users.ReplaceOne(
                Builders<Models.User>.Filter.Eq(u => u.Id, user.Id),
                user,
                userReplaceOptions);
            return Ok();
        }
    }
}
