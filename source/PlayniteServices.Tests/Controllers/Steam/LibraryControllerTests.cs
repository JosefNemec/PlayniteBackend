using Xunit;
using System.Net.Http;
using Playnite;
using Playnite.Backend.Tests;

namespace Playnite.Backend.Steam.Tests;

[Collection("DefaultCollection")]
public class LibraryControllerTests
{
    private readonly HttpClient client;

    public LibraryControllerTests(TestFixture fixture)
    {
        client = fixture.Client;
    }

    [Fact]
    public async Task GetLibraryTest()
    {
        var response = await client.GetAsync("/steam/library/123456789");
        var errorResponse = Serialization.FromJson<DataResponse<List<GetOwnedGamesResult.Game>>>(await response.Content.ReadAsStringAsync());
        Assert.True(errorResponse?.Error.IsNullOrEmpty() == false);
        Assert.Null(errorResponse?.Data);

        response = await client.GetAsync("/steam/library/76561198358889790");
        var validResponse = Serialization.FromJson<DataResponse<List<GetOwnedGamesResult.Game>>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(validResponse);
        Assert.True(validResponse.Error.IsNullOrEmpty());
        Assert.True(validResponse.Data?.Count > 0);
    }
}
