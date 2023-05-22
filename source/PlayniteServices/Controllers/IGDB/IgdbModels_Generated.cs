namespace PlayniteServices.Controllers.IGDB;
public enum AgeRatingCategoryEnum
{
    AGERATING_CATEGORY_NULL = 0,
    ESRB = 1,
    PEGI = 2,
    CERO = 3,
    USK = 4,
    GRAC = 5,
    CLASS_IND = 6,
    ACB = 7,
}

public enum AgeRatingRatingEnum
{
    AGERATING_RATING_NULL = 0,
    THREE = 1,
    SEVEN = 2,
    TWELVE = 3,
    SIXTEEN = 4,
    EIGHTEEN = 5,
    RP = 6,
    EC = 7,
    E = 8,
    E10 = 9,
    T = 10,
    M = 11,
    AO = 12,
    CERO_A = 13,
    CERO_B = 14,
    CERO_C = 15,
    CERO_D = 16,
    CERO_Z = 17,
    USK_0 = 18,
    USK_6 = 19,
    USK_12 = 20,
    USK_16 = 21,
    USK_18 = 22,
    GRAC_ALL = 23,
    GRAC_TWELVE = 24,
    GRAC_FIFTEEN = 25,
    GRAC_EIGHTEEN = 26,
    GRAC_TESTING = 27,
    CLASS_IND_L = 28,
    CLASS_IND_TEN = 29,
    CLASS_IND_TWELVE = 30,
    CLASS_IND_FOURTEEN = 31,
    CLASS_IND_SIXTEEN = 32,
    CLASS_IND_EIGHTEEN = 33,
    ACB_G = 34,
    ACB_PG = 35,
    ACB_M = 36,
    ACB_MA15 = 37,
    ACB_R18 = 38,
    ACB_RC = 39,
}

