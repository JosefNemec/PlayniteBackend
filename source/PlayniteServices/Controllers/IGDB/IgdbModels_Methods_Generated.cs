using MongoDB.Bson.Serialization;
namespace PlayniteServices.Controllers.IGDB;
public partial class AgeRating : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<AgeRating>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.content_descriptions_expanded);
        });
    }
    public async Task expand_content_descriptions(IgdbApi igdbApi)
    {
        content_descriptions_expanded = await igdbApi.AgeRatingContentDescriptions.GetItem(content_descriptions);
    }
}

public partial class AgeRatingContentDescription : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<AgeRatingContentDescription>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class AlternativeName : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<AlternativeName>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
}

public partial class Artwork : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Artwork>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
}

public partial class Character : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Character>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.games_expanded);
            cm.UnmapProperty(p => p.mug_shot_expanded);
        });
    }
    public async Task expand_games(IgdbApi igdbApi)
    {
        games_expanded = await igdbApi.Games.GetItem(games);
    }
    public async Task expand_mug_shot(IgdbApi igdbApi)
    {
        mug_shot_expanded = await igdbApi.CharacterMugShots.GetItem(mug_shot);
    }
}

public partial class CharacterMugShot : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CharacterMugShot>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class Collection : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Collection>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.games_expanded);
        });
    }
    public async Task expand_games(IgdbApi igdbApi)
    {
        games_expanded = await igdbApi.Games.GetItem(games);
    }
}

public partial class Company : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Company>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.changed_company_id_expanded);
            cm.UnmapProperty(p => p.developed_expanded);
            cm.UnmapProperty(p => p.logo_expanded);
            cm.UnmapProperty(p => p.parent_expanded);
            cm.UnmapProperty(p => p.published_expanded);
            cm.UnmapProperty(p => p.websites_expanded);
        });
    }
    public async Task expand_changed_company_id(IgdbApi igdbApi)
    {
        changed_company_id_expanded = await igdbApi.Companys.GetItem(changed_company_id);
    }
    public async Task expand_developed(IgdbApi igdbApi)
    {
        developed_expanded = await igdbApi.Games.GetItem(developed);
    }
    public async Task expand_logo(IgdbApi igdbApi)
    {
        logo_expanded = await igdbApi.CompanyLogos.GetItem(logo);
    }
    public async Task expand_parent(IgdbApi igdbApi)
    {
        parent_expanded = await igdbApi.Companys.GetItem(parent);
    }
    public async Task expand_published(IgdbApi igdbApi)
    {
        published_expanded = await igdbApi.Games.GetItem(published);
    }
    public async Task expand_websites(IgdbApi igdbApi)
    {
        websites_expanded = await igdbApi.CompanyWebsites.GetItem(websites);
    }
}

public partial class CompanyLogo : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CompanyLogo>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class CompanyWebsite : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CompanyWebsite>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class Cover : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Cover>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.game_localization_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_game_localization(IgdbApi igdbApi)
    {
        game_localization_expanded = await igdbApi.GameLocalizations.GetItem(game_localization);
    }
}

public partial class ExternalGame : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<ExternalGame>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.platform_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_platform(IgdbApi igdbApi)
    {
        platform_expanded = await igdbApi.Platforms.GetItem(platform);
    }
}

public partial class Franchise : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Franchise>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.games_expanded);
        });
    }
    public async Task expand_games(IgdbApi igdbApi)
    {
        games_expanded = await igdbApi.Games.GetItem(games);
    }
}

