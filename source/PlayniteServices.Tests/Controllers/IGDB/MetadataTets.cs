using System.Net.Http;
using System.Net.Mime;
using Xunit;
using Playnite;
using PlayniteServices.Tests;

namespace PlayniteServices.IGDB.Tests;

[Collection("DefaultCollection")]
public class MetadataTets
{
    private readonly HttpClient client;

    public MetadataTets(TestFixture fixture)
    {
        client = fixture.Client;
    }

    private async Task<Game> GetMetadata(MetadataRequest request)
    {
        var str = Serialization.ToJson(request);
        var content = new StringContent(str, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(@"/igdb/metadata", content);
        var cntStr = await response.Content.ReadAsStringAsync();
        return Serialization.FromJson<DataResponse<Game>>(cntStr)?.Data ?? new Game();
    }

    [Fact]
    public async Task AlternateNameUseTest()
    {
        var metadata = await GetMetadata(new MetadataRequest("unreal 2"));
        Assert.Equal(6222ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Rally Championship 2000"));
        Assert.Equal(793ul, metadata.id);
    }

    [Fact]
    public async Task PlatformIdUsetTest()
    {
        // Steam
        var metadata = await GetMetadata(new MetadataRequest
        {
            LibraryId = new Guid("CB91DFC9-B977-43BF-8E70-55F46E410FAB"),
            GameId = "7200"
        });

        Assert.Equal(9908ul, metadata.id);

        // GOG
        metadata = await GetMetadata(new MetadataRequest
        {
            LibraryId = new Guid("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E"),
            GameId = "1207658692"
        });

        Assert.Equal(24723ul, metadata.id);
    }

    [Fact]
    public async Task ReleaseDateUseTest()
    {
        var game = new MetadataRequest("Tomb Raider") { ReleaseYear = 1996 };

        var metadata = await GetMetadata(game);
        Assert.Equal(1996, metadata.first_release_date.ToDateFromUnixSeconds().Year);
        Assert.Equal(912ul, metadata.id);

        game.ReleaseYear = 2013;
        metadata = await GetMetadata(game);
        Assert.Equal(2013, metadata.first_release_date.ToDateFromUnixSeconds().Year);
        Assert.Equal(1164ul, metadata.id);
    }

    [Fact]
    public async Task NameMatchingTest()
    {
        // No-Intro naming
        var metadata = await GetMetadata(new MetadataRequest("Bug's Life, A"));
        Assert.Equal(2847ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Warhammer 40,000: Space Marine"));
        Assert.Equal(578ul, metadata.id);

        // Diacritics test
        metadata = await GetMetadata(new MetadataRequest("Pokémon Red"));
        Assert.Equal(1561ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Pokemon Red"));
        Assert.Equal(1561ul, metadata.id);

        // Stylized name
        metadata = await GetMetadata(new MetadataRequest("fear"));
        Assert.Equal(517ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("F.E.A.R."));
        Assert.Equal(517ul, metadata.id);

        // & / and test
        metadata = await GetMetadata(new MetadataRequest("Command and Conquer"));
        Assert.Equal(647ul, metadata.id);
        Assert.Equal(1995, metadata.first_release_date.ToDateFromUnixSeconds().Year);

        // Matches exactly
        metadata = await GetMetadata(new MetadataRequest("Grand Theft Auto IV"));
        Assert.Equal(731ul, metadata.id);
        Assert.Equal(2008, metadata.first_release_date.ToDateFromUnixSeconds().Year);

        metadata = await GetMetadata(new MetadataRequest("Grand Theft Auto [sub 1](sub 2) { sub 3} IV"));
        Assert.Equal(731ul, metadata.id);
        Assert.Equal(2008, metadata.first_release_date.ToDateFromUnixSeconds().Year);

        // Roman numerals test
        metadata = await GetMetadata(new MetadataRequest("Quake 3 Arena"));
        Assert.Equal(355ul, metadata.id);

        // THE test
        metadata = await GetMetadata(new MetadataRequest("Witcher 3: Wild Hunt"));
        Assert.Equal(1942ul, metadata.id);

        // No subtitle test
        metadata = await GetMetadata(new MetadataRequest("The Witcher 3"));
        Assert.Equal(1942ul, metadata.id);

        // Apostrophe test
        metadata = await GetMetadata(new MetadataRequest("Assassins Creed II"));
        Assert.Equal(127ul, metadata.id);

        // Hyphen vs. colon test
        metadata = await GetMetadata(new MetadataRequest("Legacy of Kain - Soul Reaver 2"));
        Assert.Equal(7893ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Legacy of Kain: Soul Reaver 2"));
        Assert.Equal(7893ul, metadata.id);

        // Trademarks test
        metadata = await GetMetadata(new MetadataRequest("Dishonored®: Death of the Outsider™"));
        Assert.Equal(37030ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("X-COM: UFO Defense"));
        Assert.Equal(24ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Invisible Inc."));
        Assert.Equal(6044ul, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Invisible, Inc."));
        Assert.Equal(6044ul, metadata.id);
    }

    [Fact]
    public async Task GetGameTest()
    {
        var response = await client.GetAsync(@"/igdb/game/333");
        var cnt = await response.Content.ReadAsStringAsync();
        var game = Serialization.FromJson<DataResponse<Game>>(cnt)?.Data;
        Assert.Equal(333ul, game!.id);
    }

    [Fact]
    public async Task SearchTest()
    {
        var request = new SearchRequest { SearchTerm = "half-life" };
        var content = new StringContent(Serialization.ToJson(request), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(@"/igdb/search", content);
        var str = await response.Content.ReadAsStringAsync();
        var games =  Serialization.FromJson<DataResponse<List<Game>>>(str)?.Data ?? new ();
        Assert.DoesNotContain(games, a => a.category == GameCategoryEnum.MOD);
    }

    // [Fact]
    // public async Task BigMatchingTest()
    // {
    //     var gameList = System.IO.File.ReadAllLines(@"c:\devel\test.txt", Encoding.UTF8);
    //     var found = new StringBuilder();
    //     var missing = new StringBuilder();
    //
    //     foreach (var game in gameList)
    //     {
    //         if (game.IsNullOrEmpty())
    //         {
    //             continue;
    //         }
    //
    //         var metadata = await GetMetadata(new MetadataRequest(game));
    //         if (metadata?.id > 0)
    //         {
    //             found.AppendLine($"{game}#{metadata.id}#{metadata.name}");
    //         }
    //         else
    //         {
    //             missing.AppendLine(game);
    //         }
    //     }
    //
    //     var id = Guid.NewGuid();
    //     System.IO.File.WriteAllText($@"c:\devel\test_found_{id}.txt", found.ToString(), Encoding.UTF8);
    //     System.IO.File.WriteAllText($@"c:\devel\test_missing_{id}.txt", missing.ToString(), Encoding.UTF8);
    // }
}
