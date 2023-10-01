namespace Database.Entities;
using HumanTime;

public class MovieRelease
{
    public Crystal Id { get; set; }
    public int TmdbId { get; set; }
    public long ChannelId { get; set; }
    
    public string Name { get; set; }
    public HumanTime ReleaseTime { get; set; } = HumanTime.Unknown;
}