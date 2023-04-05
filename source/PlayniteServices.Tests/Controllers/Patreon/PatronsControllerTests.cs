using PlayniteServices;
using PlayniteServices.Models.Patreon;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlayniteServices.Tests.Controllers.Patreon
{
    [Collection("DefaultCollection")]
    public class PatronsControllerTests
    {
        private readonly HttpClient client;

        public PatronsControllerTests(TestFixture<Startup> fixture)
        {
            client = fixture.Client;
        }

        [Fact]
        public async Task CompanyControllerTest()
        {
            var response = await client.GetAsync("patreon/patrons");
            var validResponse = DataSerialization.FromJson<DataResponse<List<string>>>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(validResponse);
            Assert.True(validResponse.Data.HasItems());
            Assert.True(validResponse.Error.IsNullOrEmpty());
        }
    }
}
