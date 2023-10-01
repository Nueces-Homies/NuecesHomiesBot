using System.Data;
using System.Data.Common;
using Dapper;
using Database;
using Database.Entities;
using Integrations.TMDbApi;
using MediatR;

namespace Core.Features.Movies;

public class TrackMovieRequest : IRequest<bool>
{
    public required int TmdbId { get; init; }
    public required long DiscordChannelId { get; init; }
}

public class TrackMovieHandler : IRequestHandler<TrackMovieRequest, bool>
{
    private ITMDb TmdbClient { get; }
    private IDbConnection DbConnection { get; }
    
    public TrackMovieHandler(ITMDb tmdbClient, IDbConnection dbConnection)
    {
        this.TmdbClient = tmdbClient;
        this.DbConnection = dbConnection;
    }

    public async Task<bool> Handle(TrackMovieRequest request, CancellationToken cancellationToken)
    {
        var movieInfo = await this.TmdbClient.GetMovieAsync(request.TmdbId);
        
        var movie = new MovieRelease
        {
            Id = new Crystal(),
            TmdbId = request.TmdbId,
            ChannelId = request.DiscordChannelId,
            Name = movieInfo.Title,
            ReleaseTime = movieInfo.ReleaseDate switch
            {
                ReleaseDate.KnownReleaseDate knownDate => HumanTime.HumanTime.Date(knownDate.Date),
                ReleaseDate.UnknownReleaseDate => HumanTime.HumanTime.TBD,
                _ => throw new Exception("Unknown release date type"),
            }
        };

        return await TryInsertMovie(movie);
    }

    private async Task<bool> TryInsertMovie(MovieRelease movie)
    {
        const string querySql = "SELECT * FROM Movies WHERE TmdbId = @TmdbId";
        var existing = await this.DbConnection.QueryFirstOrDefaultAsync<MovieRelease?>(querySql, movie);
        if (existing != null)
        {
            return true;
        }
        
        const string insertSql = """
                INSERT INTO Movies (Id, TmdbId, ChannelId, Name, ReleaseTime) 
                VALUES (@Id, @TmdbId, @ChannelId, @Name, @ReleaseTime);
            """;

        await this.DbConnection.ExecuteAsync(insertSql, movie);
        return true;
    }
}
