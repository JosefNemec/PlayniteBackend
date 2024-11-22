using MongoDB.Bson.Serialization;
namespace PlayniteServices.IGDB;
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
    public async Task expand_content_descriptions(IgdbManager igdb)
    {
        content_descriptions_expanded = await igdb.AgeRatingContentDescriptions.GetItem(content_descriptions);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
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
    public async Task expand_games(IgdbManager igdb)
    {
        games_expanded = await igdb.Games.GetItem(games);
    }
    public async Task expand_mug_shot(IgdbManager igdb)
    {
        mug_shot_expanded = await igdb.CharacterMugShots.GetItem(mug_shot);
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
            cm.UnmapProperty(p => p.type_expanded);
            cm.UnmapProperty(p => p.as_parent_relations_expanded);
            cm.UnmapProperty(p => p.as_child_relations_expanded);
        });
    }
    public async Task expand_games(IgdbManager igdb)
    {
        games_expanded = await igdb.Games.GetItem(games);
    }
    public async Task expand_type(IgdbManager igdb)
    {
        type_expanded = await igdb.CollectionTypes.GetItem(type);
    }
    public async Task expand_as_parent_relations(IgdbManager igdb)
    {
        as_parent_relations_expanded = await igdb.CollectionRelations.GetItem(as_parent_relations);
    }
    public async Task expand_as_child_relations(IgdbManager igdb)
    {
        as_child_relations_expanded = await igdb.CollectionRelations.GetItem(as_child_relations);
    }
}

public partial class CollectionMembership : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CollectionMembership>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.game_expanded);
            cm.UnmapProperty(p => p.collection_expanded);
            cm.UnmapProperty(p => p.type_expanded);
        });
    }
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_collection(IgdbManager igdb)
    {
        collection_expanded = await igdb.Collections.GetItem(collection);
    }
    public async Task expand_type(IgdbManager igdb)
    {
        type_expanded = await igdb.CollectionMembershipTypes.GetItem(type);
    }
}

public partial class CollectionMembershipType : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CollectionMembershipType>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.allowed_collection_type_expanded);
        });
    }
    public async Task expand_allowed_collection_type(IgdbManager igdb)
    {
        allowed_collection_type_expanded = await igdb.CollectionTypes.GetItem(allowed_collection_type);
    }
}

public partial class CollectionRelation : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CollectionRelation>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.child_collection_expanded);
            cm.UnmapProperty(p => p.parent_collection_expanded);
            cm.UnmapProperty(p => p.type_expanded);
        });
    }
    public async Task expand_child_collection(IgdbManager igdb)
    {
        child_collection_expanded = await igdb.Collections.GetItem(child_collection);
    }
    public async Task expand_parent_collection(IgdbManager igdb)
    {
        parent_collection_expanded = await igdb.Collections.GetItem(parent_collection);
    }
    public async Task expand_type(IgdbManager igdb)
    {
        type_expanded = await igdb.CollectionRelationTypes.GetItem(type);
    }
}

public partial class CollectionRelationType : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CollectionRelationType>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);
            cm.UnmapProperty(p => p.allowed_child_type_expanded);
            cm.UnmapProperty(p => p.allowed_parent_type_expanded);
        });
    }
    public async Task expand_allowed_child_type(IgdbManager igdb)
    {
        allowed_child_type_expanded = await igdb.CollectionTypes.GetItem(allowed_child_type);
    }
    public async Task expand_allowed_parent_type(IgdbManager igdb)
    {
        allowed_parent_type_expanded = await igdb.CollectionTypes.GetItem(allowed_parent_type);
    }
}

