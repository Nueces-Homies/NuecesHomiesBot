namespace Core.Test;

using HumanTime;
using TestCase = HumanTimeTestCase;

public class HumanTimeTests
{
    public static IEnumerable<object[]> GetData()
    {
        // Test runs relative to March 16, 2023 at 8:05:42 PM CST
        var testTime = new DateTimeOffset(2023, 3, 16, 20, 5, 42, TimeSpan.FromHours(-5));
        HumanTime.GetNowCentral = () => testTime;

        yield return new object[] { new TestCase("5/5",       HumanTime.Date(2023, 5, 5)) };
        // yield return new object[] { new TestCase("5/2025",    HumanTime.Month(2025, 5)) };
        yield return new object[] { new TestCase("5/25/2025", HumanTime.Date(2025, 5, 25)) };
        yield return new object[] { new TestCase("1/14",      HumanTime.Date(2024, 1, 14)) };
        
        yield return new object[] { new TestCase("today",     HumanTime.Date(2023, 3, 16)) };
        yield return new object[] { new TestCase("tomorrow",  HumanTime.Date(2023, 3, 17)) };
        yield return new object[] { new TestCase("yesterday", HumanTime.Date(2023, 3, 15)) };
        
        yield return new object[] { new TestCase("10th of November, 2025", HumanTime.Date(2025, 11, 10)) };
        yield return new object[] { new TestCase("25th of October 2025",   HumanTime.Date(2025, 10, 25)) };
        yield return new object[] { new TestCase("May 23rd",               HumanTime.Date(2023, 5, 23)) };
        yield return new object[] { new TestCase("February 4th",           HumanTime.Date(2024, 2, 4)) };
        
        yield return new object[] { new TestCase("now",           HumanTime.Now()) };
        //
        // yield return new object[] { new TestCase("April",     HumanTime.Month(2023, 4)) };
        // yield return new object[] { new TestCase("May",       HumanTime.Month(2023, 5)) };
        // yield return new object[] { new TestCase("June",      HumanTime.Month(2023, 6)) };
        // yield return new object[] { new TestCase("July",      HumanTime.Month(2023, 7)) };
        // yield return new object[] { new TestCase("August",    HumanTime.Month(2023, 8)) };
        // yield return new object[] { new TestCase("September", HumanTime.Month(2023, 9)) };
        // yield return new object[] { new TestCase("October",   HumanTime.Month(2023, 10)) };
        // yield return new object[] { new TestCase("November",  HumanTime.Month(2023, 11)) };
        // yield return new object[] { new TestCase("December",  HumanTime.Month(2023, 12)) };
        // yield return new object[] { new TestCase("January",   HumanTime.Month(2024, 1)) };
        // yield return new object[] { new TestCase("February",  HumanTime.Month(2024, 2)) };
        // yield return new object[] { new TestCase("March",     HumanTime.Month(2024, 3)) };
        //
        // yield return new object[] { new TestCase("Friday",    HumanTime.Date(2023, 3, 17)) };
        // yield return new object[] { new TestCase("Saturday",  HumanTime.Date(2023, 3, 18)) };
        // yield return new object[] { new TestCase("Sunday",    HumanTime.Date(2023, 3, 19)) };
        // yield return new object[] { new TestCase("Monday",    HumanTime.Date(2023, 3, 20)) };
        // yield return new object[] { new TestCase("Tuesday",   HumanTime.Date(2023, 3, 21)) };
        // yield return new object[] { new TestCase("Wednesday", HumanTime.Date(2023, 3, 22)) };
        // yield return new object[] { new TestCase("Thursday",  HumanTime.Date(2023, 3, 23)) };
        //
        //
        yield return new object[] { new TestCase("5 days before August 20th, 2032", HumanTime.Date(2032, 8, 15)) };
        yield return new object[] { new TestCase("3 days after December 18, 2022", HumanTime.Date(2022, 12, 21)) };
        yield return new object[] { new TestCase("the 12 days after December 25, 2022", HumanTime.Date(2023, 1, 6)) };
        
        yield return new object[] { new TestCase("the day after tomorrow", HumanTime.Date(2023, 3, 18)) };
        
        yield return new object[] { new TestCase("4 months before July 4, 1776", HumanTime.Date(1776, 3, 4)) };
        yield return new object[] { new TestCase("A year after January 6th", HumanTime.Date(2025, 1, 6)) };
        
        var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var japaneseTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        
        // yield return new object[] { new TestCase("3:20pm", HumanTime.DateTime(new TimeOnly(15, 20))) };
        yield return new object[]
        {
            new TestCase("December 3 at 4:20am",
                HumanTime.DateTime(new DateOnly(2023, 12, 3), new TimeOnly(4, 20), centralTimeZone))
        };
        yield return new object[]
        {
            new TestCase("11:23pm PST on February 8th",
                HumanTime.DateTime(new DateOnly(2024, 2, 8), new TimeOnly(23, 23), pacificTimeZone))
        };
        yield return new object[]
        {
            new TestCase("2nd February 8:47pm JST",
                HumanTime.DateTime(new DateOnly(2024, 2, 2), new TimeOnly(20, 47), japaneseTimeZone))
        };
        yield return new object[]
        {
            new TestCase("February 2nd 8:47pm JST",
                HumanTime.DateTime(new DateOnly(2024, 2, 2), new TimeOnly(20, 47), japaneseTimeZone))
        };
    }
    
    [Theory]
    [MemberData(nameof(GetData))]
    public void TestParsing(TestCase testCase)
    {
        Assert.NotNull(testCase.InputString);
        Assert.NotNull(testCase.Expected);
        
        var actual = HumanTime.Parse(testCase.InputString);
        Assert.Equal(testCase.Expected, actual);
    }
}