namespace Integrations.IGDBApi;

public partial struct IGDBTimestamp
{
    private static readonly DateTime TimestampEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private static readonly long FutureSeconds = 32534605233;
    
    public DateTime AsDateTime()
    {
        if (Seconds > IGDBTimestamp.FutureSeconds)
        {
            // IGDB will sometimes send milliseconds
            Seconds /= 1000;
        }

        return IGDBTimestamp.TimestampEpoch.AddSeconds(Seconds);
    }
        
    public static implicit operator DateTime(IGDBTimestamp value) => value.AsDateTime();
}