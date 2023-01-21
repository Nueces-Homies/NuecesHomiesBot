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
}