public enum AgeRatingContentDescriptionCategoryEnum
{
    AGERATINGCONTENTDESCRIPTION_CATEGORY_NULL = 0,
    ESRB_ALCOHOL_REFERENCE = 1,
    ESRB_ANIMATED_BLOOD = 2,
    ESRB_BLOOD = 3,
    ESRB_BLOOD_AND_GORE = 4,
    ESRB_CARTOON_VIOLENCE = 5,
    ESRB_COMIC_MISCHIEF = 6,
    ESRB_CRUDE_HUMOR = 7,
    ESRB_DRUG_REFERENCE = 8,
    ESRB_FANTASY_VIOLENCE = 9,
    ESRB_INTENSE_VIOLENCE = 10,
    ESRB_LANGUAGE = 11,
    ESRB_LYRICS = 12,
    ESRB_MATURE_HUMOR = 13,
    ESRB_NUDITY = 14,
    ESRB_PARTIAL_NUDITY = 15,
    ESRB_REAL_GAMBLING = 16,
    ESRB_SEXUAL_CONTENT = 17,
    ESRB_SEXUAL_THEMES = 18,
    ESRB_SEXUAL_VIOLENCE = 19,
    ESRB_SIMULATED_GAMBLING = 20,
    ESRB_STRONG_LANGUAGE = 21,
    ESRB_STRONG_LYRICS = 22,
    ESRB_STRONG_SEXUAL_CONTENT = 23,
    ESRB_SUGGESTIVE_THEMES = 24,
    ESRB_TOBACCO_REFERENCE = 25,
    ESRB_USE_OF_ALCOHOL = 26,
    ESRB_USE_OF_DRUGS = 27,
    ESRB_USE_OF_TOBACCO = 28,
    ESRB_VIOLENCE = 29,
    ESRB_VIOLENT_REFERENCES = 30,
    ESRB_ANIMATED_VIOLENCE = 31,
    ESRB_MILD_LANGUAGE = 32,
    ESRB_MILD_VIOLENCE = 33,
    ESRB_USE_OF_DRUGS_AND_ALCOHOL = 34,
    ESRB_DRUG_AND_ALCOHOL_REFERENCE = 35,
    ESRB_MILD_SUGGESTIVE_THEMES = 36,
    ESRB_MILD_CARTOON_VIOLENCE = 37,
    ESRB_MILD_BLOOD = 38,
    ESRB_REALISTIC_BLOOD_AND_GORE = 39,
    ESRB_REALISTIC_VIOLENCE = 40,
    ESRB_ALCOHOL_AND_TOBACCO_REFERENCE = 41,
    ESRB_MATURE_SEXUAL_THEMES = 42,
    ESRB_MILD_ANIMATED_VIOLENCE = 43,
    ESRB_MILD_SEXUAL_THEMES = 44,
    ESRB_USE_OF_ALCOHOL_AND_TOBACCO = 45,
    ESRB_ANIMATED_BLOOD_AND_GORE = 46,
    ESRB_MILD_FANTASY_VIOLENCE = 47,
    ESRB_MILD_LYRICS = 48,
    ESRB_REALISTIC_BLOOD = 49,
    PEGI_VIOLENCE = 50,
    PEGI_SEX = 51,
    PEGI_DRUGS = 52,
    PEGI_FEAR = 53,
    PEGI_DISCRIMINATION = 54,
    PEGI_BAD_LANGUAGE = 55,
    PEGI_GAMBLING = 56,
    PEGI_ONLINE_GAMEPLAY = 57,
    PEGI_IN_GAME_PURCHASES = 58,
    CERO_LOVE = 59,
    CERO_SEXUAL_CONTENT = 60,
    CERO_VIOLENCE = 61,
    CERO_HORROR = 62,
    CERO_DRINKING_SMOKING = 63,
    CERO_GAMBLING = 64,
    CERO_CRIME = 65,
    CERO_CONTROLLED_SUBSTANCES = 66,
    CERO_LANGUAGES_AND_OTHERS = 67,
    GRAC_SEXUALITY = 68,
    GRAC_VIOLENCE = 69,
    GRAC_FEAR_HORROR_THREATENING = 70,
    GRAC_LANGUAGE = 71,
    GRAC_ALCOHOL_TOBACCO_DRUG = 72,
    GRAC_CRIME_ANTI_SOCIAL = 73,
    GRAC_GAMBLING = 74,
    CLASS_IND_VIOLENCIA = 75,
    CLASS_IND_VIOLENCIA_EXTREMA = 76,
    CLASS_IND_CONTEUDO_SEXUAL = 77,
    CLASS_IND_NUDEZ = 78,
    CLASS_IND_SEXO = 79,
    CLASS_IND_SEXO_EXPLICITO = 80,
    CLASS_IND_DROGAS = 81,
    CLASS_IND_DROGAS_LICITAS = 82,
    CLASS_IND_DROGAS_ILICITAS = 83,
    CLASS_IND_LINGUAGEM_IMPROPRIA = 84,
    CLASS_IND_ATOS_CRIMINOSOS = 85,
}

public enum GenderGenderEnum
{
    MALE = 0,
    FEMALE = 1,
    OTHER = 2,
}

public enum CharacterSpeciesEnum
{
    CHARACTER_SPECIES_NULL = 0,
    HUMAN = 1,
    ALIEN = 2,
    ANIMAL = 3,
    ANDROID = 4,
    UNKNOWN = 5,
}

public enum DateFormatChangeDateCategoryEnum
{
    YYYYMMMMDD = 0,
    YYYYMMMM = 1,
    YYYY = 2,
    YYYYQ1 = 3,
    YYYYQ2 = 4,
    YYYYQ3 = 5,
    YYYYQ4 = 6,
    TBD = 7,
}

