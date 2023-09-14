namespace Core.Test;

using TestCase = HumanTimeTestCase;

public class HumanTimeTests
{
    public static IEnumerable<object[]> GetData()
    {
        // Test runs relative to March 16, 2023 at 8:05:42 PM CST
        var testTime = new DateTimeOffset(2023, 3, 16, 20, 5, 42, TimeSpan.FromHours(-5));
        Common.HumanTime.HumanTime.GetNowCentral = () => testTime;

        yield return new object[] { new TestCase("5/5",       Common.HumanTime.HumanTime.Date(2023, 5, 5)) };
        yield return new object[] { new TestCase("5/25/2025", Common.HumanTime.HumanTime.Date(2025, 5, 25)) };
        yield return new object[] { new TestCase("1/14",      Common.HumanTime.HumanTime.Date(2024, 1, 14)) };
        
        yield return new object[] { new TestCase("today",     Common.HumanTime.HumanTime.Date(2023, 3, 16)) };
        yield return new object[] { new TestCase("tomorrow",  Common.HumanTime.HumanTime.Date(2023, 3, 17)) };
        yield return new object[] { new TestCase("yesterday", Common.HumanTime.HumanTime.Date(2023, 3, 15)) };
        
        yield return new object[] { new TestCase("10th of November, 2025", Common.HumanTime.HumanTime.Date(2025, 11, 10)) };
        yield return new object[] { new TestCase("25th of October 2025",   Common.HumanTime.HumanTime.Date(2025, 10, 25)) };
        yield return new object[] { new TestCase("May 23rd",               Common.HumanTime.HumanTime.Date(2023, 5, 23)) };
        yield return new object[] { new TestCase("February 4th",           Common.HumanTime.HumanTime.Date(2024, 2, 4)) };
        
        yield return new object[] { new TestCase("now",           Common.HumanTime.HumanTime.Now()) };
        
        yield return new object[] { new TestCase("April",     Common.HumanTime.HumanTime.Month(2023, 4)) };
        yield return new object[] { new TestCase("May",       Common.HumanTime.HumanTime.Month(2023, 5)) };
        yield return new object[] { new TestCase("June",      Common.HumanTime.HumanTime.Month(2023, 6)) };
        yield return new object[] { new TestCase("July",      Common.HumanTime.HumanTime.Month(2023, 7)) };
        yield return new object[] { new TestCase("August",    Common.HumanTime.HumanTime.Month(2023, 8)) };
        yield return new object[] { new TestCase("September", Common.HumanTime.HumanTime.Month(2023, 9)) };
        yield return new object[] { new TestCase("October",   Common.HumanTime.HumanTime.Month(2023, 10)) };
        yield return new object[] { new TestCase("November",  Common.HumanTime.HumanTime.Month(2023, 11)) };
        yield return new object[] { new TestCase("December",  Common.HumanTime.HumanTime.Month(2023, 12)) };
        yield return new object[] { new TestCase("January",   Common.HumanTime.HumanTime.Month(2024, 1)) };
        yield return new object[] { new TestCase("February",  Common.HumanTime.HumanTime.Month(2024, 2)) };
        yield return new object[] { new TestCase("March",     Common.HumanTime.HumanTime.Month(2024, 3)) };
        
        yield return new object[] { new TestCase("March 2027",     Common.HumanTime.HumanTime.Month(2027, 3)) };
        
        yield return new object[] { new TestCase("Thursday",  Common.HumanTime.HumanTime.Date(2023, 3, 16)) };
        yield return new object[] { new TestCase("Friday",    Common.HumanTime.HumanTime.Date(2023, 3, 17)) };
        yield return new object[] { new TestCase("Saturday",  Common.HumanTime.HumanTime.Date(2023, 3, 18)) };
        yield return new object[] { new TestCase("Sunday",    Common.HumanTime.HumanTime.Date(2023, 3, 19)) };
        yield return new object[] { new TestCase("Monday",    Common.HumanTime.HumanTime.Date(2023, 3, 20)) };
        yield return new object[] { new TestCase("Tuesday",   Common.HumanTime.HumanTime.Date(2023, 3, 21)) };
        yield return new object[] { new TestCase("Wednesday", Common.HumanTime.HumanTime.Date(2023, 3, 22)) };
        
        
        yield return new object[] { new TestCase("5 days before August 20th, 2032", Common.HumanTime.HumanTime.Date(2032, 8, 15)) };
        yield return new object[] { new TestCase("3 days after December 18, 2022", Common.HumanTime.HumanTime.Date(2022, 12, 21)) };
        yield return new object[] { new TestCase("the 12 days after December 25, 2022", Common.HumanTime.HumanTime.Date(2023, 1, 6)) };
        
        yield return new object[] { new TestCase("the day after tomorrow", Common.HumanTime.HumanTime.Date(2023, 3, 18)) };
        
        yield return new object[] { new TestCase("4 months before July 4, 1776", Common.HumanTime.HumanTime.Date(1776, 3, 4)) };
        yield return new object[] { new TestCase("A year after January 6th", Common.HumanTime.HumanTime.Date(2025, 1, 6)) };
        
        var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var japaneseTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        
        yield return new object[]
        {
            new TestCase("December 3 at 4:20am",
                Common.HumanTime.HumanTime.DateTime(new DateOnly(2023, 12, 3), new TimeOnly(4, 20), centralTimeZone))
        };
        yield return new object[]
        {
            new TestCase("11:23pm PST on February 8th",
                Common.HumanTime.HumanTime.DateTime(new DateOnly(2024, 2, 8), new TimeOnly(23, 23), pacificTimeZone))
        };
        yield return new object[]
        {
            new TestCase("2nd February 8:47pm JST",
                Common.HumanTime.HumanTime.DateTime(new DateOnly(2024, 2, 2), new TimeOnly(20, 47), japaneseTimeZone))
        };
        yield return new object[]
        {
            new TestCase("February 2nd 8:47pm JST",
                Common.HumanTime.HumanTime.DateTime(new DateOnly(2024, 2, 2), new TimeOnly(20, 47), japaneseTimeZone))
        };

        yield return new object[] { new TestCase("Q2 2024", Common.HumanTime.HumanTime.Quarter(2, 2024)) };
        yield return new object[] { new TestCase("Late 2025", Common.HumanTime.HumanTime.Trimester(3, 2025)) };
        yield return new object[] { new TestCase("Holiday 2024", Common.HumanTime.HumanTime.Holiday(2024)) };
        yield return new object[] { new TestCase("2026", Common.HumanTime.HumanTime.Year(2026)) };
    }
    
    [Theory]
    [MemberData(nameof(GetData))]
    public void TestParsing(TestCase testCase)
    {
        Assert.NotNull(testCase.InputString);
        Assert.NotNull(testCase.Expected);
        
        var actual = Common.HumanTime.HumanTime.Parse(testCase.InputString);
        Assert.Equal(testCase.Expected, actual);
    }
}