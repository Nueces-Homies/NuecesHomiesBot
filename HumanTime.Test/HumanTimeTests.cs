namespace HumanTime.Test;

using TestCase = HumanTimeTestCase;

public class HumanTimeTests
{
    public static IEnumerable<object[]> GetData()
    {
        // Test runs relative to March 16, 2023 at 8:05:42 PM CST
        var testTime = new DateTimeOffset(2023, 3, 16, 20, 5, 42, TimeSpan.FromHours(-5));
        HumanTime.GetNowCentral = () => testTime;

        yield return new object[] { new TestCase("5/5",       HumanTime.Date(2023, 5, 5)) };
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
        
        yield return new object[] { new TestCase("April",     HumanTime.Month(2023, 4)) };
        yield return new object[] { new TestCase("May",       HumanTime.Month(2023, 5)) };
        yield return new object[] { new TestCase("June",      HumanTime.Month(2023, 6)) };
        yield return new object[] { new TestCase("July",      HumanTime.Month(2023, 7)) };
        yield return new object[] { new TestCase("August",    HumanTime.Month(2023, 8)) };
        yield return new object[] { new TestCase("September", HumanTime.Month(2023, 9)) };
        yield return new object[] { new TestCase("October",   HumanTime.Month(2023, 10)) };
        yield return new object[] { new TestCase("November",  HumanTime.Month(2023, 11)) };
        yield return new object[] { new TestCase("December",  HumanTime.Month(2023, 12)) };
        yield return new object[] { new TestCase("January",   HumanTime.Month(2024, 1)) };
        yield return new object[] { new TestCase("February",  HumanTime.Month(2024, 2)) };
        yield return new object[] { new TestCase("March",     HumanTime.Month(2024, 3)) };
        
        yield return new object[] { new TestCase("March 2027",     HumanTime.Month(2027, 3)) };
        
        yield return new object[] { new TestCase("Thursday",  HumanTime.Date(2023, 3, 16)) };
        yield return new object[] { new TestCase("Friday",    HumanTime.Date(2023, 3, 17)) };
        yield return new object[] { new TestCase("Saturday",  HumanTime.Date(2023, 3, 18)) };
        yield return new object[] { new TestCase("Sunday",    HumanTime.Date(2023, 3, 19)) };
        yield return new object[] { new TestCase("Monday",    HumanTime.Date(2023, 3, 20)) };
        yield return new object[] { new TestCase("Tuesday",   HumanTime.Date(2023, 3, 21)) };
        yield return new object[] { new TestCase("Wednesday", HumanTime.Date(2023, 3, 22)) };
        
        
        yield return new object[] { new TestCase("5 days before August 20th, 2032", HumanTime.Date(2032, 8, 15)) };
        yield return new object[] { new TestCase("3 days after December 18, 2022", HumanTime.Date(2022, 12, 21)) };
        yield return new object[] { new TestCase("the 12 days after December 25, 2022", HumanTime.Date(2023, 1, 6)) };
        
        yield return new object[] { new TestCase("the day after tomorrow", HumanTime.Date(2023, 3, 18)) };
        
        yield return new object[] { new TestCase("4 months before July 4, 1776", HumanTime.Date(1776, 3, 4)) };
        yield return new object[] { new TestCase("A year after January 6th", HumanTime.Date(2025, 1, 6)) };
        
        var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var japaneseTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        
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

        yield return new object[] { new TestCase("Q2 2024", HumanTime.Quarter(2, 2024)) };
        yield return new object[] { new TestCase("Late 2025", HumanTime.Trimester(3, 2025)) };
        yield return new object[] { new TestCase("Holiday 2024", HumanTime.Holiday(2024)) };
        yield return new object[] { new TestCase("2026", HumanTime.Year(2026)) };
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

    [Theory]
    [InlineData(1, 2024, "Q1 2024")]
    [InlineData(2, 2025, "Q2 2025")]
    [InlineData(3, 2026, "Q3 2026")]
    [InlineData(4, 2027, "Q4 2027")]
    public void TestQuarterDescription(int quarter, int year, string expected)
    {
        var window = HumanTime.Quarter(quarter, year);
        Assert.Equal(expected, window.ToString());
    }
    
    [Theory]
    [InlineData(1, 2024, "early 2024")]
    [InlineData(2, 2025, "mid 2025")]
    [InlineData(3, 2026, "late 2026")]
    public void TestTrimesterDescription(int trimester, int year, string expected)
    {
        var window = HumanTime.Trimester(trimester, year);
        Assert.Equal(expected, window.ToString());
    }
    
    [Theory]
    [InlineData(1,  2024, "January 2024")]
    [InlineData(2,  2025, "February 2025")]
    [InlineData(3,  2026, "March 2026")]
    [InlineData(4,  2024, "April 2024")]
    [InlineData(5,  2025, "May 2025")]
    [InlineData(6,  2026, "June 2026")]
    [InlineData(7,  2024, "July 2024")]
    [InlineData(8,  2025, "August 2025")]
    [InlineData(9,  2026, "September 2026")]
    [InlineData(10, 2024, "October 2024")]
    [InlineData(11, 2025, "November 2025")]
    [InlineData(12, 2026, "December 2026")]
    public void TestMonthDescription(int month, int year, string expected)
    {
        var window = HumanTime.Month(year, month);
        Assert.Equal(expected, window.ToString());
    }

    [Fact]
    public void TestUnspecifiedDescription()
    {
        Assert.Equal("TBD", HumanTime.TBD.ToString());
    }

    [Fact]
    public void TestHolidayDescription()
    {
        Assert.Equal("Holiday 2028", HumanTime.Holiday(2028).ToString());
    }
}