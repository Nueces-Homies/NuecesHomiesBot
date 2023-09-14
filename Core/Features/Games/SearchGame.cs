using Integrations.IGDBApi;
using MediatR;

using IgdbGame = Integrations.IGDBApi.Game;

namespace Core.Features.Games;

public record SearchGameRequest : IRequest<IList<IgdbGame>>
{
    public required string Title { get; init; }
}

public class SearchGameHandler : IRequestHandler<SearchGameRequest, IList<IgdbGame>>
{
    private readonly IIGDB igdbClient;

    public SearchGameHandler(IIGDB igdbClient)
    {
        this.igdbClient = igdbClient;
    }
    
    public async Task<IList<IgdbGame>> Handle(SearchGameRequest request, CancellationToken cancellationToken)
    {
        var games = await igdbClient.SearchForGamesAsync(request.Title);
        return games.Select(g => new Game()).ToList();
    }
}