public partial class Game : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Game>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.age_ratings_expanded);
            cm.UnmapProperty(p => p.alternative_names_expanded);
            cm.UnmapProperty(p => p.artworks_expanded);
            cm.UnmapProperty(p => p.bundles_expanded);
            cm.UnmapProperty(p => p.collection_expanded);
            cm.UnmapProperty(p => p.cover_expanded);
            cm.UnmapProperty(p => p.dlcs_expanded);
            cm.UnmapProperty(p => p.expansions_expanded);
            cm.UnmapProperty(p => p.external_games_expanded);
            cm.UnmapProperty(p => p.franchise_expanded);
            cm.UnmapProperty(p => p.franchises_expanded);
            cm.UnmapProperty(p => p.game_engines_expanded);
            cm.UnmapProperty(p => p.game_modes_expanded);
            cm.UnmapProperty(p => p.genres_expanded);
            cm.UnmapProperty(p => p.involved_companies_expanded);
            cm.UnmapProperty(p => p.keywords_expanded);
            cm.UnmapProperty(p => p.multiplayer_modes_expanded);
            cm.UnmapProperty(p => p.parent_game_expanded);
            cm.UnmapProperty(p => p.platforms_expanded);
            cm.UnmapProperty(p => p.player_perspectives_expanded);
            cm.UnmapProperty(p => p.release_dates_expanded);
            cm.UnmapProperty(p => p.screenshots_expanded);
            cm.UnmapProperty(p => p.similar_games_expanded);
            cm.UnmapProperty(p => p.standalone_expansions_expanded);
            cm.UnmapProperty(p => p.themes_expanded);
            cm.UnmapProperty(p => p.version_parent_expanded);
            cm.UnmapProperty(p => p.videos_expanded);
            cm.UnmapProperty(p => p.websites_expanded);
            cm.UnmapProperty(p => p.remakes_expanded);
            cm.UnmapProperty(p => p.remasters_expanded);
            cm.UnmapProperty(p => p.expanded_games_expanded);
            cm.UnmapProperty(p => p.ports_expanded);
            cm.UnmapProperty(p => p.forks_expanded);
            cm.UnmapProperty(p => p.language_supports_expanded);
            cm.UnmapProperty(p => p.game_localizations_expanded);
        });
    }
    public async Task expand_age_ratings(IgdbApi igdbApi)
    {
        age_ratings_expanded = await igdbApi.AgeRatings.GetItem(age_ratings);
    }
    public async Task expand_alternative_names(IgdbApi igdbApi)
    {
        alternative_names_expanded = await igdbApi.AlternativeNames.GetItem(alternative_names);
    }
    public async Task expand_artworks(IgdbApi igdbApi)
    {
        artworks_expanded = await igdbApi.Artworks.GetItem(artworks);
    }
    public async Task expand_bundles(IgdbApi igdbApi)
    {
        bundles_expanded = await igdbApi.Games.GetItem(bundles);
    }
    public async Task expand_collection(IgdbApi igdbApi)
    {
        collection_expanded = await igdbApi.Collections.GetItem(collection);
    }
    public async Task expand_cover(IgdbApi igdbApi)
    {
        cover_expanded = await igdbApi.Covers.GetItem(cover);
    }
    public async Task expand_dlcs(IgdbApi igdbApi)
    {
        dlcs_expanded = await igdbApi.Games.GetItem(dlcs);
    }
    public async Task expand_expansions(IgdbApi igdbApi)
    {
        expansions_expanded = await igdbApi.Games.GetItem(expansions);
    }
    public async Task expand_external_games(IgdbApi igdbApi)
    {
        external_games_expanded = await igdbApi.ExternalGames.GetItem(external_games);
    }
    public async Task expand_franchise(IgdbApi igdbApi)
    {
        franchise_expanded = await igdbApi.Franchises.GetItem(franchise);
    }
    public async Task expand_franchises(IgdbApi igdbApi)
    {
        franchises_expanded = await igdbApi.Franchises.GetItem(franchises);
    }
    public async Task expand_game_engines(IgdbApi igdbApi)
    {
        game_engines_expanded = await igdbApi.GameEngines.GetItem(game_engines);
    }
    public async Task expand_game_modes(IgdbApi igdbApi)
    {
        game_modes_expanded = await igdbApi.GameModes.GetItem(game_modes);
    }
    public async Task expand_genres(IgdbApi igdbApi)
    {
        genres_expanded = await igdbApi.Genres.GetItem(genres);
    }
    public async Task expand_involved_companies(IgdbApi igdbApi)
    {
        involved_companies_expanded = await igdbApi.InvolvedCompanys.GetItem(involved_companies);
    }
    public async Task expand_keywords(IgdbApi igdbApi)
    {
        keywords_expanded = await igdbApi.Keywords.GetItem(keywords);
    }
    public async Task expand_multiplayer_modes(IgdbApi igdbApi)
    {
        multiplayer_modes_expanded = await igdbApi.MultiplayerModes.GetItem(multiplayer_modes);
    }
    public async Task expand_parent_game(IgdbApi igdbApi)
    {
        parent_game_expanded = await igdbApi.Games.GetItem(parent_game);
    }
    public async Task expand_platforms(IgdbApi igdbApi)
    {
        platforms_expanded = await igdbApi.Platforms.GetItem(platforms);
    }
    public async Task expand_player_perspectives(IgdbApi igdbApi)
    {
        player_perspectives_expanded = await igdbApi.PlayerPerspectives.GetItem(player_perspectives);
    }
    public async Task expand_release_dates(IgdbApi igdbApi)
    {
        release_dates_expanded = await igdbApi.ReleaseDates.GetItem(release_dates);
    }
    public async Task expand_screenshots(IgdbApi igdbApi)
    {
        screenshots_expanded = await igdbApi.Screenshots.GetItem(screenshots);
    }
    public async Task expand_similar_games(IgdbApi igdbApi)
    {
        similar_games_expanded = await igdbApi.Games.GetItem(similar_games);
    }
    public async Task expand_standalone_expansions(IgdbApi igdbApi)
    {
        standalone_expansions_expanded = await igdbApi.Games.GetItem(standalone_expansions);
    }
    public async Task expand_themes(IgdbApi igdbApi)
    {
        themes_expanded = await igdbApi.Themes.GetItem(themes);
    }
    public async Task expand_version_parent(IgdbApi igdbApi)
    {
        version_parent_expanded = await igdbApi.Games.GetItem(version_parent);
    }
    public async Task expand_videos(IgdbApi igdbApi)
    {
        videos_expanded = await igdbApi.GameVideos.GetItem(videos);
    }
    public async Task expand_websites(IgdbApi igdbApi)
    {
        websites_expanded = await igdbApi.Websites.GetItem(websites);
    }
    public async Task expand_remakes(IgdbApi igdbApi)
    {
        remakes_expanded = await igdbApi.Games.GetItem(remakes);
    }
    public async Task expand_remasters(IgdbApi igdbApi)
    {
        remasters_expanded = await igdbApi.Games.GetItem(remasters);
    }
    public async Task expand_expanded_games(IgdbApi igdbApi)
    {
        expanded_games_expanded = await igdbApi.Games.GetItem(expanded_games);
    }
    public async Task expand_ports(IgdbApi igdbApi)
    {
        ports_expanded = await igdbApi.Games.GetItem(ports);
    }
    public async Task expand_forks(IgdbApi igdbApi)
    {
        forks_expanded = await igdbApi.Games.GetItem(forks);
    }
    public async Task expand_language_supports(IgdbApi igdbApi)
    {
        language_supports_expanded = await igdbApi.LanguageSupports.GetItem(language_supports);
    }
    public async Task expand_game_localizations(IgdbApi igdbApi)
    {
        game_localizations_expanded = await igdbApi.GameLocalizations.GetItem(game_localizations);
    }
}

