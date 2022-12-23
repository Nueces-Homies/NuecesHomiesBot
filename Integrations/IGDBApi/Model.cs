namespace Integrations.IGDBApi;

public partial class AgeRatingResult
{
    public static implicit operator List<AgeRating>(AgeRatingResult result) => result.Ageratings;
}

public partial class AgeRatingContentDescriptionResult
{
    public static implicit operator List<AgeRatingContentDescription>(AgeRatingContentDescriptionResult result) =>
        result.Ageratingcontentdescriptions;
}

public partial class AlternativeNameResult
{
    public static implicit operator List<AlternativeName>(AlternativeNameResult result) => result.Alternativenames;
}

public partial class ArtworkResult
{
    public static implicit operator List<Artwork>(ArtworkResult result) => result.Artworks;
}

public partial class CharacterResult
{
    public static implicit operator List<Character>(CharacterResult result) => result.Characters;
}

public partial class CharacterMugShotResult
{
    public static implicit operator List<CharacterMugShot>(CharacterMugShotResult result) => result.Charactermugshots;
}

public partial class CollectionResult
{
    public static implicit operator List<Collection>(CollectionResult result) => result.Collections;
}

public partial class CompanyResult
{
    public static implicit operator List<Company>(CompanyResult result) => result.Companies;
}

public partial class CompanyLogoResult
{
    public static implicit operator List<CompanyLogo>(CompanyLogoResult result) => result.Companylogos;
}

public partial class CompanyWebsiteResult
{
    public static implicit operator List<CompanyWebsite>(CompanyWebsiteResult result) => result.Companywebsites;
}

public partial class CoverResult
{
    public static implicit operator List<Cover>(CoverResult result) => result.Covers;
}

public partial class ExternalGameResult
{
    public static implicit operator List<ExternalGame>(ExternalGameResult result) => result.Externalgames;
}

public partial class FranchiseResult
{
    public static implicit operator List<Franchise>(FranchiseResult result) => result.Franchises;
}

public partial class GameResult
{
    public static implicit operator List<Game>(GameResult result) => result.Games;
}

public partial class GameEngineResult
{
    public static implicit operator List<GameEngine>(GameEngineResult result) => result.Gameengines;
}

public partial class GameEngineLogoResult
{
    public static implicit operator List<GameEngineLogo>(GameEngineLogoResult result) => result.Gameenginelogos;
}

public partial class GameLocalizationResult
{
    public static implicit operator List<GameLocalization>(GameLocalizationResult result) => result.Gamelocalizations;
}

public partial class GameModeResult
{
    public static implicit operator List<GameMode>(GameModeResult result) => result.Gamemodes;
}

public partial class GameVersionResult
{
    public static implicit operator List<GameVersion>(GameVersionResult result) => result.Gameversions;
}

public partial class GameVersionFeatureResult
{
    public static implicit operator List<GameVersionFeature>(GameVersionFeatureResult result) =>
        result.Gameversionfeatures;
}

public partial class GameVideoResult
{
    public static implicit operator List<GameVideo>(GameVideoResult result) => result.Gamevideos;
}

public partial class GenreResult
{
    public static implicit operator List<Genre>(GenreResult result) => result.Genres;
}

public partial class LanguageResult
{
    public static implicit operator List<Language>(LanguageResult result) => result.Languages;
}

public partial class LanguageSupportResult
{
    public static implicit operator List<LanguageSupport>(LanguageSupportResult result) => result.Languagesupports;
}

public partial class LanguageSupportTypeResult
{
    public static implicit operator List<LanguageSupportType>(LanguageSupportTypeResult result) =>
        result.Languagesupporttypes;
}

public partial class PlatformResult
{
    public static implicit operator List<Platform>(PlatformResult result) => result.Platforms;
}

public partial class PlatformVersionResult
{
    public static implicit operator List<PlatformVersion>(PlatformVersionResult result) => result.Platformversions;
}

public partial class PlatformVersionCompanyResult
{
    public static implicit operator List<PlatformVersionCompany>(PlatformVersionCompanyResult result) =>
        result.Platformversioncompanies;
}

public partial class PlatformVersionReleaseDateResult
{
    public static implicit operator List<PlatformVersionReleaseDate>(PlatformVersionReleaseDateResult result) =>
        result.Platformversionreleasedates;
}

public partial class PlatformWebsiteResult
{
    public static implicit operator List<PlatformWebsite>(PlatformWebsiteResult result) => result.Platformwebsites;
}

public partial class PlayerPerspectiveResult
{
    public static implicit operator List<PlayerPerspective>(PlayerPerspectiveResult result) =>
        result.Playerperspectives;
}

public partial class RegionResult
{
    public static implicit operator List<Region>(RegionResult result) => result.Regions;
}

public partial class ReleaseDateResult
{
    public static implicit operator List<ReleaseDate>(ReleaseDateResult result) => result.Releasedates;
}

public partial class ScreenshotResult
{
    public static implicit operator List<Screenshot>(ScreenshotResult result) => result.Screenshots;
}

public partial class SearchResult
{
    public static implicit operator List<Search>(SearchResult result) => result.Searches;
}

public partial class ThemeResult
{
    public static implicit operator List<Theme>(ThemeResult result) => result.Themes;
}

public partial class WebsiteResult
{
    public static implicit operator List<Website>(WebsiteResult result) => result.Websites;
}
