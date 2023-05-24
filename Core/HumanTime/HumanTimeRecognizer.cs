using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;

namespace Core.HumanTime;

public class HumanTimeRecognizer
{
    private Func<DateTimeOffset> GetBaseTime { get; init; }
    
    private DateTimeModel Model { get; set; }
    
    internal HumanTimeRecognizer(Func<DateTimeOffset> getBaseTimeFunc)
    {
        Model = new DateTimeRecognizer().GetDateTimeModel(culture: Culture.English);
        GetBaseTime = getBaseTimeFunc;
    }

    internal HumanTime Recognize(string query)
    {
        var refTime = GetBaseTime();

        var queryWithoutTimezone = WithoutTimezone(query, out var timezone);
        
        var results = Model.Parse(queryWithoutTimezone, refTime.DateTime);
        var values = (List<Dictionary<string, string>>) results[0].Resolution["values"];
        
        var times = values.Select(value => ToHumanTime(results[0].TypeName, value));
        // var refTimeAsHumanTime = HumanTime.DateTime(refTime);
        
        // foreach (var time in times)
        // {
        //     if (time > refTimeAsHumanTime)
        //     {
        //         return time;
        //     }
        // }

        var result = times.Last();

        if (result is not HumanDateTime hdt) return result;
        
        var dto = hdt.DateTime;
        var date = DateOnly.FromDateTime(dto.DateTime);
        var time = TimeOnly.FromDateTime(dto.DateTime);
        return HumanTime.DateTime(date, time, timezone);

    }

    private static string WithoutTimezone(string query, out TimeZoneInfo timezone)
    {
        var output = Regex.Replace(query, KnownTimeZones.Cache.TimeZoneRegex, "");
        
        var match = Regex.Match(query, KnownTimeZones.Cache.TimeZoneRegex);
        var tzName = match.Success ? match.Value.Trim() : "ct";
        timezone = KnownTimeZones.Cache[tzName];

        return output;
    }

    private static HumanTime ToHumanTime(string typeName, Dictionary<string, string> data)
    {
        return typeName switch
        {
            "datetimeV2.date" => ToHumanTimeDate(data),
            "datetimeV2.datetime" => ToHumanTimeDateTime(data),
            _ => throw new ArgumentException($"Unknown type {typeName}"),
        };
    }

    private static HumanTime ToHumanTimeDate(Dictionary<string, string> data)
    {
        var value = data["value"];
        var date = DateOnly.Parse(value);
        return HumanTime.Date(date);
    }

    private static HumanTime ToHumanTimeDateTime(Dictionary<string, string> data)
    {
        var value = data["value"];
        var dateTime = DateTime.Parse(value);
        dateTime = dateTime.AddSeconds(-dateTime.Second);

        var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        var offset = tz.GetUtcOffset(dateTime);
        return HumanTime.DateTime(new DateTimeOffset(dateTime, offset));
    }
}