public enum WebsiteCategoryEnum
{
    WEBSITE_CATEGORY_NULL = 0,
    WEBSITE_OFFICIAL = 1,
    WEBSITE_WIKIA = 2,
    WEBSITE_WIKIPEDIA = 3,
    WEBSITE_FACEBOOK = 4,
    WEBSITE_TWITTER = 5,
    WEBSITE_TWITCH = 6,
    WEBSITE_INSTAGRAM = 8,
    WEBSITE_YOUTUBE = 9,
    WEBSITE_IPHONE = 10,
    WEBSITE_IPAD = 11,
    WEBSITE_ANDROID = 12,
    WEBSITE_STEAM = 13,
    WEBSITE_REDDIT = 14,
    WEBSITE_ITCH = 15,
    WEBSITE_EPICGAMES = 16,
    WEBSITE_GOG = 17,
    WEBSITE_DISCORD = 18,
}

public enum ExternalGameCategoryEnum
{
    EXTERNALGAME_CATEGORY_NULL = 0,
    EXTERNALGAME_STEAM = 1,
    EXTERNALGAME_GOG = 5,
    EXTERNALGAME_YOUTUBE = 10,
    EXTERNALGAME_MICROSOFT = 11,
    EXTERNALGAME_APPLE = 13,
    EXTERNALGAME_TWITCH = 14,
    EXTERNALGAME_ANDROID = 15,
    EXTERNALGAME_AMAZON_ASIN = 20,
    EXTERNALGAME_AMAZON_LUNA = 22,
    EXTERNALGAME_AMAZON_ADG = 23,
    EXTERNALGAME_EPIC_GAME_STORE = 26,
    EXTERNALGAME_OCULUS = 28,
    EXTERNALGAME_UTOMIK = 29,
    EXTERNALGAME_ITCH_IO = 30,
    EXTERNALGAME_XBOX_MARKETPLACE = 31,
    EXTERNALGAME_KARTRIDGE = 32,
    EXTERNALGAME_PLAYSTATION_STORE_US = 36,
    EXTERNALGAME_FOCUS_ENTERTAINMENT = 37,
    EXTERNALGAME_XBOX_GAME_PASS_ULTIMATE_CLOUD = 54,
    EXTERNALGAME_GAMEJOLT = 55,
}

public enum ExternalGameMediaEnum
{
    EXTERNALGAME_MEDIA_NULL = 0,
    EXTERNALGAME_DIGITAL = 1,
    EXTERNALGAME_PHYSICAL = 2,
}

public enum GameCategoryEnum
{
    MAIN_GAME = 0,
    DLC_ADDON = 1,
    EXPANSION = 2,
    BUNDLE = 3,
    STANDALONE_EXPANSION = 4,
    MOD = 5,
    EPISODE = 6,
    SEASON = 7,
    REMAKE = 8,
    REMASTER = 9,
    EXPANDED_GAME = 10,
    PORT = 11,
    FORK = 12,
    PACK = 13,
    UPDATE = 14,
}

public enum GameStatusEnum
{
    RELEASED = 0,
    ALPHA = 2,
    BETA = 3,
    EARLY_ACCESS = 4,
    OFFLINE = 5,
    CANCELLED = 6,
    RUMORED = 7,
    DELISTED = 8,
}

public enum GameVersionFeatureCategoryEnum
{
    BOOLEAN = 0,
    DESCRIPTION = 1,
}

public enum GameVersionFeatureValueIncludedFeatureEnum
{
    NOT_INCLUDED = 0,
    INCLUDED = 1,
    PRE_ORDER_ONLY = 2,
}

public enum PlatformCategoryEnum
{
    PLATFORM_CATEGORY_NULL = 0,
    CONSOLE = 1,
    ARCADE = 2,
    PLATFORM = 3,
    OPERATING_SYSTEM = 4,
    PORTABLE_CONSOLE = 5,
    COMPUTER = 6,
}

public enum RegionRegionEnum
{
    REGION_REGION_NULL = 0,
    EUROPE = 1,
    NORTH_AMERICA = 2,
    AUSTRALIA = 3,
    NEW_ZEALAND = 4,
    JAPAN = 5,
    CHINA = 6,
    ASIA = 7,
    WORLDWIDE = 8,
    KOREA = 9,
    BRAZIL = 10,
}

