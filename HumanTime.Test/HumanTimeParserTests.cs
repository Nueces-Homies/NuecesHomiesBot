using System.Globalization;
using Pidgin;

namespace HumanTime.Test;

public class HumanTimeParserTests
{
    [Fact]
    public void TestKeywordParser()
    {
        var parser = HumanTimeParser.Keyword("April");
        var result = parser.ParseOrThrow("April    ");
        Assert.Equal("April", result);
    }
    
    [Fact]
    public void TestPunctuationParser()
    {
        var parser = HumanTimeParser.Punctuation('/');
        var result = parser.ParseOrThrow("/    ");
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public void TestMonthParser()
    {
        for (int expectedMonthNumber = 1; expectedMonthNumber <= 12; expectedMonthNumber++)
        {
            var month = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(expectedMonthNumber).ToLower();
            var testInput = $"{month}";
            var monthNumber = HumanTimeParser.Month.ParseOrThrow(testInput);
            Assert.Equal(expectedMonthNumber, monthNumber);
        }
    }
    
    [Fact]
    public void TestOrdinalNumber()
    {
        var input = "8th";
        var result = HumanTimeParser.OrdinalNumber.ParseOrThrow(input);
        Assert.Equal(8, result);
    }
    
    [Theory]
    [InlineData("may 18th")]
    [InlineData("may 18 ")]
    [InlineData("18th of may")]
    [InlineData("18th may")]
    public void TestLongDateWithoutYear(string input)
    {
        var result = HumanTimeParser.LongDateWithoutYear.ParseOrThrow(input);
        Assert.Equal((5, 18), result);
    }
    
    [Theory]
    [InlineData("may 18th, 2023")]
    [InlineData("may 18 2023")]
    [InlineData("18th of may, 2023")]
    [InlineData("18th may 2023")]
    public void TestLongDateWithYear(string input)
    {
        var result = HumanTimeParser.LongDateWithYear.ParseOrThrow(input);
        Assert.Equal(new DateOnly(2023, 5, 18), result);
    }
    
    [Theory]
    [InlineData("5/18", 5, 18)]
    [InlineData("5/2024   ", 5, 2024)]
    public void TestTwoNumbers(string input, int num1, int num2)
    {
        var result = HumanTimeParser.TwoNumbers.ParseOrThrow(input);
        Assert.Equal((num1, num2), result);
    }

    [Fact]
    public void TestTripleNumbers()
    {
        string input = "1/17/2023";
        var result = HumanTimeParser.TripleNumbers.ParseOrThrow(input);
        Assert.Equal(new DateOnly(2023, 1, 17), result);
    }

    [Theory]
    [InlineData("day", HumanTimeParser.OffsetUnit.Days)]
    [InlineData("days", HumanTimeParser.OffsetUnit.Days)]
    [InlineData("month", HumanTimeParser.OffsetUnit.Months)]
    [InlineData("months", HumanTimeParser.OffsetUnit.Months)]
    [InlineData("year", HumanTimeParser.OffsetUnit.Years)]
    [InlineData("years", HumanTimeParser.OffsetUnit.Years)]
    public void TestOffsetUnit(string input, HumanTimeParser.OffsetUnit expected)
    {
        var result = HumanTimeParser.OffsetUnitParser.ParseOrThrow(input);
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("a", 1)]
    [InlineData("the", 1)]
    [InlineData("a 51", 51)]
    [InlineData("the 62", 62)]
    [InlineData("73", 73)]
    public void TestOffsetCount(string input, int count)
    {
        var result = HumanTimeParser.Count.ParseOrThrow(input);
        Assert.Equal(count, result);
    }

    [Theory]
    [InlineData("the day before", HumanTimeParser.OffsetUnit.Days, -1)]
    [InlineData("a month after", HumanTimeParser.OffsetUnit.Months, 1)]
    [InlineData("5 years from", HumanTimeParser.OffsetUnit.Years, 5)]
    [InlineData("the 12 years before", HumanTimeParser.OffsetUnit.Years, -12)]
    public void TestOffset(string input, HumanTimeParser.OffsetUnit unit, int count)
    {
        var result = HumanTimeParser.Offset.ParseOrThrow(input);
        Assert.Equal((unit, count), result);
    }

    [Fact]
    public void TestDate()
    {
        var result = HumanTimeParser.Date.ParseOrThrow("the day before the day after may 5, 2025");
        Assert.Equal(HumanTime.Date(2025, 5, 5), result);
    }

    [Theory]
    [InlineData("6 o'clock", 6, 0)]
    [InlineData("6 o'clock pm", 18, 0)]
    [InlineData("12 o 'clock pm", 12, 0)]
    [InlineData("12 o 'clock am", 0, 0)]
    [InlineData("19:18", 19, 18)]
    [InlineData("4:20pm", 16, 20)]
    [InlineData("6:09", 6, 9)]
    [InlineData("2pm", 14, 0)]
    [InlineData("12:34 am", 0, 34)]
    public void TestTime(string input, int hour, int minute)
    {
        var result = HumanTimeParser.Time.ParseOrThrow(input);
        Assert.Equal(new TimeOnly(hour, minute), result);
    }

    [Theory]
    [InlineData("mt", "America/Denver")]
    [InlineData("pdt", "America/Los_Angeles")]
    public void TestTimeZone(string input, string tzName)
    {
        var expected = TimeZoneInfo.FindSystemTimeZoneById(tzName);
        var result = HumanTimeParser.KnownTimeZones.ParseOrThrow(input);
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("4:20pm", 16, 20, "America/Chicago")]
    [InlineData("3 am pdt", 3, 0, "America/Los_Angeles")]
    [InlineData("11pm jst", 23, 0, "Asia/Tokyo")]
    public void TestTimeWithZone(string input, int hour, int minute, string tzName)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(tzName);
        var result = HumanTimeParser.TimeWithTimeZone.ParseOrThrow(input);
        Assert.Equal((new TimeOnly(hour, minute), tz), result);
    }
}