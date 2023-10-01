using System.Data;
using Core.Features.Movies;
using Dapper;
using Database.Entities;
using Database.TypeHandlers;
using FluentAssertions;
using HumanTime;
using Integrations.TMDbApi;
using Microsoft.Data.Sqlite;
using NSubstitute;

namespace Core.Test;

using Database;

public class MovieFeatureTests
{
    public MovieFeatureTests()
    {
        SqlMapper.AddTypeHandler(new HumanTimeTypeHandler());
        SqlMapper.AddTypeHandler(new CrystalTypeHandler());
    }

    [Fact]
    public async Task TrackMovieFlow_ShouldAddMovieToDatabase()
    {
        using var dbConnection = CreateDatabase();
        
        // Arrange
        var tmdb = Substitute.For<ITMDb>();
        var movies = CreateFakeData();
        
        tmdb.SearchForMoviesAsync("Avatar").Returns(movies);
        tmdb.GetMovieAsync(Arg.Any<int>())!.Returns(args => movies.Find(m => m.Id == args.Arg<int>()));

        // Act
        var searchRequest = new SearchForMovieRequest { Title = "Avatar" };
        var searchResponse = await new SearchForMovieHandler(tmdb).Handle(searchRequest, CancellationToken.None);
            
        var selectedMovie = searchResponse[Random.Shared.Next(searchResponse.Count)];

        var trackRequest = new TrackMovieRequest { TmdbId = selectedMovie.Id, DiscordChannelId = 1234};
        var trackResponse = await new TrackMovieHandler(tmdb, dbConnection).Handle(trackRequest, CancellationToken.None);
            
        // Assert
        trackResponse.Should().BeTrue("Movie should be added to database");

        string query = "SELECT * FROM Movies WHERE TmdbId = @Id";
        var stored = await dbConnection.QueryFirstAsync<MovieRelease?>(query, new { selectedMovie.Id });

        stored.Should().NotBeNull("Movie not stored");
        stored.ChannelId.Should().Be(1234);
        stored.Name.Should().Be(selectedMovie.Title);
        stored.ReleaseTime.Should().BeOfType(typeof(HumanDate));
    }
    
    private IDbConnection CreateDatabase()
    {
        var databaseConnection = new SqliteConnection($"Data Source=:memory:");
        databaseConnection.Open();
        
        MigratorBuilder
            .GetMigrator(databaseConnection, includeTestScripts: true)
            .Build()
            .PerformUpgrade();

        return databaseConnection;
    }

    private List<Movie> CreateFakeData()
    {
        return Enumerable.Range(1, 18).Select(i => new Movie
        (
            Id: (int)Random.Shared.NextInt64(99999),
            Title: $"Avatar {i}",
            Description: Guid.NewGuid().ToString(),
            ReleaseDate.FromDateTime(new DateTime(2023+i, 12, 10))
        )).ToList();
    }
}