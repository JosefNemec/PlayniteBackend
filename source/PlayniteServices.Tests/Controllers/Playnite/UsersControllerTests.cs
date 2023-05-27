using System.Net.Http;
using Xunit;
using Playnite;
using PlayniteServices.Tests;

namespace PlayniteServices.Playnite.Tests;

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

        var content = new StringContent(Serialization.ToJson(user), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(@"/playnite/users", content);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        // TODO add db check
    }
}
