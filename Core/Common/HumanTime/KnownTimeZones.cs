using System.Text;
using System.Text.RegularExpressions;

namespace Core.HumanTime;

public class KnownTimeZones
{
    public static readonly KnownTimeZones Cache = new();
    
    private readonly Dictionary<string, TimeZoneInfo> timezones = new();
    public string TimeZoneRegex { get; private init; }
    
    public KnownTimeZones()
    {
        SetAll("America/Los_Angeles", "pt", "pst", "pdt");
        SetAll("America/Denver", "mt", "mst", "mdt");
        SetAll("America/Chicago", "ct", "cst", "cdt");
        SetAll("America/New_York", "et", "est", "edt");
        SetAll("Europe/London", "gmt", "bst");
        SetAll("Asia/Tokyo", "jst", "jt");
        this.TimeZoneRegex = BuildRegex();
    }

    private void SetAll(string timezoneName, params string[] otherNames)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);

        foreach (var otherName in otherNames)
        {
            timezones[otherName] = tz;
        }
    }

    public TimeZoneInfo this[string name] => timezones[name];

    private string BuildRegex()
    {
        StringBuilder stringBuilder = new StringBuilder(" (");
        foreach (var tzName in timezones.Keys)
        {
            stringBuilder.Append(tzName);
            stringBuilder.Append('|');
        }

        // Overwrite the last extra "|"
        stringBuilder.Length -= 1;
        
        stringBuilder.Append(')');
        
        return stringBuilder.ToString();
    }
}