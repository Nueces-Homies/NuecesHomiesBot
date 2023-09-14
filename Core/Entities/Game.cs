using Core.Common.HumanTime;
using Core.Database;

namespace Core.Entities;

public record Game
{
    public Crystal Id { get; init; }
    public ulong IgdbId { get; init; }
    
    public ulong DiscordChannelId { get; set; }
    public HumanTime ReleaseTime { get; set; } = HumanTime.Unknown;
}