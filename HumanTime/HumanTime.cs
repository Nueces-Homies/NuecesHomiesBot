namespace HumanTimeParser;

using System.Globalization;
using Antlr4.Runtime;
using Parser;

public enum HumanTimeType
{
    Date,
    DateTime,
    Window,
}

public enum HumanTimeWindowType
{
    Month,
    Quarter,
    Trimester,
    Semester,
    Year,
    Unspecified,
}

public abstract record HumanTime
{
    public abstract HumanTimeType TimeType { get; }

    public static HumanTime Date(int year, int month, int day)
    {
        return new HumanDate
        {
            Date = new DateOnly(year, month, day)
        };
    }
    
    public static HumanTime Quarter(int quarter, int year)
    {
        if (quarter is < 1 or > 4)
        {
            throw new ArgumentException($"Quarter {quarter} is not in [1,4]");
        }
        
        return new HumanTimeWindow
        {
            WindowType = HumanTimeWindowType.Quarter,
            Description = $"Q{quarter} {year}",
            StartDate = new DateOnly(year, 3*(quarter-1)+1, 1),
        };
    }

    public static HumanTime Month(int year, int month)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentException($"There aren't {month} months in a year");
        }

        return new HumanTimeWindow
        {
            WindowType = HumanTimeWindowType.Month,
            Description = $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month)} {year}",
            StartDate = new DateOnly(year, month, 1),
        };
    }

    public static HumanTime DateTime(DateOnly date, TimeOnly time, TimeZoneInfo timezone)
    {
        var dt = date.ToDateTime(time);
        var offset = timezone.GetUtcOffset(dt);
        return new HumanDateTime
        {
            DateTime = new DateTimeOffset(dt, offset),
        };
    }

    public static HumanTime Parse(string input)
    {
        var lexer = new HumanTimeGrammarLexer(new AntlrInputStream(input.ToLower()));
        var parser = new HumanTimeGrammarParser(new CommonTokenStream(lexer));
        ParseResult result = new HumanTimeParser().VisitInput(parser.input());

        return result switch
        {
            ParseError parseError                  => throw new Exception(parseError.Reason),
            ParseSuccess { Output: var humanTime } => humanTime,
            _                                      => throw new Exception($"Unexpected type {result.GetType()}")
        };
    }

    public abstract HumanTime WithYear(int year);
}

public sealed record HumanDate : HumanTime
{
    public override HumanTimeType TimeType => HumanTimeType.Date;
    
    public required DateOnly Date { get; init; }

    public override HumanTime WithYear(int year)
    {
        return new HumanDate { Date = new DateOnly(year, this.Date.Month, this.Date.Day) };
    }

    public override string ToString() => Date.ToShortDateString();
}

public sealed record HumanDateTime : HumanTime
{
    public override HumanTimeType TimeType => HumanTimeType.DateTime;
    
    public required DateTimeOffset DateTime { get; init; }
    
    public override HumanTime WithYear(int year)
    {
        return new HumanDateTime
        {
            DateTime = new DateTime(
                year, DateTime.Month, DateTime.Day,
                DateTime.Hour, DateTime.Minute, DateTime.Second
            ),
        };
    }

    public override string ToString() => DateTime.ToString();
}

public sealed record HumanTimeWindow : HumanTime
{
    public override HumanTimeType TimeType => HumanTimeType.Window;
    
    public required string Description { get; init; }
    public required HumanTimeWindowType WindowType { get; init; }
    public required DateOnly StartDate { get; init; }

    public override HumanTime WithYear(int year)
    {
        return this with
        {
            StartDate = new DateOnly(year, StartDate.Month, StartDate.Day)
        };
    }

    public override string ToString() => Description;
}