public partial class GameEngine : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameEngine>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.companies_expanded);
            cm.UnmapProperty(p => p.logo_expanded);
            cm.UnmapProperty(p => p.platforms_expanded);
        });
    }
    public async Task expand_companies(IgdbApi igdbApi)
    {
        companies_expanded = await igdbApi.Companys.GetItem(companies);
    }
    public async Task expand_logo(IgdbApi igdbApi)
    {
        logo_expanded = await igdbApi.GameEngineLogos.GetItem(logo);
    }
    public async Task expand_platforms(IgdbApi igdbApi)
    {
        platforms_expanded = await igdbApi.Platforms.GetItem(platforms);
    }
}

public partial class GameEngineLogo : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameEngineLogo>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class GameLocalization : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameLocalization>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.cover_expanded);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.region_expanded);
        });
    }
    public async Task expand_cover(IgdbApi igdbApi)
    {
        cover_expanded = await igdbApi.Covers.GetItem(cover);
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_region(IgdbApi igdbApi)
    {
        region_expanded = await igdbApi.Regions.GetItem(region);
    }
}

public partial class GameMode : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameMode>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class GameVersion : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameVersion>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.features_expanded);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.games_expanded);
        });
    }
    public async Task expand_features(IgdbApi igdbApi)
    {
        features_expanded = await igdbApi.GameVersionFeatures.GetItem(features);
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_games(IgdbApi igdbApi)
    {
        games_expanded = await igdbApi.Games.GetItem(games);
    }
}

public partial class GameVersionFeature : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameVersionFeature>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.values_expanded);
        });
    }
    public async Task expand_values(IgdbApi igdbApi)
    {
        values_expanded = await igdbApi.GameVersionFeatureValues.GetItem(values);
    }
}

public partial class GameVersionFeatureValue : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameVersionFeatureValue>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.game_feature_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_game_feature(IgdbApi igdbApi)
    {
        game_feature_expanded = await igdbApi.GameVersionFeatures.GetItem(game_feature);
    }
}

public partial class GameVideo : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameVideo>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
}

