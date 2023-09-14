namespace Database.Entities;
using HumanTime;

public record Game
{
    public Crystal Id { get; set; }
    public HumanTime ReleaseTime { get; set; } = HumanTime.Unknown;
}