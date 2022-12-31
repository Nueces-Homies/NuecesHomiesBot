using System.Reflection;
using FluentAssertions;
using Integrations.TMDbApi;
using Microsoft.Extensions.Configuration;

namespace Integrations.Test;

public class TMDbTest
{
    private string tmdbKey;
    
    public TMDbTest()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();
        
        this.tmdbKey = config["TMDB_KEY"] ?? throw new Exception("TMDB_KEY missing");
    }
    
    [Fact(DisplayName = "Lookup a movie in TMDB")]
    public async Task CanLookupMovies()
    {
        var tmdb = new TMDb(this.tmdbKey);
        
        const int blackPanther = 284054;
        Movie movie = await tmdb.GetMovieAsync(blackPanther);

        movie.Title.Should().Be("Black Panther");
        movie.ReleaseDate.Should().BeOfType(typeof(ReleaseDate.KnownReleaseDate));
    }

    [Fact(DisplayName = "Lookup an unreleased movie in TMDB")]
    public async Task CanLookupUnreleasedMovie()
    {
        var tmdb = new TMDb(this.tmdbKey);
        
        const int blackPanther3 = 1037199;
        Movie movie = await tmdb.GetMovieAsync(blackPanther3);

        movie.ReleaseDate.Should().BeOfType(typeof(ReleaseDate.UnknownReleaseDate));
    }
}