using Integrations.TMDbApi;
using MediatR;

namespace Core.Features.Movies;

public class MovieSearchResult
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required int? Year { get; init; }
}

public class SearchForMovieRequest : IRequest<List<MovieSearchResult>>
{
    public required string Title { get; init; }
}


public class SearchForMovieHandler : IRequestHandler<SearchForMovieRequest, List<MovieSearchResult>>
{
    private ITMDb TmdbClient { get; }

    public SearchForMovieHandler(ITMDb tmdbClient)
    {
        TmdbClient = tmdbClient;
    }
    
    public async Task<List<MovieSearchResult>> Handle(SearchForMovieRequest request, CancellationToken cancellationToken)
    {
        var results = await TmdbClient.SearchForMoviesAsync(request.Title);
        return results.Select(m => new MovieSearchResult
        {
            Id = m.Id,
            Title = m.Title,
            Year = GetYear(m.ReleaseDate),
        }).ToList();
    }

    private int? GetYear(ReleaseDate releaseDate)
    {
        return releaseDate switch
        {
            ReleaseDate.KnownReleaseDate knownDate => knownDate.Date.Year,
            ReleaseDate.UnknownReleaseDate => null,
            _ => throw new Exception("Unknown release date type")
        };
    }
}