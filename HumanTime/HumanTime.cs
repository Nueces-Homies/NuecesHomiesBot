namespace HumanTime;

using System.Globalization;

public enum HumanTimeType
{
    Date,
    DateTime,
    Window,
    Unknown
}

public enum HumanTimeWindowType
{
    Month,
    Quarter,
    Trimester,
    Year,
    Holiday,
    Unspecified,
}

public abstract record HumanTime
{
    public abstract HumanTimeType TimeType { get; }
    
    private static readonly TimeZoneInfo CentralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
    
    public static Func<DateTimeOffset> GetNowCentral { get; set; } = () =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.UtcNow.DateTime, CentralTimeZone);
    
    private static DateOnly TodayCentral => DateOnly.FromDateTime(GetNowCentral().DateTime);

    public static readonly HumanTime Unknown = HumanTimeUnknown.Instance;

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

    public static HumanTime Date(int number1, int number2)
    {
        int month;
        if (number2 > 1000)
        {
            month = number1;
            int year = number2;
            return Month(year, month);
        }

        month = number1;
        int day = number2;
        
        var date = new DateOnly(GetNowCentral().Year, month, day);
        if (date < TodayCentral)
        {
            date = date.AddYears(1);
        }

        return new HumanDate() { Date = date };
    }
    
    public static HumanTime Date(DayOfWeek dayOfWeek)
    {
        var today = (int)TodayCentral.DayOfWeek;
        var target = (int)dayOfWeek;
        target = target < today ? target + 7 : target;

        var offset = target - today;
        offset = offset == 0 ? 7 : offset;

        return new HumanDate { Date = TodayCentral.AddDays(offset) };
    }

    public static HumanTime Today => Date(TodayCentral);

    public static HumanTime Quarter(int quarter, int year)
    {
        if (quarter is < 1 or > 4)
        {
            throw new ArgumentException($"Quarter {quarter} is not in [1,4]");
        }
        
        return new HumanTimeWindow
        {
            WindowType = HumanTimeWindowType.Quarter,
            StartDate = new DateOnly(year, 3*(quarter-1)+1, 1),
        };
    }
    
    public static HumanTime Trimester(int trimester, int year)
    {
        if (trimester is < 1 or > 3)
        {
            throw new ArgumentException($"Trimester {trimester} is not in [1,3]");
        }

        var word = trimester switch
        {
            1 => "Early",
            2 => "Mid",
            3 => "Late",
        };
        
        return new HumanTimeWindow
        {
            WindowType = HumanTimeWindowType.Trimester,
            StartDate = new DateOnly(year, 4*(trimester-1)+1, 1),
        };
    }
    
    public static HumanTime Holiday(int year)
    {
        return new HumanTimeWindow
        {
            WindowType = HumanTimeWindowType.Holiday,
            StartDate = new DateOnly(year, 11, 1),
        };
    }

    public static HumanTime TBD = new HumanTimeWindow
    {
        WindowType = HumanTimeWindowType.Unspecified,
        StartDate = new DateOnly(3008, 1, 1),
    };

    public static HumanTime Month(int year, int month)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentException($"There aren't {month} months in a year");
        }

        return new HumanTimeWindow
        {
            WindowType = HumanTimeWindowType.Month,
            StartDate = new DateOnly(year, month, 1),
        };
    }
    
    public static HumanTime Month(int month)
    {
        var today = GetNowCentral();
        int thisMonth = today.Month;
        int year = thisMonth >= month ? today.Year + 1 : today.Year;
        
        return Month(year, month);
    }

    public static HumanTime Year(int year) => new HumanTimeWindow
    {
        WindowType = HumanTimeWindowType.Year,
        
        // We choose the end of the year for sorting purposes
        StartDate = new DateOnly(year, 12, 31),
    };

    public static HumanTime Now()
    {
        var now = GetNowCentral();
        var adjusted = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Offset); 
        
        return new HumanDateTime { DateTime = adjusted };
    }

    public static HumanTime DateTime(DateTimeOffset dateTimeOffset)
    {
        return new HumanDateTime() { DateTime = dateTimeOffset };
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
    
    public static HumanTime DateTime(TimeOnly time)
    {
        var now = GetNowCentral();
        var todayWithTime = new DateTimeOffset(now.Year, now.Month, now.Day, time.Hour, time.Minute, time.Second,
            now.Offset);

        // TODO: This will break if tomorrow is a time change 
        return new HumanDateTime { DateTime = todayWithTime < now ? todayWithTime.AddDays(1) : todayWithTime };
    }


    public static HumanTime Parse(string timeString)
    {
        // We create this dynamically so that we get the correct version of GetNowCentral
        IHumanTimeRecognizer[] recognizers = {
            new QuarterRecognizer(),
            new TrimesterRecognizer(),
            new MonthRecognizer(),
            new YearRecognizer(),
            new HolidayRecognizer(),
            new NLPRecognizer(GetNowCentral),
        };
            
        foreach (var recognizer in recognizers)
        {
            if (recognizer.TryRecognize(timeString.ToLower(), out var time))
            {
                return time;
            }
        }

        return Unknown;
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
    
    public required HumanTimeWindowType WindowType { get; init; }
    public required DateOnly StartDate { get; init; }

    public override string ToString()
    {
        string trimesterName = ((StartDate.Month-1) / 4) switch
        {
            0 => "early",
            1 => "mid",
            2 => "late"
        };
        
        return WindowType switch
        {
            HumanTimeWindowType.Month => 
                $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(StartDate.Month)} {StartDate.Year}",
            HumanTimeWindowType.Quarter => $"Q{(StartDate.Month-1)/3 + 1} {StartDate.Year}",
            HumanTimeWindowType.Trimester => $"{trimesterName} {StartDate.Year}",
            HumanTimeWindowType.Year => StartDate.Year.ToString(),
            HumanTimeWindowType.Holiday => $"Holiday {StartDate.Year}",
            HumanTimeWindowType.Unspecified => "TBD",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public sealed record HumanTimeUnknown : HumanTime
{
    public static readonly HumanTimeUnknown Instance = new HumanTimeUnknown();
    public override HumanTimeType TimeType => HumanTimeType.Unknown;

    private HumanTimeUnknown()
    {
    }

    public override string ToString()
    {
        return "Unknown";
    }
}