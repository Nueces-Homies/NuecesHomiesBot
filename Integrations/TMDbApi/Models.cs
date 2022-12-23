namespace Integrations.TMDbApi;
public record Movie(int Id, string Title, string Description, ReleaseDate ReleaseDate);
public abstract class ReleaseDate
{
    public static ReleaseDate FromDateTime(DateTime? dateTime)
    {
        return dateTime == null ? new UnknownReleaseDate() : new KnownReleaseDate(dateTime.Value);
    }
    public class KnownReleaseDate : ReleaseDate
    {
        public DateOnly Date { get; }

        public KnownReleaseDate(DateTime date)
        {
            this.Date = DateOnly.FromDateTime(date);
        }
    }

    public class UnknownReleaseDate : ReleaseDate
    {
    }
}

internal static class Utils
{
    public static Movie ToMovie(this TMDbLib.Objects.Movies.Movie movie)
    {
        return new Movie(movie.Id, movie.Title, movie.Overview, ReleaseDate.FromDateTime(movie.ReleaseDate));
    }
    
    public static Movie ToMovie(this TMDbLib.Objects.Search.SearchMovie movie)
    {
        return new Movie(movie.Id, movie.Title, movie.Overview, ReleaseDate.FromDateTime(movie.ReleaseDate));
    }
}