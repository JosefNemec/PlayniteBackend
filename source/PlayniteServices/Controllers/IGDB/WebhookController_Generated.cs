using Microsoft.AspNetCore.Mvc;

namespace Playnite.Backend.IGDB;
[Route("igdb/webhooks/age_ratings")]
public class AgeRatingWebhookController : WebhookController<AgeRating>
{
    public AgeRatingWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("age_ratings", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/age_rating_content_descriptions")]
public class AgeRatingContentDescriptionWebhookController : WebhookController<AgeRatingContentDescription>
{
    public AgeRatingContentDescriptionWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("age_rating_content_descriptions", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/alternative_names")]
public class AlternativeNameWebhookController : WebhookController<AlternativeName>
{
    public AlternativeNameWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("alternative_names", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/artworks")]
public class ArtworkWebhookController : WebhookController<Artwork>
{
    public ArtworkWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("artworks", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/characters")]
public class CharacterWebhookController : WebhookController<Character>
{
    public CharacterWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("characters", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/character_mug_shots")]
public class CharacterMugShotWebhookController : WebhookController<CharacterMugShot>
{
    public CharacterMugShotWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("character_mug_shots", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/collections")]
public class CollectionWebhookController : WebhookController<Collection>
{
    public CollectionWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("collections", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/collection_memberships")]
public class CollectionMembershipWebhookController : WebhookController<CollectionMembership>
{
    public CollectionMembershipWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_memberships", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/collection_membership_types")]
public class CollectionMembershipTypeWebhookController : WebhookController<CollectionMembershipType>
{
    public CollectionMembershipTypeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_membership_types", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/collection_relations")]
public class CollectionRelationWebhookController : WebhookController<CollectionRelation>
{
    public CollectionRelationWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_relations", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/collection_relation_types")]
public class CollectionRelationTypeWebhookController : WebhookController<CollectionRelationType>
{
    public CollectionRelationTypeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_relation_types", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/collection_types")]
public class CollectionTypeWebhookController : WebhookController<CollectionType>
{
    public CollectionTypeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("collection_types", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/companies")]
public class CompanyWebhookController : WebhookController<Company>
{
    public CompanyWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("companies", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/company_logos")]
public class CompanyLogoWebhookController : WebhookController<CompanyLogo>
{
    public CompanyLogoWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("company_logos", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/company_websites")]
public class CompanyWebsiteWebhookController : WebhookController<CompanyWebsite>
{
    public CompanyWebsiteWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("company_websites", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/covers")]
public class CoverWebhookController : WebhookController<Cover>
{
    public CoverWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("covers", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/external_games")]
public class ExternalGameWebhookController : WebhookController<ExternalGame>
{
    public ExternalGameWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("external_games", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/franchises")]
public class FranchiseWebhookController : WebhookController<Franchise>
{
    public FranchiseWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("franchises", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/games")]
public class GameWebhookController : WebhookController<Game>
{
    public GameWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("games", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_engines")]
public class GameEngineWebhookController : WebhookController<GameEngine>
{
    public GameEngineWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_engines", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_engine_logos")]
public class GameEngineLogoWebhookController : WebhookController<GameEngineLogo>
{
    public GameEngineLogoWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_engine_logos", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_localizations")]
public class GameLocalizationWebhookController : WebhookController<GameLocalization>
{
    public GameLocalizationWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_localizations", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_modes")]
public class GameModeWebhookController : WebhookController<GameMode>
{
    public GameModeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_modes", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_time_to_beats")]
public class GameTimeToBeatWebhookController : WebhookController<GameTimeToBeat>
{
    public GameTimeToBeatWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_time_to_beats", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_versions")]
public class GameVersionWebhookController : WebhookController<GameVersion>
{
    public GameVersionWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_versions", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_version_features")]
public class GameVersionFeatureWebhookController : WebhookController<GameVersionFeature>
{
    public GameVersionFeatureWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_version_features", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_version_feature_values")]
public class GameVersionFeatureValueWebhookController : WebhookController<GameVersionFeatureValue>
{
    public GameVersionFeatureValueWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_version_feature_values", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/game_videos")]
public class GameVideoWebhookController : WebhookController<GameVideo>
{
    public GameVideoWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("game_videos", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/genres")]
public class GenreWebhookController : WebhookController<Genre>
{
    public GenreWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("genres", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/involved_companies")]
public class InvolvedCompanyWebhookController : WebhookController<InvolvedCompany>
{
    public InvolvedCompanyWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("involved_companies", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/keywords")]
public class KeywordWebhookController : WebhookController<Keyword>
{
    public KeywordWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("keywords", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/languages")]
public class LanguageWebhookController : WebhookController<Language>
{
    public LanguageWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("languages", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/language_supports")]
public class LanguageSupportWebhookController : WebhookController<LanguageSupport>
{
    public LanguageSupportWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("language_supports", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/language_support_types")]
public class LanguageSupportTypeWebhookController : WebhookController<LanguageSupportType>
{
    public LanguageSupportTypeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("language_support_types", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/multiplayer_modes")]
public class MultiplayerModeWebhookController : WebhookController<MultiplayerMode>
{
    public MultiplayerModeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("multiplayer_modes", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platforms")]
public class PlatformWebhookController : WebhookController<Platform>
{
    public PlatformWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platforms", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platform_families")]
public class PlatformFamilyWebhookController : WebhookController<PlatformFamily>
{
    public PlatformFamilyWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_families", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platform_logos")]
public class PlatformLogoWebhookController : WebhookController<PlatformLogo>
{
    public PlatformLogoWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_logos", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platform_versions")]
public class PlatformVersionWebhookController : WebhookController<PlatformVersion>
{
    public PlatformVersionWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_versions", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platform_version_companies")]
public class PlatformVersionCompanyWebhookController : WebhookController<PlatformVersionCompany>
{
    public PlatformVersionCompanyWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_version_companies", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platform_version_release_dates")]
public class PlatformVersionReleaseDateWebhookController : WebhookController<PlatformVersionReleaseDate>
{
    public PlatformVersionReleaseDateWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_version_release_dates", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/platform_websites")]
public class PlatformWebsiteWebhookController : WebhookController<PlatformWebsite>
{
    public PlatformWebsiteWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("platform_websites", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/player_perspectives")]
public class PlayerPerspectiveWebhookController : WebhookController<PlayerPerspective>
{
    public PlayerPerspectiveWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("player_perspectives", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/regions")]
public class RegionWebhookController : WebhookController<Region>
{
    public RegionWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("regions", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/release_dates")]
public class ReleaseDateWebhookController : WebhookController<ReleaseDate>
{
    public ReleaseDateWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("release_dates", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/release_date_statuses")]
public class ReleaseDateStatusWebhookController : WebhookController<ReleaseDateStatus>
{
    public ReleaseDateStatusWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("release_date_statuses", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/screenshots")]
public class ScreenshotWebhookController : WebhookController<Screenshot>
{
    public ScreenshotWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("screenshots", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/themes")]
public class ThemeWebhookController : WebhookController<Theme>
{
    public ThemeWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("themes", igdb, settings)
    {
    }
}
[Route("igdb/webhooks/websites")]
public class WebsiteWebhookController : WebhookController<Website>
{
    public WebsiteWebhookController(IgdbManager igdb, UpdatableAppSettings settings) : base("websites", igdb, settings)
    {
    }
}