public partial class Genre : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Genre>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class InvolvedCompany : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<InvolvedCompany>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.company_expanded);
            cm.UnmapProperty(p => p.game_expanded);
        });
    }
    public async Task expand_company(IgdbApi igdbApi)
    {
        company_expanded = await igdbApi.Companys.GetItem(company);
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
}

public partial class Keyword : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Keyword>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class Language : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Language>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class LanguageSupport : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<LanguageSupport>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.language_expanded);
            cm.UnmapProperty(p => p.language_support_type_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_language(IgdbApi igdbApi)
    {
        language_expanded = await igdbApi.Languages.GetItem(language);
    }
    public async Task expand_language_support_type(IgdbApi igdbApi)
    {
        language_support_type_expanded = await igdbApi.LanguageSupportTypes.GetItem(language_support_type);
    }
}

public partial class LanguageSupportType : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<LanguageSupportType>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class MultiplayerMode : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<MultiplayerMode>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.platform_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_platform(IgdbApi igdbApi)
    {
        platform_expanded = await igdbApi.Platforms.GetItem(platform);
    }
}

public partial class Platform : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Platform>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.platform_logo_expanded);
            cm.UnmapProperty(p => p.platform_family_expanded);
            cm.UnmapProperty(p => p.versions_expanded);
            cm.UnmapProperty(p => p.websites_expanded);
        });
    }
    public async Task expand_platform_logo(IgdbApi igdbApi)
    {
        platform_logo_expanded = await igdbApi.PlatformLogos.GetItem(platform_logo);
    }
    public async Task expand_platform_family(IgdbApi igdbApi)
    {
        platform_family_expanded = await igdbApi.PlatformFamilys.GetItem(platform_family);
    }
    public async Task expand_versions(IgdbApi igdbApi)
    {
        versions_expanded = await igdbApi.PlatformVersions.GetItem(versions);
    }
    public async Task expand_websites(IgdbApi igdbApi)
    {
        websites_expanded = await igdbApi.PlatformWebsites.GetItem(websites);
    }
}

public partial class PlatformFamily : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlatformFamily>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class PlatformLogo : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlatformLogo>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class PlatformVersion : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlatformVersion>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.companies_expanded);
            cm.UnmapProperty(p => p.main_manufacturer_expanded);
            cm.UnmapProperty(p => p.platform_logo_expanded);
            cm.UnmapProperty(p => p.platform_version_release_dates_expanded);
        });
    }
    public async Task expand_companies(IgdbApi igdbApi)
    {
        companies_expanded = await igdbApi.PlatformVersionCompanys.GetItem(companies);
    }
    public async Task expand_main_manufacturer(IgdbApi igdbApi)
    {
        main_manufacturer_expanded = await igdbApi.PlatformVersionCompanys.GetItem(main_manufacturer);
    }
    public async Task expand_platform_logo(IgdbApi igdbApi)
    {
        platform_logo_expanded = await igdbApi.PlatformLogos.GetItem(platform_logo);
    }
    public async Task expand_platform_version_release_dates(IgdbApi igdbApi)
    {
        platform_version_release_dates_expanded = await igdbApi.PlatformVersionReleaseDates.GetItem(platform_version_release_dates);
    }
}

public partial class PlatformVersionCompany : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlatformVersionCompany>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.company_expanded);
        });
    }
    public async Task expand_company(IgdbApi igdbApi)
    {
        company_expanded = await igdbApi.Companys.GetItem(company);
    }
}

public partial class PlatformVersionReleaseDate : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlatformVersionReleaseDate>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.platform_version_expanded);
        });
    }
    public async Task expand_platform_version(IgdbApi igdbApi)
    {
        platform_version_expanded = await igdbApi.PlatformVersions.GetItem(platform_version);
    }
}

public partial class PlatformWebsite : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlatformWebsite>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class PlayerPerspective : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<PlayerPerspective>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class Region : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Region>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class ReleaseDate : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<ReleaseDate>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.platform_expanded);
            cm.UnmapProperty(p => p.status_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
    public async Task expand_platform(IgdbApi igdbApi)
    {
        platform_expanded = await igdbApi.Platforms.GetItem(platform);
    }
    public async Task expand_status(IgdbApi igdbApi)
    {
        status_expanded = await igdbApi.ReleaseDateStatuss.GetItem(status);
    }
}

public partial class ReleaseDateStatus : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<ReleaseDateStatus>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class Screenshot : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Screenshot>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
}

public partial class Theme : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Theme>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
    }
}

public partial class Website : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<Website>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
        });
    }
    public async Task expand_game(IgdbApi igdbApi)
    {
        game_expanded = await igdbApi.Games.GetItem(game);
    }
}

