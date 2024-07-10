using Microsoft.AspNetCore.Mvc;

namespace PlayniteServices.IGDB;
[Route("igdb/collections/age_ratings")]
public class AgeRatingCollectionController : CollectionController<AgeRating>
{
    public AgeRatingCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("age_ratings", igdb, settings)
    {
    }
}
[Route("igdb/collections/age_rating_content_descriptions")]
public class AgeRatingContentDescriptionCollectionController : CollectionController<AgeRatingContentDescription>
{
    public AgeRatingContentDescriptionCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("age_rating_content_descriptions", igdb, settings)
    {
    }
}
[Route("igdb/collections/alternative_names")]
public class AlternativeNameCollectionController : CollectionController<AlternativeName>
{
    public AlternativeNameCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("alternative_names", igdb, settings)
    {
    }
}
[Route("igdb/collections/artworks")]
public class ArtworkCollectionController : CollectionController<Artwork>
{
    public ArtworkCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("artworks", igdb, settings)
    {
    }
}
[Route("igdb/collections/characters")]
public class CharacterCollectionController : CollectionController<Character>
{
    public CharacterCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("characters", igdb, settings)
    {
    }
}
[Route("igdb/collections/character_mug_shots")]
public class CharacterMugShotCollectionController : CollectionController<CharacterMugShot>
{
    public CharacterMugShotCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("character_mug_shots", igdb, settings)
    {
    }
}
[Route("igdb/collections/collections")]
public class CollectionCollectionController : CollectionController<Collection>
{
    public CollectionCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("collections", igdb, settings)
    {
    }
}
[Route("igdb/collections/collection_memberships")]
public class CollectionMembershipCollectionController : CollectionController<CollectionMembership>
{
    public CollectionMembershipCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_memberships", igdb, settings)
    {
    }
}
[Route("igdb/collections/collection_membership_types")]
public class CollectionMembershipTypeCollectionController : CollectionController<CollectionMembershipType>
{
    public CollectionMembershipTypeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_membership_types", igdb, settings)
    {
    }
}
[Route("igdb/collections/collection_relations")]
public class CollectionRelationCollectionController : CollectionController<CollectionRelation>
{
    public CollectionRelationCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_relations", igdb, settings)
    {
    }
}
[Route("igdb/collections/collection_relation_types")]
public class CollectionRelationTypeCollectionController : CollectionController<CollectionRelationType>
{
    public CollectionRelationTypeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_relation_types", igdb, settings)
    {
    }
}
[Route("igdb/collections/collection_types")]
public class CollectionTypeCollectionController : CollectionController<CollectionType>
{
    public CollectionTypeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_types", igdb, settings)
    {
    }
}
[Route("igdb/collections/companies")]
public class CompanyCollectionController : CollectionController<Company>
{
    public CompanyCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("companies", igdb, settings)
    {
    }
}
[Route("igdb/collections/company_logos")]
public class CompanyLogoCollectionController : CollectionController<CompanyLogo>
{
    public CompanyLogoCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("company_logos", igdb, settings)
    {
    }
}
[Route("igdb/collections/company_websites")]
public class CompanyWebsiteCollectionController : CollectionController<CompanyWebsite>
{
    public CompanyWebsiteCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("company_websites", igdb, settings)
    {
    }
}
[Route("igdb/collections/covers")]
public class CoverCollectionController : CollectionController<Cover>
{
    public CoverCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("covers", igdb, settings)
    {
    }
}
[Route("igdb/collections/external_games")]
public class ExternalGameCollectionController : CollectionController<ExternalGame>
{
    public ExternalGameCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("external_games", igdb, settings)
    {
    }
}
[Route("igdb/collections/franchises")]
public class FranchiseCollectionController : CollectionController<Franchise>
{
    public FranchiseCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("franchises", igdb, settings)
    {
    }
}
[Route("igdb/collections/games")]
public class GameCollectionController : CollectionController<Game>
{
    public GameCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("games", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_engines")]
public class GameEngineCollectionController : CollectionController<GameEngine>
{
    public GameEngineCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_engines", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_engine_logos")]
public class GameEngineLogoCollectionController : CollectionController<GameEngineLogo>
{
    public GameEngineLogoCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_engine_logos", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_localizations")]
public class GameLocalizationCollectionController : CollectionController<GameLocalization>
{
    public GameLocalizationCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_localizations", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_modes")]
public class GameModeCollectionController : CollectionController<GameMode>
{
    public GameModeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_modes", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_versions")]
public class GameVersionCollectionController : CollectionController<GameVersion>
{
    public GameVersionCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_versions", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_version_features")]
public class GameVersionFeatureCollectionController : CollectionController<GameVersionFeature>
{
    public GameVersionFeatureCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_version_features", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_version_feature_values")]
