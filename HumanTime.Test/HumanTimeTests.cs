namespace HumanTime.Test;

using System.Globalization;
using HumanTimeParser;

public class HumanTimeTests
{
    public static IEnumerable<object[]> GetData()
    {
        DateTimeOffset now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "America/Chicago");
        DateTimeOffset future = now.AddMonths(2);
        
        // Some tests may fail if run on January 1st or 2nd, so just don't do that :)
        DateTimeOffset past = new DateTimeOffset(now.Year, 1, 2, 0, 0, 0, TimeSpan.FromHours(-6));
        
        yield return new object[] { $"{future.Month}/{future.Day}", HumanTime.Date(future.Year, future.Month, future.Day) };
        yield return new object[] { $"5/{now.Year + 2}", HumanTime.Month(now.Year + 2, 5) };
        yield return new object[] { "5/25/2025", HumanTime.Date(2025, 5, 25) };
        yield return new object[] { $"{past.Month}/{past.Day}", HumanTime.Date(past.Year + 1, past.Month, past.Day) };
        yield return new object[] { "today", HumanTime.Date(now.Year, now.Month, now.Day) };
        
        DateTimeOffset tomorrow = now.AddDays(1);
        yield return new object[] { "tomorrow", HumanTime.Date(tomorrow.Year, tomorrow.Month, tomorrow.Day) };
        
        DateTimeOffset yesterday = now.AddDays(-1);
        yield return new object[] { "yesterday", HumanTime.Date(yesterday.Year, yesterday.Month, yesterday.Day) };

        yield return new object[] { "25th of November, 2025", HumanTime.Date(2025, 11, 25) };
        yield return new object[] { "25th of October '25", HumanTime.Date(2025, 10, 25) };
        yield return new object[]
        {
            $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(future.Month)} {future.Day.ToOrdinal()}",
            HumanTime.Date(future.Year, future.Month, future.Day),
        };
        yield return new object[]
        {
            $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(past.Month)} {past.Day.ToOrdinal()}",
            HumanTime.Date(past.Year+1, past.Month, past.Day),
        };
    }
    
    [Theory]
    [MemberData(nameof(HumanTimeTests.GetData))]
    public void TestParsing(string input, HumanTime expected)
    {
        HumanTime actual = HumanTime.Parse(input);
        Assert.Equal(expected, actual);
    }
}

file static class TestExtensions
{
    public static string ToOrdinal(this int num)
    {
        switch (num % 100)
        {
            case 11:
            case 12:
            case 13: 
                return num+"th";
        }

        return num + (num % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }
}