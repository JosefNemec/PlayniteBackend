using PlayniteServices;
using PlayniteServices.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlayniteServices.Tests.Controllers.Playnite
{
    [Collection("DefaultCollection")]
    public class UsersControllerTests
    {
        private readonly HttpClient client;

        public UsersControllerTests(TestFixture fixture)
        {
            client = fixture.Client;
        }

        [Fact]
        public async Task PostUserTest()
        {
            var user = new User()
            {
                Id = "testId",
                WinVersion = "windversion",
                PlayniteVersion = "1.0"
            };

            var content = new StringContent(DataSerialization.ToJson(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(@"/playnite/users", content);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            // TODO add db check
        }
    }
}
