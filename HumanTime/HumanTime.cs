using System.Diagnostics.CodeAnalysis;

namespace HumanTime;

using System.Globalization;

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
    
    private static readonly TimeZoneInfo CentralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
    
    private static DateTimeOffset Now => TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.UtcNow.DateTime, CentralTimeZone);
    private static DateOnly Today => DateOnly.FromDateTime(Now.DateTime);

    public static HumanTime Date(DateOnly date)
    {
        return new HumanDate { Date = date };
    }

    public static HumanTime Date(int year, int month, int day)
    {
        return new HumanDate
        {
            Date = new DateOnly(year, month, day)
        };
    }

    public static HumanTime Date(int month, int day)
    {
        var date = new DateOnly(Now.Year, month, day);
        if (date < Today)
        {
            date = date.AddYears(1);
        }

        return new HumanDate() { Date = date };
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
}

public sealed record HumanDate : HumanTime
{
    public override HumanTimeType TimeType => HumanTimeType.Date;
    
    public new required DateOnly Date { get; init; }

    public override string ToString() => Date.ToShortDateString();
}

public sealed record HumanDateTime : HumanTime
{
    public override HumanTimeType TimeType => HumanTimeType.DateTime;
    
    public new required DateTimeOffset DateTime { get; init; }
    
    public override string ToString() => DateTime.ToString();
}

public sealed record HumanTimeWindow : HumanTime
{
    public override HumanTimeType TimeType => HumanTimeType.Window;
    
    public required string Description { get; init; }
    public required HumanTimeWindowType WindowType { get; init; }
    public required DateOnly StartDate { get; init; }

    public override string ToString() => Description;
}