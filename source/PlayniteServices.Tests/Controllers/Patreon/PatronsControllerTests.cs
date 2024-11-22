using System.Net.Http;
using Xunit;
using Playnite;
using Playnite.Backend.Tests;

namespace Playnite.Backend.Patreon.Tests;

[Collection("DefaultCollection")]
public class PatronsControllerTests
{
    private readonly HttpClient client;

    public PatronsControllerTests(TestFixture fixture)
    {
        client = fixture.Client;
    }

    [Fact]
    public async Task CompanyControllerTest()
    {
        var response = await client.GetAsync("patreon/patrons");
        var validResponse = Serialization.FromJson<DataResponse<List<string>>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(validResponse);
        Assert.True(validResponse.Data.HasItems());
        Assert.True(validResponse.Error.IsNullOrEmpty());
    }
}
