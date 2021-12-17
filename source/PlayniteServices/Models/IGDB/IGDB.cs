﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SdkModels = Playnite.SDK.Models;

namespace PlayniteServices.Models.IGDB
{
    public enum ReleaseStatus : ulong
    {
        Released = 0,
        Alpha =	2,
        Beta = 3,
        EarlyAccess = 4,
        Offline = 5,
        Cancelled =6,
        Rumored = 7,
        Delisted = 8
    }

    public enum WebSiteCategory : ulong
    {
        [Description("Official Website")]
        Official = 1,
        Wikia = 2,
        Wikipedia = 3,
        Facebook = 4,
        Twitter = 5,
        Twitch = 6,
        Instagram = 8,
        Youtube = 9,
        [Description("iPhone")]
        Iphone = 10,
        [Description("iPad")]
        Ipad = 11,
        Android = 12,
        Steam = 13,
        Reddit = 14,
        Itch = 15,
        Epic = 16,
        GOG = 17,
        Discord = 18
    }

    public enum GameCategory : ulong
    {
        MainGame = 0,
        DLC = 1,
        Expansion = 2,
        Bundle = 3,
        StandaloneExpansion = 4
    }

    public enum ReleaseDateCategory
    {
        YYYYMMMMDD = 0,
        YYYYMMMM = 1,
        YYYY = 2,
        YYYYQ1 = 3,
        YYYYQ2 = 4,
        YYYYQ3 = 5,
        YYYYQ4 = 6,
        TBD = 7
    }

    public enum Region
    {
        Europe = 1,
        NorthAmerica = 2,
        Australia = 3,
        NewZealand = 4,
        Japan = 5,
        China = 6,
        Asia = 7,
        Worldwide = 8,
        Korea = 9,
        Brazil = 10
    }

    public enum ExternalGameCategory : int
    {
        Steam = 1,
        Gog = 5,
        YouTube = 10,
        Microsoft = 11,
        Apple = 13,
        Twitch = 14,
        Android = 15,
        AmazonAsin = 20,
        AmazonLuna = 22,
        AmazonAdg = 23,
        EpicGameStore = 26,
        Oculus = 28
    }

    public enum ExternalGameMedia : int
    {
        Digital = 1,
        Physical = 2
    }

    public enum PlatformCategory
    {
        Console = 1,
        Arcade = 2,
        Platform = 3,
        OperatingSystem = 4,
        PortableConsole = 5,
        Computer = 6,
    }

    public enum AgeRatingOrganization
    {
        ESRB = 1,
        PEGI = 2
    }

    public enum AgeRatingType
    {
        [Description("3")]
        Three = 1,
        [Description("7")]
        Seven = 2,
        [Description("12")]
        Twelve = 3,
        [Description("16")]
        Sixteen = 4,
        [Description("18")]
        Eighteen = 5,
        RP = 6,
        EC = 7,
        E = 8,
        E10 = 9,
        T = 10,
        M = 11,
        AO = 12
    }

    public class Keyword : IgdbItem
    {
    }

    public class MultiplayerMode : IgdbItem
    {
        public ulong game { get; set; }
        public ulong platform { get; set; }
        public bool campaigncoop { get; set; }
        public bool dropin { get; set; }
        public bool lancoop { get; set; }
        public bool offlinecoop { get; set; }
        public bool onlinecoop { get; set; }
        public bool splitscreen { get; set; }
        public bool splitscreenonline { get; set; }
        public int offlinecoopmax { get; set; }
        public int offlinemax { get; set; }
        public int onlinecoopmax { get; set; }
        public int onlinemax { get; set; }
    }

    public class AgeRating : IgdbItem
    {
        public AgeRatingOrganization category { get; set; }
        public List<ulong> content_descriptions { get; set; }
        public AgeRatingType rating { get; set; }
        public string rating_cover_url { get; set; }
        public string synopsis { get; set; }
    }

    public class Website : IgdbItem
    {
        public WebSiteCategory category { get; set; }
        public ulong game { get; set; }
        public bool trusted { get; set; }
    }

    public class AlternativeName : IgdbItem
    {
        public string comment { get; set; }
        public ulong game { get; set; }
        public double? textScore { get; set; }
    }

    public class GameImage : IgdbItem
    {
        public bool animated { get; set; }
        public bool alpha_channel { get; set; }
        public string image_id { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
        public ulong game { get; set; }
    }

    public class Video : IgdbItem
    {
        public ulong game { get; set; }
        public string video_id { get; set; }
    }

    public class TimeTobeat : IgdbItem
    {
        public ulong game { get; set; }
        public ulong hastly { get; set; }
        public ulong normally { get; set; }
        public ulong completely { get; set; }
    }

