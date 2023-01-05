namespace HumanTimeParser;

internal class TimeZoneCache
{
    private readonly Dictionary<string, TimeZoneInfo> cache = new();
    
    public TimeZoneCache()
    {
        this.SetAll("America/Los_Angeles", "PT", "PST", "PDT");
        this.SetAll("America/Denver", "MT", "MST", "MDT");
        this.SetAll("America/Chicago", "CT", "CST", "CDT");
        this.SetAll("America/New_York", "ET", "EST", "EDT");
        this.SetAll("Europe/London", "GMT", "BST");
        this.SetAll("Asia/Tokyo", "JST", "JT");
    }

    public void SetAll(string timezoneName, params string[] otherNames)
    {
        var tz = this[timezoneName];

        foreach (string otherName in otherNames)
        {
            this[otherName] = tz;
        }
    }

    public TimeZoneInfo this[string name]
    {
        get
        {
            if (this.cache.TryGetValue(name, out TimeZoneInfo? foundTz))
            {
                return foundTz;
            }

            TimeZoneInfo newTz = TimeZoneInfo.FindSystemTimeZoneById(name);
            this.cache[name] = newTz;
            return newTz;
        }

        set => this.cache[name] = value;
    }

}