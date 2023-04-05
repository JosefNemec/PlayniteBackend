using Playnite.Common;
using Playnite.SDK;
using PlayniteServices;
using PlayniteServices.Controllers.IGDB;
using PlayniteServices.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SdkModels = Playnite.SDK.Models;

namespace PlayniteServices.Tests.Controllers.IGDB
{
    [Collection("DefaultCollection")]
    public class MetadataTets
    {
        private readonly HttpClient client;

        public MetadataTets(TestFixture<Startup> fixture)
        {
            client = fixture.Client;
        }

        //private async Task<ExpandedGame> GetMetadata(PlayniteGame game)
        //{
        //    var str = DataSerialization.ToJson(game);
        //    var content = new StringContent(str, Encoding.UTF8, MediaTypeNames.Application.Json);
        //    var response = await client.PostAsync(@"/igdb/metadata_v3", content);
        //    return DataSerialization.FromJson<ServicesResponse<ExpandedGame>>(await response.Content.ReadAsStringAsync())?.Data ?? new ExpandedGame();
        //}

        //private int GetYearFromUnix(long date)
        //{
        //    return DateTimeOffset.FromUnixTimeMilliseconds(date).DateTime.Year;
        //}

        //[Fact]
        //public async Task AlternateNameUseTest()
        //{
        //    var metadata = await GetMetadata(new PlayniteGame("pubg"));
        //    Assert.Equal("PUBG: BATTLEGROUNDS", metadata.name);

        //    metadata = await GetMetadata(new PlayniteGame("unreal 2"));
        //    Assert.Equal("Unreal II: The Awakening", metadata.name);
        //}

        //[Fact]
        //public void ReleaseDateLegacyDeserializationTest()
        //{
        //    var game = DataSerialization.FromJson<PlayniteGame>("""
        //        {"Name":"Chavez II","ReleaseDate":"1993","GameId":"61810"}
        //        """);
        //    Assert.Equal(1993, game!.ReleaseDate!.Value.Year);
        //    Assert.Equal("61810", game!.GameId);

        //    game = DataSerialization.FromJson<PlayniteGame>("""
        //        {"ReleaseDate":{"ReleaseDate":"1993"},"GameId":"61810","Name":"Chavez II"}
        //        """);
        //    Assert.Equal(1993, game!.ReleaseDate!.Value.Year);
        //    Assert.Equal("61810", game!.GameId);
        //    Assert.Equal("Chavez II", game!.Name);
        //}

        //[Fact]
        //public async Task SteamIdUseTest()
        //{
        //    var metadata = await GetMetadata(new PlayniteGame("")
        //    {
        //        PluginId = BuiltinExtensions.GetIdFromExtension(BuiltinExtension.SteamLibrary),
        //        GameId = "7200"
        //    });

        //    Assert.Equal("TrackMania United", metadata.name);
        //}

        //[Fact]
        //public async Task ReleaseDateUseTest()
        //{
        //    var game = new PlayniteGame("Tomb Raider")
        //    {
        //        ReleaseDate = new SdkModels.ReleaseDate(1996)
        //    };

        //    var metadata = await GetMetadata(game);
        //    Assert.Equal(1996, GetYearFromUnix(metadata.first_release_date));
        //    Assert.Equal("Core Design", metadata.involved_companies?.Where(a => a.developer).FirstOrDefault()?.company?.name);

        //    game.ReleaseDate = new SdkModels.ReleaseDate(2013, 1, 1);
        //    metadata = await GetMetadata(game);
        //    Assert.Equal(2013, GetYearFromUnix(metadata.first_release_date));
        //    Assert.Equal("Crystal Dynamics", metadata.involved_companies?.Where(a => a.developer).FirstOrDefault()?.company?.name);
        //}

        //[Fact]
        //public async Task NameMatchingTest()
        //{
        //    // No-Intro naming
        //    var metadata = await GetMetadata(new PlayniteGame("Bug's Life, A"));
        //    Assert.Equal((ulong)49841, metadata.id);

        //    metadata = await GetMetadata(new PlayniteGame("Warhammer 40,000: Space Marine"));
        //    Assert.Equal((ulong)578, metadata.id);

        //    // & / and test
        //    metadata = await GetMetadata(new PlayniteGame("Command and Conquer"));
        //    Assert.NotNull(metadata.cover);
        //    Assert.Equal("Command & Conquer", metadata.name);
        //    Assert.Equal(1995, GetYearFromUnix(metadata.first_release_date));

        //    // Matches exactly
        //    metadata = await GetMetadata(new PlayniteGame("Grand Theft Auto IV"));
        //    Assert.Equal(2008, GetYearFromUnix(metadata.first_release_date));

        //    // Roman numerals test
        //    metadata = await GetMetadata(new PlayniteGame("Quake 3 Arena"));
        //    Assert.NotNull(metadata.cover);
        //    Assert.Equal("Quake III Arena", metadata.name);

        //    // THE test
        //    metadata = await GetMetadata(new PlayniteGame("Witcher 3: Wild Hunt"));
        //    Assert.Equal("The Witcher 3: Wild Hunt", metadata.name);

        //    // No subtitle test
        //    metadata = await GetMetadata(new PlayniteGame("The Witcher 3"));
        //    Assert.Equal("The Witcher 3: Wild Hunt", metadata.name);

        //    // Apostrophe test
        //    metadata = await GetMetadata(new PlayniteGame("Dragons Lair"));
        //    Assert.Equal("Dragon's Lair", metadata.name);

        //    // Hyphen vs. colon test
        //    metadata = await GetMetadata(new PlayniteGame("Legacy of Kain - Soul Reaver 2"));
        //    Assert.Equal("Legacy of Kain: Soul Reaver 2", metadata.name);

        //    metadata = await GetMetadata(new PlayniteGame("Legacy of Kain: Soul Reaver 2"));
        //    Assert.Equal("Legacy of Kain: Soul Reaver 2", metadata.name);

        //    // Trademarks test
        //    metadata = await GetMetadata(new PlayniteGame("Dishonored®: Death of the Outsider™"));
        //    Assert.Equal("Dishonored: Death of the Outsider", metadata.name);
        //}

        //[Fact]
        //public async Task CrashNameTests()
        //{
        //    await GetMetadata(new PlayniteGame(@"\millennium 2"));
        //    await GetMetadata(new PlayniteGame("BroForce.v864.201901211236"));
        //    await GetMetadata(new PlayniteGame("Danganronpa １・２ Reload"));
        //    await GetMetadata(new PlayniteGame("ココロクローバー パート１/Kokoro Clover Part1"));
        //}

        //[Fact]
        //public async Task DashSearch()
        //{
        //    var response = await (await client.GetAsync(@"/igdb/games/x-com")).Content.ReadAsStringAsync();
        //    var data = DataSerialization.FromJson<ServicesResponse<List<ExpandedGameLegacy>>>(response);
        //    Assert.NotNull(data?.Data?.FirstOrDefault(a => a.name == "X-COM: UFO Defense"));
        //}

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
        public async Task SearchTest()
        {
            var search = new SearchRequest { SearchTerm = "quake" };
            var content = new StringContent(DataSerialization.ToJson(search), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync(@"/igdb/search", content);
            var games = DataSerialization.FromJson<DataResponse<List<Game>>>(await response.Content.ReadAsStringAsync())!.Data;
            Assert.NotNull(games);
            Assert.NotNull(games.FirstOrDefault(a => a.id == 333));
        }
    }
}