    public class ReleaseDate : IgdbItem
    {
        public ReleaseDateCategory category;
        public long date { get; set; }
        public ulong game { get; set; }
        public string human { get; set; }
        public int m { get; set; }
        public ulong platform { get; set; }
        public Region region { get; set; }
        public int y { get; set; }
    }

    public class IgdbItem
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        public string checksum { get; set; }

        public override string ToString()
        {
            return $"{name} : {id}";
        }
    }

    public class ExternalGame : IgdbItem
    {
        public ExternalGameCategory category { get; set; }
        public ulong game { get; set; }
        public string uid { get; set; }
        public int year { get; set; }
        public List<int> countries { get; set; }
        public ExternalGameMedia media { get; set; }
        public ulong platform { get; set; }
    }

    public class Platform : IgdbItem
    {
        public string abbreviation { get; set; }
        public string alternative_name { get; set; }
        public PlatformCategory category { get; set; }
        public int generation { get; set; }
        public ulong platform_logo { get; set; }
        public ulong platform_family { get; set; }
        public string summary { get; set; }
    }

    public class Franchise : IgdbItem
    {
        public List<ulong> games { get; set; }
    }

    public class ProductFamily : IgdbItem
    {
    }

    public class GameMode : IgdbItem
    {
    }

    public class Theme : IgdbItem
    {
    }

    public class Company : IgdbItem
    {
        public ulong logo { get; set; }
        public int country { get; set; }
        public string description { get; set; }
        public ulong parent { get; set; }
        public List<ulong> developed { get; set; }
        public List<ulong> published { get; set; }
    }

    public class Genre : IgdbItem
    {
    }

    public class PlayerPerspective : IgdbItem
    {
    }

    public class InvolvedCompany : IgdbItem
    {
        public ulong game { get; set; }
        public ulong company { get; set; }
        public bool developer { get; set; }
        public bool porting { get; set; }
        public bool publisher { get; set; }
        public bool supporting { get; set; }
    }

    public class ExpandedInvolvedCompany : InvolvedCompany
    {
        public new Company company { get; set; }
    }

    public class Collection : IgdbItem
    {
        public List<ulong> games { get; set; }
    }

    public class ExpandedGameLegacy : Game
    {
        public new Franchise franchise { get; set; }
        public new Collection collection { get; set; }
        public new Game version_parent { get; set; }
        public new List<ExpandedInvolvedCompany> involved_companies { get; set; }
        public List<Genre> genres_v3 { get; set; }
        public new List<Theme> themes { get; set; }
        public List<GameMode> game_modes_v3 { get; set; }
        public GameImage cover_v3 { get; set; }
        public new List<Website> websites { get; set; }
        public new List<AlternativeName> alternative_names { get; set; }
        public new List<ExternalGame> external_games { get; set; }
        public new List<GameImage> screenshots { get; set; }
        public new List<GameImage> artworks { get; set; }
        public new List<Video> videos { get; set; }
        public new List<Platform> platforms { get; set; }
        public new List<ReleaseDate> release_dates { get; set; }
        public new List<PlayerPerspective> player_perspectives { get; set; }

        // fallback properties for 4.x
        public new string cover { get; set; }
        public List<string> developers { get; set; }
        public List<string> publishers { get; set; }
        public new List<string> genres { get; set; }
        public new List<string> game_modes { get; set; }
    }

    public class ExpandedGame
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        public string summary { get; set; }
        public string storyline { get; set; }
        public string version_title { get; set; }
        public GameCategory category { get; set; }
        public long first_release_date { get; set; }
        public double rating { get; set; }
        public double aggregated_rating { get; set; }
        public double total_rating { get; set; }
        public ulong parent_game { get; set; }

