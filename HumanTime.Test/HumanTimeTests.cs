namespace HumanTime.Test;

using HumanTimeParser;

public class HumanTimeTests
{
    public static IEnumerable<object[]> GetData()
    {
        DateTimeOffset now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "America/Chicago");
        DateTimeOffset future = now.AddMonths(2);
        DateTimeOffset past = new DateTimeOffset(now.Year, 1, 3, 0, 0, 0, TimeSpan.FromHours(-6));
        
        yield return new object[] { $"{future.Month}/{future.Day}", HumanTime.Date(future.Year, future.Month, future.Day) };
        yield return new object[] { $"5/{now.Year + 2}", HumanTime.Month(now.Year + 2, 5) };
        yield return new object[] { "5/25/2025", HumanTime.Date(2025, 5, 25) };
        yield return new object[] { $"{past.Month}/{past.Day}", HumanTime.Date(past.Year + 1, past.Month, past.Day) };
        yield return new object[] { "today", HumanTime.Date(now.Year, now.Month, now.Day) };
        
        DateTimeOffset tomorrow = now.AddDays(1);
        yield return new object[] { "tomorrow", HumanTime.Date(tomorrow.Year, tomorrow.Month, tomorrow.Day) };
        
        DateTimeOffset yesterday = now.AddDays(-1);
        yield return new object[] { "yesterday", HumanTime.Date(yesterday.Year, yesterday.Month, yesterday.Day) };

        yield return new object[] { "25th of May, 2025", HumanTime.Date(2025, 5, 25) };
    }
    
    [Theory]
    [MemberData(nameof(HumanTimeTests.GetData))]
    public void TestParsing(string input, HumanTime expected)
    {
        HumanTime actual = HumanTime.Parse(input);
        Assert.Equal(expected, actual);
    }
}
