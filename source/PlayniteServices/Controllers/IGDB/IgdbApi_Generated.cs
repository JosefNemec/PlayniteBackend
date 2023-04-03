using PlayniteServices.Controllers.IGDB;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
namespace PlayniteServices;
public partial class IgdbApi : IDisposable
{
    [AllowNull] public AgeRatingCollection AgeRatings { get; private set; }
    [AllowNull] public AgeRatingContentDescriptionCollection AgeRatingContentDescriptions { get; private set; }
    [AllowNull] public AlternativeNameCollection AlternativeNames { get; private set; }
    [AllowNull] public ArtworkCollection Artworks { get; private set; }
    [AllowNull] public CharacterCollection Characters { get; private set; }
    [AllowNull] public CharacterMugShotCollection CharacterMugShots { get; private set; }
    [AllowNull] public CollectionCollection Collections { get; private set; }
    [AllowNull] public CompanyCollection Companys { get; private set; }
    [AllowNull] public CompanyLogoCollection CompanyLogos { get; private set; }
    [AllowNull] public CompanyWebsiteCollection CompanyWebsites { get; private set; }
    [AllowNull] public CoverCollection Covers { get; private set; }
    [AllowNull] public ExternalGameCollection ExternalGames { get; private set; }
    [AllowNull] public FranchiseCollection Franchises { get; private set; }
    [AllowNull] public GameCollection Games { get; private set; }
    [AllowNull] public GameEngineCollection GameEngines { get; private set; }
    [AllowNull] public GameEngineLogoCollection GameEngineLogos { get; private set; }
    [AllowNull] public GameLocalizationCollection GameLocalizations { get; private set; }
    [AllowNull] public GameModeCollection GameModes { get; private set; }
    [AllowNull] public GameVersionCollection GameVersions { get; private set; }
    [AllowNull] public GameVersionFeatureCollection GameVersionFeatures { get; private set; }
    [AllowNull] public GameVersionFeatureValueCollection GameVersionFeatureValues { get; private set; }
    [AllowNull] public GameVideoCollection GameVideos { get; private set; }
    [AllowNull] public GenreCollection Genres { get; private set; }
    [AllowNull] public InvolvedCompanyCollection InvolvedCompanys { get; private set; }
    [AllowNull] public KeywordCollection Keywords { get; private set; }
    [AllowNull] public LanguageCollection Languages { get; private set; }
    [AllowNull] public LanguageSupportCollection LanguageSupports { get; private set; }
    [AllowNull] public LanguageSupportTypeCollection LanguageSupportTypes { get; private set; }
    [AllowNull] public MultiplayerModeCollection MultiplayerModes { get; private set; }
    [AllowNull] public PlatformCollection Platforms { get; private set; }
    [AllowNull] public PlatformFamilyCollection PlatformFamilys { get; private set; }
    [AllowNull] public PlatformLogoCollection PlatformLogos { get; private set; }
    [AllowNull] public PlatformVersionCollection PlatformVersions { get; private set; }
    [AllowNull] public PlatformVersionCompanyCollection PlatformVersionCompanys { get; private set; }
    [AllowNull] public PlatformVersionReleaseDateCollection PlatformVersionReleaseDates { get; private set; }
    [AllowNull] public PlatformWebsiteCollection PlatformWebsites { get; private set; }
    [AllowNull] public PlayerPerspectiveCollection PlayerPerspectives { get; private set; }
    [AllowNull] public RegionCollection Regions { get; private set; }
    [AllowNull] public ReleaseDateCollection ReleaseDates { get; private set; }
    [AllowNull] public ScreenshotCollection Screenshots { get; private set; }
    [AllowNull] public ThemeCollection Themes { get; private set; }
    [AllowNull] public WebsiteCollection Websites { get; private set; }
    void InitCollections()
    {
       AgeRatings = new AgeRatingCollection(this, Database);
       DataCollections.Add(AgeRatings);
       AgeRatingContentDescriptions = new AgeRatingContentDescriptionCollection(this, Database);
       DataCollections.Add(AgeRatingContentDescriptions);
       AlternativeNames = new AlternativeNameCollection(this, Database);
       DataCollections.Add(AlternativeNames);
       Artworks = new ArtworkCollection(this, Database);
       DataCollections.Add(Artworks);
       Characters = new CharacterCollection(this, Database);
       DataCollections.Add(Characters);
       CharacterMugShots = new CharacterMugShotCollection(this, Database);
       DataCollections.Add(CharacterMugShots);
       Collections = new CollectionCollection(this, Database);
       DataCollections.Add(Collections);
       Companys = new CompanyCollection(this, Database);
       DataCollections.Add(Companys);
       CompanyLogos = new CompanyLogoCollection(this, Database);
       DataCollections.Add(CompanyLogos);
       CompanyWebsites = new CompanyWebsiteCollection(this, Database);
       DataCollections.Add(CompanyWebsites);
       Covers = new CoverCollection(this, Database);
       DataCollections.Add(Covers);
       ExternalGames = new ExternalGameCollection(this, Database);
       DataCollections.Add(ExternalGames);
       Franchises = new FranchiseCollection(this, Database);
       DataCollections.Add(Franchises);
       Games = new GameCollection(this, Database);
       DataCollections.Add(Games);
       GameEngines = new GameEngineCollection(this, Database);
       DataCollections.Add(GameEngines);
       GameEngineLogos = new GameEngineLogoCollection(this, Database);
       DataCollections.Add(GameEngineLogos);
       GameLocalizations = new GameLocalizationCollection(this, Database);
       DataCollections.Add(GameLocalizations);
       GameModes = new GameModeCollection(this, Database);
       DataCollections.Add(GameModes);
       GameVersions = new GameVersionCollection(this, Database);
       DataCollections.Add(GameVersions);
       GameVersionFeatures = new GameVersionFeatureCollection(this, Database);
       DataCollections.Add(GameVersionFeatures);
       GameVersionFeatureValues = new GameVersionFeatureValueCollection(this, Database);
       DataCollections.Add(GameVersionFeatureValues);
       GameVideos = new GameVideoCollection(this, Database);
       DataCollections.Add(GameVideos);
       Genres = new GenreCollection(this, Database);
       DataCollections.Add(Genres);
       InvolvedCompanys = new InvolvedCompanyCollection(this, Database);
       DataCollections.Add(InvolvedCompanys);
       Keywords = new KeywordCollection(this, Database);
       DataCollections.Add(Keywords);
       Languages = new LanguageCollection(this, Database);
       DataCollections.Add(Languages);
       LanguageSupports = new LanguageSupportCollection(this, Database);
       DataCollections.Add(LanguageSupports);
       LanguageSupportTypes = new LanguageSupportTypeCollection(this, Database);
       DataCollections.Add(LanguageSupportTypes);
       MultiplayerModes = new MultiplayerModeCollection(this, Database);
       DataCollections.Add(MultiplayerModes);
       Platforms = new PlatformCollection(this, Database);
       DataCollections.Add(Platforms);
       PlatformFamilys = new PlatformFamilyCollection(this, Database);
       DataCollections.Add(PlatformFamilys);
       PlatformLogos = new PlatformLogoCollection(this, Database);
       DataCollections.Add(PlatformLogos);
       PlatformVersions = new PlatformVersionCollection(this, Database);
       DataCollections.Add(PlatformVersions);
       PlatformVersionCompanys = new PlatformVersionCompanyCollection(this, Database);
       DataCollections.Add(PlatformVersionCompanys);
       PlatformVersionReleaseDates = new PlatformVersionReleaseDateCollection(this, Database);
       DataCollections.Add(PlatformVersionReleaseDates);
       PlatformWebsites = new PlatformWebsiteCollection(this, Database);
       DataCollections.Add(PlatformWebsites);
       PlayerPerspectives = new PlayerPerspectiveCollection(this, Database);
       DataCollections.Add(PlayerPerspectives);
       Regions = new RegionCollection(this, Database);
       DataCollections.Add(Regions);
       ReleaseDates = new ReleaseDateCollection(this, Database);
       DataCollections.Add(ReleaseDates);
       Screenshots = new ScreenshotCollection(this, Database);
       DataCollections.Add(Screenshots);
       Themes = new ThemeCollection(this, Database);
       DataCollections.Add(Themes);
       Websites = new WebsiteCollection(this, Database);
       DataCollections.Add(Websites);
    }
public class AgeRatingCollection : IgdbCollection<AgeRating>
{
    public AgeRatingCollection(IgdbApi igdb, Database database) : base(igdb, "age_ratings", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class AgeRatingContentDescriptionCollection : IgdbCollection<AgeRatingContentDescription>
{
    public AgeRatingContentDescriptionCollection(IgdbApi igdb, Database database) : base(igdb, "age_rating_content_descriptions", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class AlternativeNameCollection : IgdbCollection<AlternativeName>
{
    public AlternativeNameCollection(IgdbApi igdb, Database database) : base(igdb, "alternative_names", database)
    {
    }
public override void CreateIndexes()
{
collection.Indexes.CreateOne(new CreateIndexModel<AlternativeName>(Builders<AlternativeName>.IndexKeys.Text(x => x.name)));
}
}
public class ArtworkCollection : IgdbCollection<Artwork>
{
    public ArtworkCollection(IgdbApi igdb, Database database) : base(igdb, "artworks", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CharacterCollection : IgdbCollection<Character>
{
    public CharacterCollection(IgdbApi igdb, Database database) : base(igdb, "characters", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CharacterMugShotCollection : IgdbCollection<CharacterMugShot>
{
    public CharacterMugShotCollection(IgdbApi igdb, Database database) : base(igdb, "character_mug_shots", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CollectionCollection : IgdbCollection<Collection>
{
    public CollectionCollection(IgdbApi igdb, Database database) : base(igdb, "collections", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CompanyCollection : IgdbCollection<Company>
{
    public CompanyCollection(IgdbApi igdb, Database database) : base(igdb, "companies", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CompanyLogoCollection : IgdbCollection<CompanyLogo>
{
    public CompanyLogoCollection(IgdbApi igdb, Database database) : base(igdb, "company_logos", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CompanyWebsiteCollection : IgdbCollection<CompanyWebsite>
{
    public CompanyWebsiteCollection(IgdbApi igdb, Database database) : base(igdb, "company_websites", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class CoverCollection : IgdbCollection<Cover>
{
    public CoverCollection(IgdbApi igdb, Database database) : base(igdb, "covers", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class ExternalGameCollection : IgdbCollection<ExternalGame>
{
    public ExternalGameCollection(IgdbApi igdb, Database database) : base(igdb, "external_games", database)
    {
    }
public override void CreateIndexes()
{
collection.Indexes.CreateOne(new CreateIndexModel<ExternalGame>(Builders<ExternalGame>.IndexKeys.Ascending(x => x.uid)));
}
}
public class FranchiseCollection : IgdbCollection<Franchise>
{
    public FranchiseCollection(IgdbApi igdb, Database database) : base(igdb, "franchises", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameCollection : IgdbCollection<Game>
{
    public GameCollection(IgdbApi igdb, Database database) : base(igdb, "games", database)
    {
    }
public override void CreateIndexes()
{
collection.Indexes.CreateOne(new CreateIndexModel<Game>(Builders<Game>.IndexKeys.Text(x => x.name)));
}
}
public class GameEngineCollection : IgdbCollection<GameEngine>
{
    public GameEngineCollection(IgdbApi igdb, Database database) : base(igdb, "game_engines", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameEngineLogoCollection : IgdbCollection<GameEngineLogo>
{
    public GameEngineLogoCollection(IgdbApi igdb, Database database) : base(igdb, "game_engine_logos", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameLocalizationCollection : IgdbCollection<GameLocalization>
{
    public GameLocalizationCollection(IgdbApi igdb, Database database) : base(igdb, "game_localizations", database)
    {
    }
public override void CreateIndexes()
{
collection.Indexes.CreateOne(new CreateIndexModel<GameLocalization>(Builders<GameLocalization>.IndexKeys.Text(x => x.name)));
}
}
public class GameModeCollection : IgdbCollection<GameMode>
{
    public GameModeCollection(IgdbApi igdb, Database database) : base(igdb, "game_modes", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameVersionCollection : IgdbCollection<GameVersion>
{
    public GameVersionCollection(IgdbApi igdb, Database database) : base(igdb, "game_versions", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameVersionFeatureCollection : IgdbCollection<GameVersionFeature>
{
    public GameVersionFeatureCollection(IgdbApi igdb, Database database) : base(igdb, "game_version_features", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameVersionFeatureValueCollection : IgdbCollection<GameVersionFeatureValue>
{
    public GameVersionFeatureValueCollection(IgdbApi igdb, Database database) : base(igdb, "game_version_feature_values", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GameVideoCollection : IgdbCollection<GameVideo>
{
    public GameVideoCollection(IgdbApi igdb, Database database) : base(igdb, "game_videos", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class GenreCollection : IgdbCollection<Genre>
{
    public GenreCollection(IgdbApi igdb, Database database) : base(igdb, "genres", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class InvolvedCompanyCollection : IgdbCollection<InvolvedCompany>
{
    public InvolvedCompanyCollection(IgdbApi igdb, Database database) : base(igdb, "involved_companies", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class KeywordCollection : IgdbCollection<Keyword>
{
    public KeywordCollection(IgdbApi igdb, Database database) : base(igdb, "keywords", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class LanguageCollection : IgdbCollection<Language>
{
    public LanguageCollection(IgdbApi igdb, Database database) : base(igdb, "languages", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class LanguageSupportCollection : IgdbCollection<LanguageSupport>
{
    public LanguageSupportCollection(IgdbApi igdb, Database database) : base(igdb, "language_supports", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class LanguageSupportTypeCollection : IgdbCollection<LanguageSupportType>
{
    public LanguageSupportTypeCollection(IgdbApi igdb, Database database) : base(igdb, "language_support_types", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class MultiplayerModeCollection : IgdbCollection<MultiplayerMode>
{
    public MultiplayerModeCollection(IgdbApi igdb, Database database) : base(igdb, "multiplayer_modes", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformCollection : IgdbCollection<Platform>
{
    public PlatformCollection(IgdbApi igdb, Database database) : base(igdb, "platforms", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformFamilyCollection : IgdbCollection<PlatformFamily>
{
    public PlatformFamilyCollection(IgdbApi igdb, Database database) : base(igdb, "platform_families", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformLogoCollection : IgdbCollection<PlatformLogo>
{
    public PlatformLogoCollection(IgdbApi igdb, Database database) : base(igdb, "platform_logos", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformVersionCollection : IgdbCollection<PlatformVersion>
{
    public PlatformVersionCollection(IgdbApi igdb, Database database) : base(igdb, "platform_versions", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformVersionCompanyCollection : IgdbCollection<PlatformVersionCompany>
{
    public PlatformVersionCompanyCollection(IgdbApi igdb, Database database) : base(igdb, "platform_version_companies", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformVersionReleaseDateCollection : IgdbCollection<PlatformVersionReleaseDate>
{
    public PlatformVersionReleaseDateCollection(IgdbApi igdb, Database database) : base(igdb, "platform_version_release_dates", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlatformWebsiteCollection : IgdbCollection<PlatformWebsite>
{
    public PlatformWebsiteCollection(IgdbApi igdb, Database database) : base(igdb, "platform_websites", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class PlayerPerspectiveCollection : IgdbCollection<PlayerPerspective>
{
    public PlayerPerspectiveCollection(IgdbApi igdb, Database database) : base(igdb, "player_perspectives", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class RegionCollection : IgdbCollection<Region>
{
    public RegionCollection(IgdbApi igdb, Database database) : base(igdb, "regions", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class ReleaseDateCollection : IgdbCollection<ReleaseDate>
{
    public ReleaseDateCollection(IgdbApi igdb, Database database) : base(igdb, "release_dates", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class ScreenshotCollection : IgdbCollection<Screenshot>
{
    public ScreenshotCollection(IgdbApi igdb, Database database) : base(igdb, "screenshots", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class ThemeCollection : IgdbCollection<Theme>
{
    public ThemeCollection(IgdbApi igdb, Database database) : base(igdb, "themes", database)
    {
    }
public override void CreateIndexes()
{
}
}
public class WebsiteCollection : IgdbCollection<Website>
{
    public WebsiteCollection(IgdbApi igdb, Database database) : base(igdb, "websites", database)
    {
    }
public override void CreateIndexes()
{
}
}
}
