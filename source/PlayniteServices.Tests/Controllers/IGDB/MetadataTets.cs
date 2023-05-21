using PlayniteServices.Controllers.IGDB;
using System.Net.Http;
using System.Net.Mime;
using Xunit;

namespace PlayniteServices.Tests.IGDB;

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
        var str = DataSerialization.ToJson(request);
        var content = new StringContent(str, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(@"/igdb/metadata", content);
        return DataSerialization.FromJson<DataResponse<Game>>(await response.Content.ReadAsStringAsync())?.Data ?? new Game();
    }

    [Fact]
    public async Task AlternateNameUseTest()
    {
        var metadata = await GetMetadata(new MetadataRequest("unreal 2"));
        Assert.Equal("Unreal II: The Awakening", metadata.name);

        metadata = await GetMetadata(new MetadataRequest("Rally Championship 2000"));
        Assert.Equal("Mobil 1 Rally Championship", metadata.name);
    }

    [Fact]
    public async Task SteamIdUseTest()
    {
        // Steam
        var metadata = await GetMetadata(new MetadataRequest
        {
            LibraryId = new Guid("CB91DFC9-B977-43BF-8E70-55F46E410FAB"),
            GameId = "7200"
        });

        Assert.Equal("TrackMania United", metadata.name);

        // GOG
        metadata = await GetMetadata(new MetadataRequest
        {
            LibraryId = new Guid("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E"),
            GameId = "1207658692"
        });

        Assert.Equal("Unreal Tournament: Game of the Year Edition", metadata.name);
    }

    [Fact]
    public async Task ReleaseDateUseTest()
    {
        var game = new MetadataRequest("Tomb Raider") { ReleaseYear = 1996 };

        var metadata = await GetMetadata(game);
        Assert.Equal(1996, metadata.first_release_date.ToDateFromUnixSeconds().Year);
        Assert.Equal((ulong)912, metadata.id);

        game.ReleaseYear = 2013;
        metadata = await GetMetadata(game);
        Assert.Equal(2013, metadata.first_release_date.ToDateFromUnixSeconds().Year);
        Assert.Equal((ulong)1164, metadata.id);
    }

    [Fact]
    public async Task NameMatchingTest()
    {
        // No-Intro naming
        var metadata = await GetMetadata(new MetadataRequest("Bug's Life, A"));
        Assert.Equal((ulong)2847, metadata.id);

        metadata = await GetMetadata(new MetadataRequest("Warhammer 40,000: Space Marine"));
        Assert.Equal((ulong)578, metadata.id);

        // & / and test
        metadata = await GetMetadata(new MetadataRequest("Command and Conquer"));
        Assert.Equal("Command & Conquer", metadata.name);
        Assert.Equal(1995, metadata.first_release_date.ToDateFromUnixSeconds().Year);

        // Matches exactly
        metadata = await GetMetadata(new MetadataRequest("Grand Theft Auto IV"));
        Assert.Equal("Grand Theft Auto IV", metadata.name);
        Assert.Equal(2008, metadata.first_release_date.ToDateFromUnixSeconds().Year);

        metadata = await GetMetadata(new MetadataRequest("Grand Theft Auto [sub 1](sub 2) { sub 3} IV"));
        Assert.Equal("Grand Theft Auto IV", metadata.name);
        Assert.Equal(2008, metadata.first_release_date.ToDateFromUnixSeconds().Year);

        // Roman numerals test
        metadata = await GetMetadata(new MetadataRequest("Quake 3 Arena"));
        Assert.Equal("Quake III Arena", metadata.name);

        // THE test
        metadata = await GetMetadata(new MetadataRequest("Witcher 3: Wild Hunt"));
        Assert.Equal("The Witcher 3: Wild Hunt", metadata.name);

        // No subtitle test
        metadata = await GetMetadata(new MetadataRequest("The Witcher 3"));
        Assert.Equal("The Witcher 3: Wild Hunt", metadata.name);

        // Apostrophe test
        metadata = await GetMetadata(new MetadataRequest("Dragons Lair"));
        Assert.Equal("Dragon's Lair", metadata.name);

        // Hyphen vs. colon test
        metadata = await GetMetadata(new MetadataRequest("Legacy of Kain - Soul Reaver 2"));
        Assert.Equal("Legacy of Kain: Soul Reaver 2", metadata.name);

        metadata = await GetMetadata(new MetadataRequest("Legacy of Kain: Soul Reaver 2"));
        Assert.Equal("Legacy of Kain: Soul Reaver 2", metadata.name);

        // Trademarks test
        metadata = await GetMetadata(new MetadataRequest("Dishonored®: Death of the Outsider™"));
        Assert.Equal("Dishonored: Death of the Outsider", metadata.name);

        metadata = await GetMetadata(new MetadataRequest("X-COM: UFO Defense"));
        Assert.Equal("X-COM: UFO Defense", metadata.name);
    }

    //[Fact]
    //public async Task BigMatchingTest()
    //{
    //    var resultPath = @"d:\Downloads\download_" + Guid.NewGuid() + ".txt";
    //    var gameList = File.ReadAllLines(@"d:\Downloads\export.csv");
    //    var results = new List<string>();
    //    foreach (var game in gameList)
    //    {
    //        if (game.IsNullOrEmpty())
    //        {
    //            continue;
    //        }

    //        var metadata = await GetMetadata(new PlayniteGame(game));
    //        File.AppendAllText(resultPath, $"{game}#{metadata.id}#{metadata.name}" + Environment.NewLine);
    //    }
    //}

    [Fact]
    public async Task GetGameTest()
    { 
        var response = await client.GetAsync(@"/igdb/game/333");
        var cnt = await response.Content.ReadAsStringAsync();
        var game = DataSerialization.FromJson<DataResponse<Game>>(cnt)?.Data;
        Assert.Equal(333d, game!.id);
        Assert.Equal("Quake", game!.name);
    }
}