public partial class CollectionType : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<CollectionType>(cm => {
            cm.AutoMap();
            cm.MapIdMember(p => p.id);
            cm.SetIgnoreExtraElements(true);

        });
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
    public async Task expand_changed_company_id(IgdbManager igdb)
    {
        changed_company_id_expanded = await igdb.Companys.GetItem(changed_company_id);
    }
    public async Task expand_developed(IgdbManager igdb)
    {
        developed_expanded = await igdb.Games.GetItem(developed);
    }
    public async Task expand_logo(IgdbManager igdb)
    {
        logo_expanded = await igdb.CompanyLogos.GetItem(logo);
    }
    public async Task expand_parent(IgdbManager igdb)
    {
        parent_expanded = await igdb.Companys.GetItem(parent);
    }
    public async Task expand_published(IgdbManager igdb)
    {
        published_expanded = await igdb.Games.GetItem(published);
    }
    public async Task expand_websites(IgdbManager igdb)
    {
        websites_expanded = await igdb.CompanyWebsites.GetItem(websites);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_game_localization(IgdbManager igdb)
    {
        game_localization_expanded = await igdb.GameLocalizations.GetItem(game_localization);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_platform(IgdbManager igdb)
    {
        platform_expanded = await igdb.Platforms.GetItem(platform);
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
    public async Task expand_games(IgdbManager igdb)
    {
        games_expanded = await igdb.Games.GetItem(games);
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
            cm.UnmapProperty(p => p.collections_expanded);
        });
    }
    public async Task expand_age_ratings(IgdbManager igdb)
    {
        age_ratings_expanded = await igdb.AgeRatings.GetItem(age_ratings);
    }
    public async Task expand_alternative_names(IgdbManager igdb)
    {
        alternative_names_expanded = await igdb.AlternativeNames.GetItem(alternative_names);
    }
    public async Task expand_artworks(IgdbManager igdb)
    {
        artworks_expanded = await igdb.Artworks.GetItem(artworks);
    }
    public async Task expand_bundles(IgdbManager igdb)
    {
        bundles_expanded = await igdb.Games.GetItem(bundles);
    }
    public async Task expand_cover(IgdbManager igdb)
    {
        cover_expanded = await igdb.Covers.GetItem(cover);
    }
    public async Task expand_dlcs(IgdbManager igdb)
    {
        dlcs_expanded = await igdb.Games.GetItem(dlcs);
    }
    public async Task expand_expansions(IgdbManager igdb)
    {
        expansions_expanded = await igdb.Games.GetItem(expansions);
    }
    public async Task expand_external_games(IgdbManager igdb)
    {
        external_games_expanded = await igdb.ExternalGames.GetItem(external_games);
    }
    public async Task expand_franchise(IgdbManager igdb)
    {
        franchise_expanded = await igdb.Franchises.GetItem(franchise);
    }
    public async Task expand_franchises(IgdbManager igdb)
    {
        franchises_expanded = await igdb.Franchises.GetItem(franchises);
    }
    public async Task expand_game_engines(IgdbManager igdb)
    {
        game_engines_expanded = await igdb.GameEngines.GetItem(game_engines);
    }
    public async Task expand_game_modes(IgdbManager igdb)
    {
        game_modes_expanded = await igdb.GameModes.GetItem(game_modes);
    }
    public async Task expand_genres(IgdbManager igdb)
    {
        genres_expanded = await igdb.Genres.GetItem(genres);
    }
    public async Task expand_involved_companies(IgdbManager igdb)
    {
        involved_companies_expanded = await igdb.InvolvedCompanys.GetItem(involved_companies);
    }
    public async Task expand_keywords(IgdbManager igdb)
    {
        keywords_expanded = await igdb.Keywords.GetItem(keywords);
    }
    public async Task expand_multiplayer_modes(IgdbManager igdb)
    {
        multiplayer_modes_expanded = await igdb.MultiplayerModes.GetItem(multiplayer_modes);
    }
    public async Task expand_parent_game(IgdbManager igdb)
    {
        parent_game_expanded = await igdb.Games.GetItem(parent_game);
    }
    public async Task expand_platforms(IgdbManager igdb)
    {
        platforms_expanded = await igdb.Platforms.GetItem(platforms);
    }
    public async Task expand_player_perspectives(IgdbManager igdb)
    {
        player_perspectives_expanded = await igdb.PlayerPerspectives.GetItem(player_perspectives);
    }
    public async Task expand_release_dates(IgdbManager igdb)
    {
        release_dates_expanded = await igdb.ReleaseDates.GetItem(release_dates);
    }
    public async Task expand_screenshots(IgdbManager igdb)
    {
        screenshots_expanded = await igdb.Screenshots.GetItem(screenshots);
    }
    public async Task expand_similar_games(IgdbManager igdb)
    {
        similar_games_expanded = await igdb.Games.GetItem(similar_games);
    }
    public async Task expand_standalone_expansions(IgdbManager igdb)
    {
        standalone_expansions_expanded = await igdb.Games.GetItem(standalone_expansions);
    }
    public async Task expand_themes(IgdbManager igdb)
    {
        themes_expanded = await igdb.Themes.GetItem(themes);
    }
    public async Task expand_version_parent(IgdbManager igdb)
    {
        version_parent_expanded = await igdb.Games.GetItem(version_parent);
    }
    public async Task expand_videos(IgdbManager igdb)
    {
        videos_expanded = await igdb.GameVideos.GetItem(videos);
    }
    public async Task expand_websites(IgdbManager igdb)
    {
        websites_expanded = await igdb.Websites.GetItem(websites);
    }
    public async Task expand_remakes(IgdbManager igdb)
    {
        remakes_expanded = await igdb.Games.GetItem(remakes);
    }
    public async Task expand_remasters(IgdbManager igdb)
    {
        remasters_expanded = await igdb.Games.GetItem(remasters);
    }
    public async Task expand_expanded_games(IgdbManager igdb)
    {
        expanded_games_expanded = await igdb.Games.GetItem(expanded_games);
    }
    public async Task expand_ports(IgdbManager igdb)
    {
        ports_expanded = await igdb.Games.GetItem(ports);
    }
    public async Task expand_forks(IgdbManager igdb)
    {
        forks_expanded = await igdb.Games.GetItem(forks);
    }
    public async Task expand_language_supports(IgdbManager igdb)
    {
        language_supports_expanded = await igdb.LanguageSupports.GetItem(language_supports);
    }
    public async Task expand_game_localizations(IgdbManager igdb)
    {
        game_localizations_expanded = await igdb.GameLocalizations.GetItem(game_localizations);
    }
    public async Task expand_collections(IgdbManager igdb)
    {
        collections_expanded = await igdb.Collections.GetItem(collections);
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
    public async Task expand_companies(IgdbManager igdb)
    {
        companies_expanded = await igdb.Companys.GetItem(companies);
    }
    public async Task expand_logo(IgdbManager igdb)
    {
        logo_expanded = await igdb.GameEngineLogos.GetItem(logo);
    }
    public async Task expand_platforms(IgdbManager igdb)
    {
        platforms_expanded = await igdb.Platforms.GetItem(platforms);
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
    public async Task expand_cover(IgdbManager igdb)
    {
        cover_expanded = await igdb.Covers.GetItem(cover);
    }
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_region(IgdbManager igdb)
    {
        region_expanded = await igdb.Regions.GetItem(region);
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

public partial class GameTimeToBeat : IIgdbItem
{
    public static void RegisterClassMap()
    {
        BsonClassMap.RegisterClassMap<GameTimeToBeat>(cm => {
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
    public async Task expand_features(IgdbManager igdb)
    {
        features_expanded = await igdb.GameVersionFeatures.GetItem(features);
    }
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_games(IgdbManager igdb)
    {
        games_expanded = await igdb.Games.GetItem(games);
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
    public async Task expand_values(IgdbManager igdb)
    {
        values_expanded = await igdb.GameVersionFeatureValues.GetItem(values);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_game_feature(IgdbManager igdb)
    {
        game_feature_expanded = await igdb.GameVersionFeatures.GetItem(game_feature);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
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
    public async Task expand_company(IgdbManager igdb)
    {
        company_expanded = await igdb.Companys.GetItem(company);
    }
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_language(IgdbManager igdb)
    {
        language_expanded = await igdb.Languages.GetItem(language);
    }
    public async Task expand_language_support_type(IgdbManager igdb)
    {
        language_support_type_expanded = await igdb.LanguageSupportTypes.GetItem(language_support_type);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_platform(IgdbManager igdb)
    {
        platform_expanded = await igdb.Platforms.GetItem(platform);
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
    public async Task expand_platform_logo(IgdbManager igdb)
    {
        platform_logo_expanded = await igdb.PlatformLogos.GetItem(platform_logo);
    }
    public async Task expand_platform_family(IgdbManager igdb)
    {
        platform_family_expanded = await igdb.PlatformFamilys.GetItem(platform_family);
    }
    public async Task expand_versions(IgdbManager igdb)
    {
        versions_expanded = await igdb.PlatformVersions.GetItem(versions);
    }
    public async Task expand_websites(IgdbManager igdb)
    {
        websites_expanded = await igdb.PlatformWebsites.GetItem(websites);
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
    public async Task expand_companies(IgdbManager igdb)
    {
        companies_expanded = await igdb.PlatformVersionCompanys.GetItem(companies);
    }
    public async Task expand_main_manufacturer(IgdbManager igdb)
    {
        main_manufacturer_expanded = await igdb.PlatformVersionCompanys.GetItem(main_manufacturer);
    }
    public async Task expand_platform_logo(IgdbManager igdb)
    {
        platform_logo_expanded = await igdb.PlatformLogos.GetItem(platform_logo);
    }
    public async Task expand_platform_version_release_dates(IgdbManager igdb)
    {
        platform_version_release_dates_expanded = await igdb.PlatformVersionReleaseDates.GetItem(platform_version_release_dates);
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
    public async Task expand_company(IgdbManager igdb)
    {
        company_expanded = await igdb.Companys.GetItem(company);
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
    public async Task expand_platform_version(IgdbManager igdb)
    {
        platform_version_expanded = await igdb.PlatformVersions.GetItem(platform_version);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
    public async Task expand_platform(IgdbManager igdb)
    {
        platform_expanded = await igdb.Platforms.GetItem(platform);
    }
    public async Task expand_status(IgdbManager igdb)
    {
        status_expanded = await igdb.ReleaseDateStatuss.GetItem(status);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
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
    public async Task expand_game(IgdbManager igdb)
    {
        game_expanded = await igdb.Games.GetItem(game);
    }
}

