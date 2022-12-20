using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using PlayniteServices.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using PlayniteServices;
using PlayniteServices.Models.Steam;

namespace PlayniteServices.Tests.Controllers.Steam
{
    [Collection("DefaultCollection")]
    public class LibraryControllerTests
    {
        private readonly HttpClient client;

        public LibraryControllerTests(TestFixture<Startup> fixture)
        {
            client = fixture.Client;
        }

        [Fact]
        public async Task GetLibraryTest()
        {
            var response = await client.GetAsync("/steam/library/123456789");
            var errorResponse = DataSerialization.FromJson<GenericResponse>(await response.Content.ReadAsStringAsync());
            Assert.True(errorResponse?.Error.IsNullOrEmpty() == false);
            Assert.Null(errorResponse?.Data);

            response = await client.GetAsync("/steam/library/76561198358889790");
            var validResponse = DataSerialization.FromJson<ServicesResponse<List<GetOwnedGamesResult.Game>>>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(validResponse);
            Assert.True(validResponse.Error.IsNullOrEmpty());
            Assert.True(validResponse.Data?.Count > 0);
        }
    }
}