public class GameVersionFeatureValueCollectionController : CollectionController<GameVersionFeatureValue>
{
    public GameVersionFeatureValueCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_version_feature_values", igdb, settings)
    {
    }
}
[Route("igdb/collections/game_videos")]
public class GameVideoCollectionController : CollectionController<GameVideo>
{
    public GameVideoCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_videos", igdb, settings)
    {
    }
}
[Route("igdb/collections/genres")]
public class GenreCollectionController : CollectionController<Genre>
{
    public GenreCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("genres", igdb, settings)
    {
    }
}
[Route("igdb/collections/involved_companies")]
public class InvolvedCompanyCollectionController : CollectionController<InvolvedCompany>
{
    public InvolvedCompanyCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("involved_companies", igdb, settings)
    {
    }
}
[Route("igdb/collections/keywords")]
public class KeywordCollectionController : CollectionController<Keyword>
{
    public KeywordCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("keywords", igdb, settings)
    {
    }
}
[Route("igdb/collections/languages")]
public class LanguageCollectionController : CollectionController<Language>
{
    public LanguageCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("languages", igdb, settings)
    {
    }
}
[Route("igdb/collections/language_supports")]
public class LanguageSupportCollectionController : CollectionController<LanguageSupport>
{
    public LanguageSupportCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("language_supports", igdb, settings)
    {
    }
}
[Route("igdb/collections/language_support_types")]
public class LanguageSupportTypeCollectionController : CollectionController<LanguageSupportType>
{
    public LanguageSupportTypeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("language_support_types", igdb, settings)
    {
    }
}
[Route("igdb/collections/multiplayer_modes")]
public class MultiplayerModeCollectionController : CollectionController<MultiplayerMode>
{
    public MultiplayerModeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("multiplayer_modes", igdb, settings)
    {
    }
}
[Route("igdb/collections/platforms")]
public class PlatformCollectionController : CollectionController<Platform>
{
    public PlatformCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platforms", igdb, settings)
    {
    }
}
[Route("igdb/collections/platform_families")]
public class PlatformFamilyCollectionController : CollectionController<PlatformFamily>
{
    public PlatformFamilyCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_families", igdb, settings)
    {
    }
}
[Route("igdb/collections/platform_logos")]
public class PlatformLogoCollectionController : CollectionController<PlatformLogo>
{
    public PlatformLogoCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_logos", igdb, settings)
    {
    }
}
[Route("igdb/collections/platform_versions")]
public class PlatformVersionCollectionController : CollectionController<PlatformVersion>
{
    public PlatformVersionCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_versions", igdb, settings)
    {
    }
}
[Route("igdb/collections/platform_version_companies")]
public class PlatformVersionCompanyCollectionController : CollectionController<PlatformVersionCompany>
{
    public PlatformVersionCompanyCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_version_companies", igdb, settings)
    {
    }
}
[Route("igdb/collections/platform_version_release_dates")]
public class PlatformVersionReleaseDateCollectionController : CollectionController<PlatformVersionReleaseDate>
{
    public PlatformVersionReleaseDateCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_version_release_dates", igdb, settings)
    {
    }
}
[Route("igdb/collections/platform_websites")]
public class PlatformWebsiteCollectionController : CollectionController<PlatformWebsite>
{
    public PlatformWebsiteCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_websites", igdb, settings)
    {
    }
}
[Route("igdb/collections/player_perspectives")]
public class PlayerPerspectiveCollectionController : CollectionController<PlayerPerspective>
{
    public PlayerPerspectiveCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("player_perspectives", igdb, settings)
    {
    }
}
[Route("igdb/collections/regions")]
public class RegionCollectionController : CollectionController<Region>
{
    public RegionCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("regions", igdb, settings)
    {
    }
}
[Route("igdb/collections/release_dates")]
public class ReleaseDateCollectionController : CollectionController<ReleaseDate>
{
    public ReleaseDateCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("release_dates", igdb, settings)
    {
    }
}
[Route("igdb/collections/release_date_statuses")]
public class ReleaseDateStatusCollectionController : CollectionController<ReleaseDateStatus>
{
    public ReleaseDateStatusCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("release_date_statuses", igdb, settings)
    {
    }
}
[Route("igdb/collections/screenshots")]
public class ScreenshotCollectionController : CollectionController<Screenshot>
{
    public ScreenshotCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("screenshots", igdb, settings)
    {
    }
}
[Route("igdb/collections/themes")]
public class ThemeCollectionController : CollectionController<Theme>
{
    public ThemeCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("themes", igdb, settings)
    {
    }
}
[Route("igdb/collections/websites")]
public class WebsiteCollectionController : CollectionController<Website>
{
    public WebsiteCollectionController(IgdbManager igdb, UpdatableAppSettings settings) : base("websites", igdb, settings)
    {
    }
}
