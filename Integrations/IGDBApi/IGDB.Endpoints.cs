namespace Integrations.IGDBApi;

public partial class IGDBClient
{
    public QueryBuilder<List<AgeRating>> AgeRatings => new("age_ratings", this);
    public QueryBuilder<List<AgeRatingContentDescription>> AgeRatingContentDescriptions => new("age_ratings_content_descriptions", this);
    public QueryBuilder<List<AlternativeName>> AlternativeNames => new("alternative_names", this);
    public QueryBuilder<List<Artwork>> Artworks => new("artworks", this);
    public QueryBuilder<List<Character>> Characters => new("characters", this);
    public QueryBuilder<List<CharacterMugShot>> CharacterMugShots => new ("character_mugshot", this);
    public QueryBuilder<List<Collection>> Collections => new("collections", this);
    public QueryBuilder<List<Company>> Companies => new ("companies", this);
    public QueryBuilder<List<CompanyLogo>> CompanyLogos => new ("company_logos", this);
    public QueryBuilder<List<CompanyWebsite>> CompanyWebsites => new ("company_websites", this);
    public QueryBuilder<List<Cover>> Covers => new ("covers", this);
    public QueryBuilder<List<ExternalGame>> ExternalGames => new ("external_games", this);
    public QueryBuilder<List<Franchise>> Franchises => new ("franchises", this);
    public QueryBuilder<List<Game>> Games => new ("games", this);
    public QueryBuilder<List<GameEngine>> GameEngines => new ("game_engines", this);
    public QueryBuilder<List<GameEngineLogo>> GameEngineLogos => new ("game_engine_logos", this);
    public QueryBuilder<List<GameLocalization>> GameLocalizations => new ("game_localization", this);
    public QueryBuilder<List<GameMode>> GameModes => new ("game_modes", this);
    public QueryBuilder<List<GameVersion>> GameVersions => new ("game_versions", this);
    public QueryBuilder<List<GameVersionFeature>> GameVersionFeatures => new ("game_version_features", this);
    public QueryBuilder<List<GameVersionFeatureValue>> GameVersionFeatureValues => new ("game_version_feature_values", this);
    public QueryBuilder<List<GameVideo>> GameVideos => new ("game_videos", this);
    public QueryBuilder<List<Genre>> Genres => new ("genres", this);
    public QueryBuilder<List<InvolvedCompany>> InvolvedCompanies => new ("involved_companies", this);
    public QueryBuilder<List<Keyword>> Keywords => new ("keywords", this);
    public QueryBuilder<List<Language>> Languages => new ("languages", this);
    public QueryBuilder<List<LanguageSupport>> LanguageSupports => new ("language_supports", this);
    public QueryBuilder<List<LanguageSupportType>> LanguageSupportTypes => new ("language_support_types", this);
    public QueryBuilder<List<MultiplayerMode>> MultiplayerModes => new ("multiplayer_modes", this);
    public QueryBuilder<List<Platform>> Platforms => new ("platforms", this);
    public QueryBuilder<List<PlatformFamily>> PlatformFamilies => new ("platform_families", this);
    public QueryBuilder<List<PlatformVersion>> PlatformVersions => new ("platform_versions", this);
    public QueryBuilder<List<PlatformVersionCompany>> PlatformVersionCompanies => new ("platform_version_companies", this);
    public QueryBuilder<List<PlatformVersionReleaseDate>> PlatformVersionReleaseDates => new ("platform_version_release_dates", this);
    public QueryBuilder<List<PlatformWebsite>> PlatformWebsites => new ("platform_websites", this);
    public QueryBuilder<List<PlayerPerspective>> PlayerPerspectives => new ("player_perspectives", this);
    public QueryBuilder<List<Region>> Regions => new ("regions", this);
    public QueryBuilder<List<ReleaseDate>> ReleaseDates => new ("release_dates", this);
    public QueryBuilder<List<Screenshot>> Screenshots => new ("screenshots", this);
    public QueryBuilder<List<Theme>> Themes => new ("themes", this);
    public QueryBuilder<List<Website>> Websites => new ("websites", this);
}