public partial class AgeRating : IIgdbItem
{
    public ulong id { get; set; }
    public AgeRatingCategoryEnum category { get; set; }
    public List<ulong>? content_descriptions { get; set; }
    public AgeRatingRatingEnum rating { get; set; }
    public string? rating_cover_url { get; set; }
    public string? synopsis { get; set; }
    public string? checksum { get; set; }

    public List<AgeRatingContentDescription>? content_descriptions_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class AgeRatingContentDescription : IIgdbItem
{
    public ulong id { get; set; }
    public AgeRatingContentDescriptionCategoryEnum category { get; set; }
    public string? description { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class AlternativeName : IIgdbItem
{
    public ulong id { get; set; }
    public string? comment { get; set; }
    public ulong game { get; set; }
    public string? name { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Artwork : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public ulong game { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class Character : IIgdbItem
{
    public ulong id { get; set; }
    public List<string>? akas { get; set; }
    public string? country_name { get; set; }
    public long created_at { get; set; }
    public string? description { get; set; }
    public List<ulong>? games { get; set; }
    public GenderGenderEnum gender { get; set; }
    public ulong mug_shot { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public CharacterSpeciesEnum species { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public List<Game>? games_expanded { get; set; }
    public CharacterMugShot? mug_shot_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class CharacterMugShot : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class Collection : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public List<ulong>? games { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public List<Game>? games_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Company : IIgdbItem
{
    public ulong id { get; set; }
    public long change_date { get; set; }
    public DateFormatChangeDateCategoryEnum change_date_category { get; set; }
    public ulong changed_company_id { get; set; }
    public int country { get; set; }
    public long created_at { get; set; }
    public string? description { get; set; }
    public List<ulong>? developed { get; set; }
    public ulong logo { get; set; }
    public string? name { get; set; }
    public ulong parent { get; set; }
    public List<ulong>? published { get; set; }
    public string? slug { get; set; }
    public long start_date { get; set; }
    public DateFormatChangeDateCategoryEnum start_date_category { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public List<ulong>? websites { get; set; }
    public string? checksum { get; set; }

    public Company? changed_company_id_expanded { get; set; }
    public List<Game>? developed_expanded { get; set; }
    public CompanyLogo? logo_expanded { get; set; }
    public Company? parent_expanded { get; set; }
    public List<Game>? published_expanded { get; set; }
    public List<CompanyWebsite>? websites_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class CompanyLogo : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class CompanyWebsite : IIgdbItem
{
    public ulong id { get; set; }
    public WebsiteCategoryEnum category { get; set; }
    public bool trusted { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class Cover : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public ulong game { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }
    public ulong game_localization { get; set; }

    public Game? game_expanded { get; set; }
    public GameLocalization? game_localization_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class ExternalGame : IIgdbItem
{
    public ulong id { get; set; }
    public ExternalGameCategoryEnum category { get; set; }
    public long created_at { get; set; }
    public ulong game { get; set; }
    public string? name { get; set; }
    public string? uid { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public int year { get; set; }
    public ExternalGameMediaEnum media { get; set; }
    public ulong platform { get; set; }
    public List<int>? countries { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }
    public Platform? platform_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Franchise : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public List<ulong>? games { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public List<Game>? games_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Game : IIgdbItem
{
    public ulong id { get; set; }
    public List<ulong>? age_ratings { get; set; }
    public double aggregated_rating { get; set; }
    public int aggregated_rating_count { get; set; }
    public List<ulong>? alternative_names { get; set; }
    public List<ulong>? artworks { get; set; }
    public List<ulong>? bundles { get; set; }
    public GameCategoryEnum category { get; set; }
    public ulong collection { get; set; }
    public ulong cover { get; set; }
    public long created_at { get; set; }
    public List<ulong>? dlcs { get; set; }
    public List<ulong>? expansions { get; set; }
    public List<ulong>? external_games { get; set; }
    public long first_release_date { get; set; }
    public int follows { get; set; }
    public ulong franchise { get; set; }
    public List<ulong>? franchises { get; set; }
    public List<ulong>? game_engines { get; set; }
    public List<ulong>? game_modes { get; set; }
    public List<ulong>? genres { get; set; }
    public int hypes { get; set; }
    public List<ulong>? involved_companies { get; set; }
    public List<ulong>? keywords { get; set; }
    public List<ulong>? multiplayer_modes { get; set; }
    public string? name { get; set; }
    public ulong parent_game { get; set; }
    public List<ulong>? platforms { get; set; }
    public List<ulong>? player_perspectives { get; set; }
    public double rating { get; set; }
    public int rating_count { get; set; }
    public List<ulong>? release_dates { get; set; }
    public List<ulong>? screenshots { get; set; }
    public List<ulong>? similar_games { get; set; }
    public string? slug { get; set; }
    public List<ulong>? standalone_expansions { get; set; }
    public GameStatusEnum status { get; set; }
    public string? storyline { get; set; }
    public string? summary { get; set; }
    public List<int>? tags { get; set; }
    public List<ulong>? themes { get; set; }
    public double total_rating { get; set; }
    public int total_rating_count { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public ulong version_parent { get; set; }
    public string? version_title { get; set; }
    public List<ulong>? videos { get; set; }
    public List<ulong>? websites { get; set; }
    public string? checksum { get; set; }
    public List<ulong>? remakes { get; set; }
    public List<ulong>? remasters { get; set; }
    public List<ulong>? expanded_games { get; set; }
    public List<ulong>? ports { get; set; }
    public List<ulong>? forks { get; set; }
    public List<ulong>? language_supports { get; set; }
    public List<ulong>? game_localizations { get; set; }

    public List<AgeRating>? age_ratings_expanded { get; set; }
    public List<AlternativeName>? alternative_names_expanded { get; set; }
    public List<Artwork>? artworks_expanded { get; set; }
    public List<Game>? bundles_expanded { get; set; }
    public Collection? collection_expanded { get; set; }
    public Cover? cover_expanded { get; set; }
    public List<Game>? dlcs_expanded { get; set; }
    public List<Game>? expansions_expanded { get; set; }
    public List<ExternalGame>? external_games_expanded { get; set; }
    public Franchise? franchise_expanded { get; set; }
    public List<Franchise>? franchises_expanded { get; set; }
    public List<GameEngine>? game_engines_expanded { get; set; }
    public List<GameMode>? game_modes_expanded { get; set; }
    public List<Genre>? genres_expanded { get; set; }
    public List<InvolvedCompany>? involved_companies_expanded { get; set; }
    public List<Keyword>? keywords_expanded { get; set; }
    public List<MultiplayerMode>? multiplayer_modes_expanded { get; set; }
    public Game? parent_game_expanded { get; set; }
    public List<Platform>? platforms_expanded { get; set; }
    public List<PlayerPerspective>? player_perspectives_expanded { get; set; }
    public List<ReleaseDate>? release_dates_expanded { get; set; }
    public List<Screenshot>? screenshots_expanded { get; set; }
    public List<Game>? similar_games_expanded { get; set; }
    public List<Game>? standalone_expansions_expanded { get; set; }
    public List<Theme>? themes_expanded { get; set; }
    public Game? version_parent_expanded { get; set; }
    public List<GameVideo>? videos_expanded { get; set; }
    public List<Website>? websites_expanded { get; set; }
    public List<Game>? remakes_expanded { get; set; }
    public List<Game>? remasters_expanded { get; set; }
    public List<Game>? expanded_games_expanded { get; set; }
    public List<Game>? ports_expanded { get; set; }
    public List<Game>? forks_expanded { get; set; }
    public List<LanguageSupport>? language_supports_expanded { get; set; }
    public List<GameLocalization>? game_localizations_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class GameEngine : IIgdbItem
{
    public ulong id { get; set; }
    public List<ulong>? companies { get; set; }
    public long created_at { get; set; }
    public string? description { get; set; }
    public ulong logo { get; set; }
    public string? name { get; set; }
    public List<ulong>? platforms { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public List<Company>? companies_expanded { get; set; }
    public GameEngineLogo? logo_expanded { get; set; }
    public List<Platform>? platforms_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class GameEngineLogo : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class GameLocalization : IIgdbItem
{
    public ulong id { get; set; }
    public string? name { get; set; }
    public ulong cover { get; set; }
    public ulong game { get; set; }
    public ulong region { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }

    public Cover? cover_expanded { get; set; }
    public Game? game_expanded { get; set; }
    public Region? region_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class GameMode : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class GameVersion : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public List<ulong>? features { get; set; }
    public ulong game { get; set; }
    public List<ulong>? games { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public List<GameVersionFeature>? features_expanded { get; set; }
    public Game? game_expanded { get; set; }
    public List<Game>? games_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class GameVersionFeature : IIgdbItem
{
    public ulong id { get; set; }
    public GameVersionFeatureCategoryEnum category { get; set; }
    public string? description { get; set; }
    public int position { get; set; }
    public string? title { get; set; }
    public List<ulong>? values { get; set; }
    public string? checksum { get; set; }

    public List<GameVersionFeatureValue>? values_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class GameVersionFeatureValue : IIgdbItem
{
    public ulong id { get; set; }
    public ulong game { get; set; }
    public ulong game_feature { get; set; }
    public GameVersionFeatureValueIncludedFeatureEnum included_feature { get; set; }
    public string? note { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }
    public GameVersionFeature? game_feature_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class GameVideo : IIgdbItem
{
    public ulong id { get; set; }
    public ulong game { get; set; }
    public string? name { get; set; }
    public string? video_id { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Genre : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class InvolvedCompany : IIgdbItem
{
    public ulong id { get; set; }
    public ulong company { get; set; }
    public long created_at { get; set; }
    public bool developer { get; set; }
    public ulong game { get; set; }
    public bool porting { get; set; }
    public bool publisher { get; set; }
    public bool supporting { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }

    public Company? company_expanded { get; set; }
    public Game? game_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class Keyword : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Language : IIgdbItem
{
    public ulong id { get; set; }
    public string? name { get; set; }
    public string? native_name { get; set; }
    public string? locale { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class LanguageSupport : IIgdbItem
{
    public ulong id { get; set; }
    public ulong game { get; set; }
    public ulong language { get; set; }
    public ulong language_support_type { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }
    public Language? language_expanded { get; set; }
    public LanguageSupportType? language_support_type_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class LanguageSupportType : IIgdbItem
{
    public ulong id { get; set; }
    public string? name { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class MultiplayerMode : IIgdbItem
{
    public ulong id { get; set; }
    public bool campaigncoop { get; set; }
    public bool dropin { get; set; }
    public ulong game { get; set; }
    public bool lancoop { get; set; }
    public bool offlinecoop { get; set; }
    public int offlinecoopmax { get; set; }
    public int offlinemax { get; set; }
    public bool onlinecoop { get; set; }
    public int onlinecoopmax { get; set; }
    public int onlinemax { get; set; }
    public ulong platform { get; set; }
    public bool splitscreen { get; set; }
    public bool splitscreenonline { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }
    public Platform? platform_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class Platform : IIgdbItem
{
    public ulong id { get; set; }
    public string? abbreviation { get; set; }
    public string? alternative_name { get; set; }
    public PlatformCategoryEnum category { get; set; }
    public long created_at { get; set; }
    public int generation { get; set; }
    public string? name { get; set; }
    public ulong platform_logo { get; set; }
    public ulong platform_family { get; set; }
    public string? slug { get; set; }
    public string? summary { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public List<ulong>? versions { get; set; }
    public List<ulong>? websites { get; set; }
    public string? checksum { get; set; }

    public PlatformLogo? platform_logo_expanded { get; set; }
    public PlatformFamily? platform_family_expanded { get; set; }
    public List<PlatformVersion>? versions_expanded { get; set; }
    public List<PlatformWebsite>? websites_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class PlatformFamily : IIgdbItem
{
    public ulong id { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class PlatformLogo : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class PlatformVersion : IIgdbItem
{
    public ulong id { get; set; }
    public List<ulong>? companies { get; set; }
    public string? connectivity { get; set; }
    public string? cpu { get; set; }
    public string? graphics { get; set; }
    public ulong main_manufacturer { get; set; }
    public string? media { get; set; }
    public string? memory { get; set; }
    public string? name { get; set; }
    public string? online { get; set; }
    public string? os { get; set; }
    public string? output { get; set; }
    public ulong platform_logo { get; set; }
    public List<ulong>? platform_version_release_dates { get; set; }
    public string? resolutions { get; set; }
    public string? slug { get; set; }
    public string? sound { get; set; }
    public string? storage { get; set; }
    public string? summary { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public List<PlatformVersionCompany>? companies_expanded { get; set; }
    public PlatformVersionCompany? main_manufacturer_expanded { get; set; }
    public PlatformLogo? platform_logo_expanded { get; set; }
    public List<PlatformVersionReleaseDate>? platform_version_release_dates_expanded { get; set; }

    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class PlatformVersionCompany : IIgdbItem
{
    public ulong id { get; set; }
    public string? comment { get; set; }
    public ulong company { get; set; }
    public bool developer { get; set; }
    public bool manufacturer { get; set; }
    public string? checksum { get; set; }

    public Company? company_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class PlatformVersionReleaseDate : IIgdbItem
{
    public ulong id { get; set; }
    public DateFormatChangeDateCategoryEnum category { get; set; }
    public long created_at { get; set; }
    public long date { get; set; }
    public string? human { get; set; }
    public int m { get; set; }
    public ulong platform_version { get; set; }
    public RegionRegionEnum region { get; set; }
    public long updated_at { get; set; }
    public int y { get; set; }
    public string? checksum { get; set; }

    public PlatformVersion? platform_version_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class PlatformWebsite : IIgdbItem
{
    public ulong id { get; set; }
    public WebsiteCategoryEnum category { get; set; }
    public bool trusted { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class PlayerPerspective : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Region : IIgdbItem
{
    public ulong id { get; set; }
    public string? name { get; set; }
    public string? category { get; set; }
    public string? identifier { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class ReleaseDate : IIgdbItem
{
    public ulong id { get; set; }
    public DateFormatChangeDateCategoryEnum category { get; set; }
    public long created_at { get; set; }
    public long date { get; set; }
    public ulong game { get; set; }
    public string? human { get; set; }
    public int m { get; set; }
    public ulong platform { get; set; }
    public RegionRegionEnum region { get; set; }
    public long updated_at { get; set; }
    public int y { get; set; }
    public string? checksum { get; set; }
    public ulong status { get; set; }

    public Game? game_expanded { get; set; }
    public Platform? platform_expanded { get; set; }
    public ReleaseDateStatus? status_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class ReleaseDateStatus : IIgdbItem
{
    public ulong id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Screenshot : IIgdbItem
{
    public ulong id { get; set; }
    public bool alpha_channel { get; set; }
    public bool animated { get; set; }
    public ulong game { get; set; }
    public int height { get; set; }
    public string? image_id { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

public partial class Theme : IIgdbItem
{
    public ulong id { get; set; }
    public long created_at { get; set; }
    public string? name { get; set; }
    public string? slug { get; set; }
    public long updated_at { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }


    public override string ToString()
    {
        return $"{id}: {name}";
    }
}

public partial class Website : IIgdbItem
{
    public ulong id { get; set; }
    public WebsiteCategoryEnum category { get; set; }
    public ulong game { get; set; }
    public bool trusted { get; set; }
    public string? url { get; set; }
    public string? checksum { get; set; }

    public Game? game_expanded { get; set; }

    public override string ToString()
    {
        return id.ToString();
    }
}

