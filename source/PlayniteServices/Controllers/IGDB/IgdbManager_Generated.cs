using PlayniteServices.IGDB;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
namespace PlayniteServices.IGDB;
public partial class IgdbManager : IDisposable
{
    [AllowNull] public AgeRatingCollection AgeRatings { get; private set; }
    [AllowNull] public AgeRatingContentDescriptionCollection AgeRatingContentDescriptions { get; private set; }
    [AllowNull] public AlternativeNameCollection AlternativeNames { get; private set; }
    [AllowNull] public ArtworkCollection Artworks { get; private set; }
    [AllowNull] public CharacterCollection Characters { get; private set; }
    [AllowNull] public CharacterMugShotCollection CharacterMugShots { get; private set; }
    [AllowNull] public CollectionCollection Collections { get; private set; }
    [AllowNull] public CollectionMembershipCollection CollectionMemberships { get; private set; }
    [AllowNull] public CollectionMembershipTypeCollection CollectionMembershipTypes { get; private set; }
    [AllowNull] public CollectionRelationCollection CollectionRelations { get; private set; }
    [AllowNull] public CollectionRelationTypeCollection CollectionRelationTypes { get; private set; }
    [AllowNull] public CollectionTypeCollection CollectionTypes { get; private set; }
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
    [AllowNull] public GameTimeToBeatCollection GameTimeToBeats { get; private set; }
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
    [AllowNull] public ReleaseDateStatusCollection ReleaseDateStatuss { get; private set; }
    [AllowNull] public ScreenshotCollection Screenshots { get; private set; }
    [AllowNull] public ThemeCollection Themes { get; private set; }
    [AllowNull] public WebsiteCollection Websites { get; private set; }
    void InitCollections()
    {
       AgeRatings = new AgeRatingCollection(this, Database);
       AgeRatings.CreateIndexes();
       DataCollections.Add(AgeRatings);
       AgeRatingContentDescriptions = new AgeRatingContentDescriptionCollection(this, Database);
       AgeRatingContentDescriptions.CreateIndexes();
       DataCollections.Add(AgeRatingContentDescriptions);
       AlternativeNames = new AlternativeNameCollection(this, Database);
       AlternativeNames.CreateIndexes();
       DataCollections.Add(AlternativeNames);
       Artworks = new ArtworkCollection(this, Database);
       Artworks.CreateIndexes();
       DataCollections.Add(Artworks);
       Characters = new CharacterCollection(this, Database);
       Characters.CreateIndexes();
       DataCollections.Add(Characters);
       CharacterMugShots = new CharacterMugShotCollection(this, Database);
       CharacterMugShots.CreateIndexes();
       DataCollections.Add(CharacterMugShots);
       Collections = new CollectionCollection(this, Database);
       Collections.CreateIndexes();
       DataCollections.Add(Collections);
       CollectionMemberships = new CollectionMembershipCollection(this, Database);
       CollectionMemberships.CreateIndexes();
       DataCollections.Add(CollectionMemberships);
       CollectionMembershipTypes = new CollectionMembershipTypeCollection(this, Database);
       CollectionMembershipTypes.CreateIndexes();
       DataCollections.Add(CollectionMembershipTypes);
       CollectionRelations = new CollectionRelationCollection(this, Database);
       CollectionRelations.CreateIndexes();
       DataCollections.Add(CollectionRelations);
       CollectionRelationTypes = new CollectionRelationTypeCollection(this, Database);
       CollectionRelationTypes.CreateIndexes();
       DataCollections.Add(CollectionRelationTypes);
       CollectionTypes = new CollectionTypeCollection(this, Database);
       CollectionTypes.CreateIndexes();
       DataCollections.Add(CollectionTypes);
       Companys = new CompanyCollection(this, Database);
       Companys.CreateIndexes();
       DataCollections.Add(Companys);
       CompanyLogos = new CompanyLogoCollection(this, Database);
       CompanyLogos.CreateIndexes();
       DataCollections.Add(CompanyLogos);
       CompanyWebsites = new CompanyWebsiteCollection(this, Database);
       CompanyWebsites.CreateIndexes();
       DataCollections.Add(CompanyWebsites);
       Covers = new CoverCollection(this, Database);
       Covers.CreateIndexes();
       DataCollections.Add(Covers);
       ExternalGames = new ExternalGameCollection(this, Database);
       ExternalGames.CreateIndexes();
       DataCollections.Add(ExternalGames);
       Franchises = new FranchiseCollection(this, Database);
       Franchises.CreateIndexes();
       DataCollections.Add(Franchises);
       Games = new GameCollection(this, Database);
       Games.CreateIndexes();
       DataCollections.Add(Games);
       GameEngines = new GameEngineCollection(this, Database);
       GameEngines.CreateIndexes();
       DataCollections.Add(GameEngines);
       GameEngineLogos = new GameEngineLogoCollection(this, Database);
       GameEngineLogos.CreateIndexes();
       DataCollections.Add(GameEngineLogos);
       GameLocalizations = new GameLocalizationCollection(this, Database);
       GameLocalizations.CreateIndexes();
       DataCollections.Add(GameLocalizations);
       GameModes = new GameModeCollection(this, Database);
       GameModes.CreateIndexes();
       DataCollections.Add(GameModes);
       GameTimeToBeats = new GameTimeToBeatCollection(this, Database);
       GameTimeToBeats.CreateIndexes();
       DataCollections.Add(GameTimeToBeats);
       GameVersions = new GameVersionCollection(this, Database);
       GameVersions.CreateIndexes();
       DataCollections.Add(GameVersions);
       GameVersionFeatures = new GameVersionFeatureCollection(this, Database);
       GameVersionFeatures.CreateIndexes();
       DataCollections.Add(GameVersionFeatures);
       GameVersionFeatureValues = new GameVersionFeatureValueCollection(this, Database);
       GameVersionFeatureValues.CreateIndexes();
       DataCollections.Add(GameVersionFeatureValues);
       GameVideos = new GameVideoCollection(this, Database);
       GameVideos.CreateIndexes();
       DataCollections.Add(GameVideos);
       Genres = new GenreCollection(this, Database);
       Genres.CreateIndexes();
       DataCollections.Add(Genres);
       InvolvedCompanys = new InvolvedCompanyCollection(this, Database);
       InvolvedCompanys.CreateIndexes();
       DataCollections.Add(InvolvedCompanys);
       Keywords = new KeywordCollection(this, Database);
       Keywords.CreateIndexes();
       DataCollections.Add(Keywords);
       Languages = new LanguageCollection(this, Database);
       Languages.CreateIndexes();
       DataCollections.Add(Languages);
       LanguageSupports = new LanguageSupportCollection(this, Database);
       LanguageSupports.CreateIndexes();
       DataCollections.Add(LanguageSupports);
       LanguageSupportTypes = new LanguageSupportTypeCollection(this, Database);
       LanguageSupportTypes.CreateIndexes();
       DataCollections.Add(LanguageSupportTypes);
       MultiplayerModes = new MultiplayerModeCollection(this, Database);
       MultiplayerModes.CreateIndexes();
       DataCollections.Add(MultiplayerModes);
       Platforms = new PlatformCollection(this, Database);
       Platforms.CreateIndexes();
       DataCollections.Add(Platforms);
       PlatformFamilys = new PlatformFamilyCollection(this, Database);
       PlatformFamilys.CreateIndexes();
       DataCollections.Add(PlatformFamilys);
       PlatformLogos = new PlatformLogoCollection(this, Database);
       PlatformLogos.CreateIndexes();
       DataCollections.Add(PlatformLogos);
       PlatformVersions = new PlatformVersionCollection(this, Database);
       PlatformVersions.CreateIndexes();
       DataCollections.Add(PlatformVersions);
       PlatformVersionCompanys = new PlatformVersionCompanyCollection(this, Database);
       PlatformVersionCompanys.CreateIndexes();
       DataCollections.Add(PlatformVersionCompanys);
       PlatformVersionReleaseDates = new PlatformVersionReleaseDateCollection(this, Database);
       PlatformVersionReleaseDates.CreateIndexes();
       DataCollections.Add(PlatformVersionReleaseDates);
       PlatformWebsites = new PlatformWebsiteCollection(this, Database);
       PlatformWebsites.CreateIndexes();
       DataCollections.Add(PlatformWebsites);
       PlayerPerspectives = new PlayerPerspectiveCollection(this, Database);
       PlayerPerspectives.CreateIndexes();
       DataCollections.Add(PlayerPerspectives);
       Regions = new RegionCollection(this, Database);
       Regions.CreateIndexes();
       DataCollections.Add(Regions);
       ReleaseDates = new ReleaseDateCollection(this, Database);
       ReleaseDates.CreateIndexes();
       DataCollections.Add(ReleaseDates);
       ReleaseDateStatuss = new ReleaseDateStatusCollection(this, Database);
       ReleaseDateStatuss.CreateIndexes();
       DataCollections.Add(ReleaseDateStatuss);
       Screenshots = new ScreenshotCollection(this, Database);
       Screenshots.CreateIndexes();
       DataCollections.Add(Screenshots);
       Themes = new ThemeCollection(this, Database);
       Themes.CreateIndexes();
       DataCollections.Add(Themes);
       Websites = new WebsiteCollection(this, Database);
       Websites.CreateIndexes();
       DataCollections.Add(Websites);
    }
public static void RegisterClassMaps()
{
AgeRating.RegisterClassMap();
AgeRatingContentDescription.RegisterClassMap();
AlternativeName.RegisterClassMap();
Artwork.RegisterClassMap();
Character.RegisterClassMap();
CharacterMugShot.RegisterClassMap();
Collection.RegisterClassMap();
CollectionMembership.RegisterClassMap();
CollectionMembershipType.RegisterClassMap();
CollectionRelation.RegisterClassMap();
CollectionRelationType.RegisterClassMap();
CollectionType.RegisterClassMap();
Company.RegisterClassMap();
CompanyLogo.RegisterClassMap();
CompanyWebsite.RegisterClassMap();
Cover.RegisterClassMap();
ExternalGame.RegisterClassMap();
Franchise.RegisterClassMap();
Game.RegisterClassMap();
GameEngine.RegisterClassMap();
GameEngineLogo.RegisterClassMap();
GameLocalization.RegisterClassMap();
GameMode.RegisterClassMap();
GameTimeToBeat.RegisterClassMap();
GameVersion.RegisterClassMap();
GameVersionFeature.RegisterClassMap();
GameVersionFeatureValue.RegisterClassMap();
GameVideo.RegisterClassMap();
Genre.RegisterClassMap();
InvolvedCompany.RegisterClassMap();
Keyword.RegisterClassMap();
Language.RegisterClassMap();
LanguageSupport.RegisterClassMap();
LanguageSupportType.RegisterClassMap();
MultiplayerMode.RegisterClassMap();
Platform.RegisterClassMap();
PlatformFamily.RegisterClassMap();
PlatformLogo.RegisterClassMap();
PlatformVersion.RegisterClassMap();
PlatformVersionCompany.RegisterClassMap();
PlatformVersionReleaseDate.RegisterClassMap();
PlatformWebsite.RegisterClassMap();
PlayerPerspective.RegisterClassMap();
Region.RegisterClassMap();
ReleaseDate.RegisterClassMap();
ReleaseDateStatus.RegisterClassMap();
Screenshot.RegisterClassMap();
Theme.RegisterClassMap();
Website.RegisterClassMap();
}
}
public partial class AgeRatingCollection : IgdbCollection<AgeRating>
{
    public AgeRatingCollection(IgdbManager igdb, Database database) : base("age_ratings", igdb, database)
    {
    }
}
public partial class AgeRatingContentDescriptionCollection : IgdbCollection<AgeRatingContentDescription>
{
    public AgeRatingContentDescriptionCollection(IgdbManager igdb, Database database) : base("age_rating_content_descriptions", igdb, database)
    {
    }
}
public partial class AlternativeNameCollection : IgdbCollection<AlternativeName>
{
    public AlternativeNameCollection(IgdbManager igdb, Database database) : base("alternative_names", igdb, database)
    {
    }
}
public partial class ArtworkCollection : IgdbCollection<Artwork>
{
    public ArtworkCollection(IgdbManager igdb, Database database) : base("artworks", igdb, database)
    {
    }
}
public partial class CharacterCollection : IgdbCollection<Character>
{
    public CharacterCollection(IgdbManager igdb, Database database) : base("characters", igdb, database)
    {
    }
}
public partial class CharacterMugShotCollection : IgdbCollection<CharacterMugShot>
{
    public CharacterMugShotCollection(IgdbManager igdb, Database database) : base("character_mug_shots", igdb, database)
    {
    }
}
public partial class CollectionCollection : IgdbCollection<Collection>
{
    public CollectionCollection(IgdbManager igdb, Database database) : base("collections", igdb, database)
    {
    }
}
public partial class CollectionMembershipCollection : IgdbCollection<CollectionMembership>
{
    public CollectionMembershipCollection(IgdbManager igdb, Database database) : base("collection_memberships", igdb, database)
    {
    }
}
public partial class CollectionMembershipTypeCollection : IgdbCollection<CollectionMembershipType>
{
    public CollectionMembershipTypeCollection(IgdbManager igdb, Database database) : base("collection_membership_types", igdb, database)
    {
    }
}
public partial class CollectionRelationCollection : IgdbCollection<CollectionRelation>
{
    public CollectionRelationCollection(IgdbManager igdb, Database database) : base("collection_relations", igdb, database)
    {
    }
}
public partial class CollectionRelationTypeCollection : IgdbCollection<CollectionRelationType>
{
    public CollectionRelationTypeCollection(IgdbManager igdb, Database database) : base("collection_relation_types", igdb, database)
    {
    }
}
public partial class CollectionTypeCollection : IgdbCollection<CollectionType>
{
    public CollectionTypeCollection(IgdbManager igdb, Database database) : base("collection_types", igdb, database)
    {
    }
}
public partial class CompanyCollection : IgdbCollection<Company>
{
    public CompanyCollection(IgdbManager igdb, Database database) : base("companies", igdb, database)
    {
    }
}
public partial class CompanyLogoCollection : IgdbCollection<CompanyLogo>
{
    public CompanyLogoCollection(IgdbManager igdb, Database database) : base("company_logos", igdb, database)
    {
    }
}
public partial class CompanyWebsiteCollection : IgdbCollection<CompanyWebsite>
{
    public CompanyWebsiteCollection(IgdbManager igdb, Database database) : base("company_websites", igdb, database)
    {
    }
}
public partial class CoverCollection : IgdbCollection<Cover>
{
    public CoverCollection(IgdbManager igdb, Database database) : base("covers", igdb, database)
    {
    }
}
public partial class ExternalGameCollection : IgdbCollection<ExternalGame>
{
    public ExternalGameCollection(IgdbManager igdb, Database database) : base("external_games", igdb, database)
    {
    }
}
public partial class FranchiseCollection : IgdbCollection<Franchise>
{
    public FranchiseCollection(IgdbManager igdb, Database database) : base("franchises", igdb, database)
    {
    }
}
public partial class GameCollection : IgdbCollection<Game>
{
    public GameCollection(IgdbManager igdb, Database database) : base("games", igdb, database)
    {
    }
}
public partial class GameEngineCollection : IgdbCollection<GameEngine>
{
    public GameEngineCollection(IgdbManager igdb, Database database) : base("game_engines", igdb, database)
    {
    }
}
public partial class GameEngineLogoCollection : IgdbCollection<GameEngineLogo>
{
    public GameEngineLogoCollection(IgdbManager igdb, Database database) : base("game_engine_logos", igdb, database)
    {
    }
}
public partial class GameLocalizationCollection : IgdbCollection<GameLocalization>
{
    public GameLocalizationCollection(IgdbManager igdb, Database database) : base("game_localizations", igdb, database)
    {
    }
}
public partial class GameModeCollection : IgdbCollection<GameMode>
{
    public GameModeCollection(IgdbManager igdb, Database database) : base("game_modes", igdb, database)
    {
    }
}
public partial class GameTimeToBeatCollection : IgdbCollection<GameTimeToBeat>
{
    public GameTimeToBeatCollection(IgdbManager igdb, Database database) : base("game_time_to_beats", igdb, database)
    {
    }
}
public partial class GameVersionCollection : IgdbCollection<GameVersion>
{
    public GameVersionCollection(IgdbManager igdb, Database database) : base("game_versions", igdb, database)
    {
    }
}
public partial class GameVersionFeatureCollection : IgdbCollection<GameVersionFeature>
{
    public GameVersionFeatureCollection(IgdbManager igdb, Database database) : base("game_version_features", igdb, database)
    {
    }
}
public partial class GameVersionFeatureValueCollection : IgdbCollection<GameVersionFeatureValue>
{
    public GameVersionFeatureValueCollection(IgdbManager igdb, Database database) : base("game_version_feature_values", igdb, database)
    {
    }
}
public partial class GameVideoCollection : IgdbCollection<GameVideo>
{
    public GameVideoCollection(IgdbManager igdb, Database database) : base("game_videos", igdb, database)
    {
    }
}
public partial class GenreCollection : IgdbCollection<Genre>
{
    public GenreCollection(IgdbManager igdb, Database database) : base("genres", igdb, database)
    {
    }
}
public partial class InvolvedCompanyCollection : IgdbCollection<InvolvedCompany>
{
    public InvolvedCompanyCollection(IgdbManager igdb, Database database) : base("involved_companies", igdb, database)
    {
    }
}
public partial class KeywordCollection : IgdbCollection<Keyword>
{
    public KeywordCollection(IgdbManager igdb, Database database) : base("keywords", igdb, database)
    {
    }
}
public partial class LanguageCollection : IgdbCollection<Language>
{
    public LanguageCollection(IgdbManager igdb, Database database) : base("languages", igdb, database)
    {
    }
}
public partial class LanguageSupportCollection : IgdbCollection<LanguageSupport>
{
    public LanguageSupportCollection(IgdbManager igdb, Database database) : base("language_supports", igdb, database)
    {
    }
}
public partial class LanguageSupportTypeCollection : IgdbCollection<LanguageSupportType>
{
    public LanguageSupportTypeCollection(IgdbManager igdb, Database database) : base("language_support_types", igdb, database)
    {
    }
}
public partial class MultiplayerModeCollection : IgdbCollection<MultiplayerMode>
{
    public MultiplayerModeCollection(IgdbManager igdb, Database database) : base("multiplayer_modes", igdb, database)
    {
    }
}
public partial class PlatformCollection : IgdbCollection<Platform>
{
    public PlatformCollection(IgdbManager igdb, Database database) : base("platforms", igdb, database)
    {
    }
}
public partial class PlatformFamilyCollection : IgdbCollection<PlatformFamily>
{
    public PlatformFamilyCollection(IgdbManager igdb, Database database) : base("platform_families", igdb, database)
    {
    }
}
public partial class PlatformLogoCollection : IgdbCollection<PlatformLogo>
{
    public PlatformLogoCollection(IgdbManager igdb, Database database) : base("platform_logos", igdb, database)
    {
    }
}
public partial class PlatformVersionCollection : IgdbCollection<PlatformVersion>
{
    public PlatformVersionCollection(IgdbManager igdb, Database database) : base("platform_versions", igdb, database)
    {
    }
}
public partial class PlatformVersionCompanyCollection : IgdbCollection<PlatformVersionCompany>
{
    public PlatformVersionCompanyCollection(IgdbManager igdb, Database database) : base("platform_version_companies", igdb, database)
    {
    }
}
public partial class PlatformVersionReleaseDateCollection : IgdbCollection<PlatformVersionReleaseDate>
{
    public PlatformVersionReleaseDateCollection(IgdbManager igdb, Database database) : base("platform_version_release_dates", igdb, database)
    {
    }
}
public partial class PlatformWebsiteCollection : IgdbCollection<PlatformWebsite>
{
    public PlatformWebsiteCollection(IgdbManager igdb, Database database) : base("platform_websites", igdb, database)
    {
    }
}
public partial class PlayerPerspectiveCollection : IgdbCollection<PlayerPerspective>
{
    public PlayerPerspectiveCollection(IgdbManager igdb, Database database) : base("player_perspectives", igdb, database)
    {
    }
}
public partial class RegionCollection : IgdbCollection<Region>
{
    public RegionCollection(IgdbManager igdb, Database database) : base("regions", igdb, database)
    {
    }
}
public partial class ReleaseDateCollection : IgdbCollection<ReleaseDate>
{
    public ReleaseDateCollection(IgdbManager igdb, Database database) : base("release_dates", igdb, database)
    {
    }
}
public partial class ReleaseDateStatusCollection : IgdbCollection<ReleaseDateStatus>
{
    public ReleaseDateStatusCollection(IgdbManager igdb, Database database) : base("release_date_statuses", igdb, database)
    {
    }
}
public partial class ScreenshotCollection : IgdbCollection<Screenshot>
{
    public ScreenshotCollection(IgdbManager igdb, Database database) : base("screenshots", igdb, database)
    {
    }
}
public partial class ThemeCollection : IgdbCollection<Theme>
{
    public ThemeCollection(IgdbManager igdb, Database database) : base("themes", igdb, database)
    {
    }
}
public partial class WebsiteCollection : IgdbCollection<Website>
{
    public WebsiteCollection(IgdbManager igdb, Database database) : base("websites", igdb, database)
    {
    }
}
