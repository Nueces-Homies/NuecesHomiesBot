using System.Data;
using Dapper;
using Database.Entities;
using MediatR;

namespace Core.Features.Movies;

public class GetUpcomingMoviesRequest : IRequest<IEnumerable<MovieRelease>>
{
    public required ulong StartTime { get; init; }
}

public class GetUpcomingMoviesHandler : IRequestHandler<GetUpcomingMoviesRequest, IEnumerable<MovieRelease>>
{
    private readonly IDbConnection dbConnection;

    public GetUpcomingMoviesHandler(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public async Task<IEnumerable<MovieRelease>> Handle(GetUpcomingMoviesRequest request, CancellationToken cancellationToken)
    {
        string selectQuery = "SELECT * FROM Movies WHERE ReleaseTime > @StartTime";
        var movieReleases = await dbConnection.QueryAsync<MovieRelease>(selectQuery, new { request.StartTime });
        return movieReleases;
    }
}