        public Game version_parent { get; set; }
        public Franchise franchise { get; set; }
        public Collection collection { get; set; }
        public List<ExpandedInvolvedCompany> involved_companies { get; set; }
        public List<Genre> genres { get; set; }
        public List<Theme> themes { get; set; }
        public List<GameMode> game_modes { get; set; }
        public GameImage cover { get; set; }
        public List<Website> websites { get; set; }
        public List<PlayerPerspective> player_perspectives { get; set; }
        public List<Franchise> franchises { get; set; }
        public List<Keyword> keywords { get; set; }
        public List<MultiplayerMode> multiplayer_modes { get; set; }
        public List<AlternativeName> alternative_names { get; set; }
        public List<ExternalGame> external_games { get; set; }
        public List<GameImage> screenshots { get; set; }
        public List<GameImage> artworks { get; set; }
        public List<Video> videos { get; set; }
        public List<Platform> platforms { get; set; }
        public List<ReleaseDate> release_dates { get; set; }
        public List<AgeRating> age_ratings { get; set; }
    }

    public class Game : IgdbItem
    {
        public string summary { get; set; }
        public string storyline { get; set; }
        public ulong franchise { get; set; }
        public ulong collection { get; set; }
        public ulong version_parent { get; set; }
        public string version_title { get; set; }
        public GameCategory category { get; set; }
        public List<ulong> involved_companies { get; set; }
        public List<ulong> genres { get; set; }
        public List<ulong> themes { get; set; }
        public List<ulong> game_modes { get; set; }
        public long first_release_date { get; set; }
        public ulong cover { get; set; }
        public List<ulong> websites { get; set; }
        public double rating { get; set; }
        public double aggregated_rating { get; set; }
        public double total_rating { get; set; }
        public List<ulong> alternative_names { get; set; }
        public List<ulong> external_games { get; set; }
        public List<ulong> screenshots { get; set; }
        public List<ulong> artworks { get; set; }
        public List<ulong> videos { get; set; }
        public List<ulong> platforms { get; set; }
        public List<ulong> release_dates { get; set; }
        public List<ulong> age_ratings { get; set; }
        public List<ulong> similar_games { get; set; }
        public List<ulong> player_perspectives { get; set; }
        public ulong parent_game { get; set; }
        public List<ulong> bundles { get; set; }
        public List<ulong> dlcs { get; set; }
        public List<ulong> expanded_games { get; set; }
        public List<ulong> expansions { get; set; }
        public List<ulong> forks { get; set; }
        public List<ulong> franchises { get; set; }
        public List<ulong> keywords { get; set; }
        public List<ulong> multiplayer_modes { get; set; }
        public List<ulong> ports { get; set; }
        public List<ulong> remakes { get; set; }
        public List<ulong> remasters { get; set; }
        public List<ulong> standalone_expansions { get; set; }
        public ReleaseStatus status { get; set; }
        public double? textScore { get; set; }
    }

    public class DbClonningGame : IgdbItem
    {
        public string summary { get; set; }
        public string storyline { get; set; }
        public ulong version_parent { get; set; }
        public string version_title { get; set; }
        public GameCategory category { get; set; }
        public long first_release_date { get; set; }
        public double rating { get; set; }
        public double aggregated_rating { get; set; }
        public double total_rating { get; set; }
        public List<ulong> similar_games { get; set; }
        public ulong parent_game { get; set; }
        public List<ulong> bundles { get; set; }
        public List<ulong> dlcs { get; set; }
        public List<ulong> expanded_games { get; set; }
        public List<ulong> expansions { get; set; }
        public List<ulong> forks { get; set; }
        public List<ulong> ports { get; set; }
        public List<ulong> remakes { get; set; }
        public List<ulong> remasters { get; set; }
        public List<ulong> standalone_expansions { get; set; }
        public ReleaseStatus status { get; set; }

        public Franchise franchise { get; set; }
        public Collection collection { get; set; }
        public List<InvolvedCompany> involved_companies { get; set; }
        public List<Genre> genres { get; set; }
        public List<Theme> themes { get; set; }
        public List<GameMode> game_modes { get; set; }
        public GameImage cover { get; set; }
        public List<Website> websites { get; set; }
        public List<PlayerPerspective> player_perspectives { get; set; }
        public List<Franchise> franchises { get; set; }
        public List<Keyword> keywords { get; set; }
        public List<MultiplayerMode> multiplayer_modes { get; set; }
        public List<AlternativeName> alternative_names { get; set; }
        public List<ExternalGame> external_games { get; set; }
        public List<GameImage> screenshots { get; set; }
        public List<GameImage> artworks { get; set; }
        public List<Video> videos { get; set; }
        public List<Platform> platforms { get; set; }
        public List<ReleaseDate> release_dates { get; set; }
        public List<AgeRating> age_ratings { get; set; }
    }

    public class SearchRequest
    {
        public string Name { get; set; }
    }

    public class GameMatchRequest
    {
        public string Name { get; set; }
        public SdkModels.ReleaseDate? ReleaseDate { get; set; }
        public string Platform { get; set; }
        public string PlatformSpecification { get; set; }
        public Guid PluginId { get; set; }
        public string GameId { get; set; }

        public string GetMatchId()
        {
            return $"{Name},{ReleaseDate?.Year},{Platform},{PlatformSpecification}";
        }
    }

    public static class ModelsUtils
    {
        public static string GetIgdbSearchString(string gameName)
        {
            var temp = gameName.Replace(":", " ").ToLower().Replace("\"", string.Empty).Trim();
            return Regex.Replace(temp, @"\s+", " ");
        }
    }
}
