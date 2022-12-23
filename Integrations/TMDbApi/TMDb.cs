using System.Collections.Immutable;
using TMDbLib.Client;


namespace Integrations.TMDbApi;

public interface ITMDb
{
    Task<Movie> GetMovieAsync(int tmdbId);
    Task<IList<Movie>> SearchForMoviesAsync(string query);
}


public class TMDb : ITMDb
{
    private TMDbClient client;

    public TMDb(string apiKey)
    {
        this.client = new TMDbClient(apiKey);
    }

    public async Task<Movie> GetMovieAsync(int tmdbId)
    {
        var result = await this.client.GetMovieAsync(tmdbId);
        return result.ToMovie();
    }

    public async Task<IList<Movie>> SearchForMoviesAsync(string query)
    {
        var searchResults = await this.client.SearchMovieAsync(query, includeAdult: false);
        return searchResults.Results.Select(m => m.ToMovie()).ToImmutableArray();
    }
}
