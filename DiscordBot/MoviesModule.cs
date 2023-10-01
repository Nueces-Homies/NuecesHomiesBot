using Core.Features.Movies;
using Discord.Interactions;
using MediatR;

namespace DiscordBot;

[Group("games", "Track games")]
public class MoviesModule : InteractionModuleBase<SocketInteractionContext>
{
    private IMediator mediator;

    public MoviesModule(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [SlashCommand("track", "Tracks a movie")]
    public async Task TrackCommand(string title)
    {
        var request = new SearchForMovieRequest { Title = title };
        var response = await this.mediator.Send(request);
    }
}
