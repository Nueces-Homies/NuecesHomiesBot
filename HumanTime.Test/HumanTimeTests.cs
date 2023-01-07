namespace HumanTime.Test;

using System.Globalization;
using HumanTimeParser;

using TestCase = HumanTimeTestCase;

public class HumanTimeTests
{
    public static IEnumerable<object[]> GetData()
    {
        DateTimeOffset now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "America/Chicago");
        DateTimeOffset future = now.AddMonths(2);
        
        // Some tests may fail if run on January 1st or 2nd, so just don't do that :)
        DateTimeOffset past = new DateTimeOffset(now.Year, 1, 2, 0, 0, 0, TimeSpan.FromHours(-6));
        
        yield return new object[]
        {
            new TestCase($"{future.Month}/{future.Day}", HumanTime.Date(future.Year, future.Month, future.Day)),
        };

        yield return new object[] { new TestCase($"5/{now.Year + 2}", HumanTime.Month(now.Year + 2, 5)) };
        yield return new object[] { new TestCase("5/25/2025", HumanTime.Date(2025, 5, 25)) };
        yield return new object[] { new TestCase($"{past.Month}/{past.Day}", HumanTime.Date(past.Year + 1, past.Month, past.Day)) };
        yield return new object[] { new TestCase("today", HumanTime.Date(now.Year, now.Month, now.Day)) };
        
        DateTimeOffset tomorrow = now.AddDays(1);
        yield return new object[] { new TestCase("tomorrow", HumanTime.Date(tomorrow.Year, tomorrow.Month, tomorrow.Day)) };
        
        DateTimeOffset yesterday = now.AddDays(-1);
        yield return new object[] { new TestCase("yesterday", HumanTime.Date(yesterday.Year, yesterday.Month, yesterday.Day)) };
        
        yield return new object[] { new TestCase("25th of November, 2025", HumanTime.Date(2025, 11, 25)) };
        yield return new object[] { new TestCase("25th of October '25", HumanTime.Date(2025, 10, 25)) };
        yield return new object[]
        {
            new TestCase(
                $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(future.Month)} {future.Day.ToOrdinal()}", 
                HumanTime.Date(future.Year, future.Month, future.Day)),
        };
        yield return new object[]
        {
            new TestCase(
                $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(past.Month)} {past.Day.ToOrdinal()}",
                HumanTime.Date(past.Year+1, past.Month, past.Day)),
        };
        
        var roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        yield return new object[]
        {
            new TestCase(
                "Now",
                HumanTime.DateTime(
                    DateOnly.FromDateTime(roundedNow), 
                    TimeOnly.FromDateTime(roundedNow), 
                    TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"))
            ),
        };
        
        for (var i = 1; i <= 12; i++)
        {
            yield return new object[]
            {
                new TestCase(CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(i),
                    HumanTime.Month(now.Year, i)),
            };
        }
        
        yield return new object[] { new TestCase("5 days before August 20th, 2032", HumanTime.Date(2032, 8, 15)) };
        yield return new object[] { new TestCase("3 days after December 18, 2022", HumanTime.Date(2022, 12, 21)) };
        
        DateTimeOffset dayAfterTomorrow = now.AddDays(2);
        yield return new object[]
        {
            new TestCase(
                "the day after tomorrow", 
                HumanTime.Date(dayAfterTomorrow.Year, dayAfterTomorrow.Month, dayAfterTomorrow.Day)
            )
        };
        
        yield return new object[]
        {
            new TestCase(
                "the day before the day after tomorrow", 
                HumanTime.Parse("tomorrow")
            ),
        };
        
        yield return new object[] { new TestCase("4 months before July 4, 1776", HumanTime.Date(1776, 3, 4)) };
        yield return new object[] { new TestCase("A year after January 6th", HumanTime.Date(now.Year+1, 1, 6)) };
    }
    
    [Theory]
    [MemberData(nameof(HumanTimeTests.GetData))]
    public void TestParsing(TestCase testCase)
    {
        Assert.NotNull(testCase.InputString);
        Assert.NotNull(testCase.Expected);
        
        HumanTime actual = HumanTime.Parse(testCase.InputString);
        Assert.Equal(testCase.Expected, actual);
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