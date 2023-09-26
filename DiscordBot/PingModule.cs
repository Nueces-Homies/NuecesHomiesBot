using Core.Features;
using Discord.Interactions;
using MediatR;

namespace DiscordBot;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    private IMediator mediator;

    public PingModule(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [SlashCommand("nhb-ping", "Pings the bot")]
    public async Task PingCommand(string message)
    {
        var request = new PingRequest { Message = message };
        var response = await this.mediator.Send(request);
        await RespondAsync(text: response, ephemeral: true);
    }
}