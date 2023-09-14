namespace Core.Database;

public readonly record struct Crystal (ulong Value)
{
    /// <summary>
    /// Crystal is our take on Discord/Twitter's Snowflake and UUID7. Since we do not operate
    /// at scale and are not distributed, we forgo encoding machine/worker ID (like in Snowflake)
    /// and use only 64-bits (unlike UUID7 which is 128-bit).
    ///
    /// The format of the Crystal is as follows:
    /// Bits 64 to 16: Milliseconds since Unix epoch
    /// Bits 15 to 0: Randomly generated noise
    ///
    /// This allows for randomly generated, but ordered, IDs that we can use as primary keys in our database 
    /// </summary>
    public static Crystal New()
    {
        var milliseconds = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        Span<byte> noiseBytes = stackalloc byte[2];
        Random.Shared.NextBytes(noiseBytes);
        var noise = BitConverter.ToUInt16(noiseBytes);

        return new Crystal((milliseconds << 16) + noise);
    }
}