namespace HumanTimeParser.Parser;

using System.Xml;

internal class HumanTimeParser : HumanTimeGrammarBaseVisitor<ParseResult>
{
    private static readonly TimeZoneCache TimeZoneCache = new();

    private bool explicitYear = false;
    private bool explicitDate = false;

    private static readonly Lazy<DateTimeOffset> Now = new (() =>
    {
        var centralTimeZone = HumanTimeParser.TimeZoneCache["CT"];
        var dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);
        var rounded = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        var offset = centralTimeZone.GetUtcOffset(rounded);
        return new DateTimeOffset(rounded, offset);
    });

    protected override ParseResult AggregateResult(ParseResult? aggregate, ParseResult? nextResult)
    {
        if (aggregate == null && nextResult != null)
            return nextResult;
        if (nextResult == null && aggregate != null)
            return aggregate;
        
        throw new Exception($"Not sure how to aggregate {aggregate} and {nextResult}");
    }

    public override ParseResult VisitInput(HumanTimeGrammarParser.InputContext context)
    {
        var result = VisitChildren(context);

        switch (result)
        {
            case ParseSuccess {Output: HumanDate date}:
                HumanTime dateOutput = date;
                if (this.explicitYear == false && date.Date < DateOnly.FromDateTime(Now.Value.Date))
                {
                    dateOutput = date.WithYear(Now.Value.Year + 1);
                }

                return new ParseSuccess(dateOutput);
            
            case ParseSuccess {Output: HumanDateTime dateTime}:
                HumanTime dateTimeOutput = dateTime;
                if (this.explicitDate == false && dateTime.DateTime < Now.Value.Date)
                {
                    dateTimeOutput = dateTime.WithYear(HumanTimeParser.Now.Value.Year + 1);
                }

                return new ParseSuccess(dateTimeOutput);
            
            default:
                return result;
        }
        
    }

    public override ParseResult VisitDatetime(HumanTimeGrammarParser.DatetimeContext context)
    {
        DateOnly date;
        var dateResult = VisitDate(context.date());
        switch (dateResult)
        {
            case ParseSuccess { Output: HumanDate exactDate}:
                date = exactDate.Date;
                break;
            default:
                throw new ParseError($"Date can only be {nameof(HumanDate)}. Got {dateResult.GetType()}");
        }

        TimeOnly time;
        TimeZoneInfo timeZone;
        var timeResult = VisitTime(context.time());
        switch (timeResult)
        {
            case TimeResult {Time: var resultTime, TimeZone: var resultTz}:
                time = resultTime;
                timeZone = resultTz;
                break;
            default:
                throw new ParseError($"Time can only be {nameof(TimeResult)}. Got {timeResult.GetType()}");
        }

        return new ParseSuccess(HumanTime.DateTime(date, time, timeZone));
    }

    public override ParseResult VisitDate(HumanTimeGrammarParser.DateContext context)
    {
        var now = HumanTimeParser.Now.Value;
        var today = DateOnly.FromDateTime(now.DateTime);

        if (context.NOW() != null)
        {
            this.explicitDate = true;
            this.explicitYear = true;
            return new ParseSuccess(new HumanDateTime { DateTime = now });
        }
        
        if (context.TODAY() != null)
        {
            this.explicitDate = true;
            this.explicitYear = true;
            return new ParseSuccess(new HumanDate { Date = today });
        }

        if (context.YESTERDAY() != null)
        {
            this.explicitDate = true;
            this.explicitYear = true;
            return new ParseSuccess(new HumanDate { Date = today.AddDays(-1) });
        }

        if (context.TOMORROW() != null)
        {
            this.explicitDate = true;
            this.explicitYear = true;
            return new ParseSuccess(new HumanDate { Date = today.AddDays(1) });
        }

        return VisitChildren(context);
    }

    public override ParseResult VisitLongDateWithYear(HumanTimeGrammarParser.LongDateWithYearContext context)
    {
        string yearText = context.INT().GetText();
        var year = int.Parse(yearText);
            
        if (year < 100)
        {
            year += 2000;
        }
        
        ParseResult result = this.VisitLongDateWithoutYear(context.longDateWithoutYear());
        
        switch (result)
        {
            case ParseSuccess { Output: HumanDate dateResult}:
                DateOnly date = dateResult.Date;
                this.explicitYear = true;
                this.explicitDate = true;
                return new ParseSuccess(HumanTime.Date(year, date.Month, date.Day));
            default:
                throw new ParseError($"Unexpected result. Got {result.GetType()}");
        }
    }

    public override ParseResult VisitLongDateWithoutYear(HumanTimeGrammarParser.LongDateWithoutYearContext context)
    {
        ParseResult result = this.VisitMonth(context.month());

        int month;
        int year;
        switch (result)
        {
            case ParseSuccess {Output: HumanTimeWindow {WindowType: HumanTimeWindowType.Month, StartDate: var startDate} }:
                month = startDate.Month;
                year = startDate.Year;
                break;
            default:
                throw new ParseError($"Expected a month. Got {result.GetType()}");
        } 
        
        string dayText = context.INT().GetText();
        var day = int.Parse(dayText);
        
        if (day > DateTime.DaysInMonth(year, month))
        {
            throw new ParseError($"{day} is greater than number of days in month {month}");
        }

        this.explicitDate = true;
        return new ParseSuccess(HumanTime.Date(year, month, day));
    }

    public override ParseResult VisitMonth(HumanTimeGrammarParser.MonthContext context)
    {
        int year = Now.Value.Year;
        if (context.JANUARY() != null) return new ParseSuccess(HumanTime.Month(year, 1));
        if (context.FEBRUARY() != null) return new ParseSuccess(HumanTime.Month(year, 2));
        if (context.MARCH() != null) return new ParseSuccess(HumanTime.Month(year, 3));
        if (context.APRIL() != null) return new ParseSuccess(HumanTime.Month(year, 4));
        if (context.MAY() != null) return new ParseSuccess(HumanTime.Month(year, 5));
        if (context.JUNE() != null) return new ParseSuccess(HumanTime.Month(year, 6));
        if (context.JULY() != null) return new ParseSuccess(HumanTime.Month(year, 7));
        if (context.AUGUST() != null) return new ParseSuccess(HumanTime.Month(year, 8));
        if (context.SEPTEMBER() != null) return new ParseSuccess(HumanTime.Month(year, 9));
        if (context.OCTOBER() != null) return new ParseSuccess(HumanTime.Month(year, 10));
        if (context.NOVEMBER() != null) return new ParseSuccess(HumanTime.Month(year, 11));
        if (context.DECEMBER() != null) return new ParseSuccess(HumanTime.Month(year, 12));

        throw new ParseError($"Unrecognized month {context.GetText()}");
    }

    /// <summary>
    /// Turns a string like "5/24" into May 24th and "5/35" into May 2035
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override ParseResult VisitTwoNumbers(HumanTimeGrammarParser.TwoNumbersContext context)
    {
        var intNodes = context.INT();

        int year = HumanTimeParser.Now.Value.Year;
        var monthText = intNodes[0].GetText();
        var dayOrYearText = intNodes[1].GetText();

        var month = int.Parse(monthText);

        if (month > 12)
        {
            throw new ParseError($"There aren't {month} months in a year!");
        }

        var dayOrYear = int.Parse(dayOrYearText);

        if (dayOrYear > DateTime.DaysInMonth(year, month))
        {
            year = dayOrYear < 100 ? dayOrYear + 2000 : dayOrYear;
            this.explicitYear = true;
            return new ParseSuccess(HumanTime.Month(year, month));
        }

        this.explicitDate = true;
        return new ParseSuccess(HumanTime.Date(year, month, dayOrYear));
    }
    
    public override ParseResult VisitShortDateWithYear(HumanTimeGrammarParser.ShortDateWithYearContext context)
    {
        var intNodes = context.INT();

        var monthText = intNodes[0].GetText();
        var dayText = intNodes[1].GetText();
        var yearText = intNodes[2].GetText();

        var month = int.Parse(monthText);

        if (month > 12)
        {
            throw new ParseError($"There aren't {month} months in a year");
        }

        var day = int.Parse(dayText);
        var year = int.Parse(yearText);
        
        if (day > DateTime.DaysInMonth(year, month))
        {
            throw new ParseError($"Month {month} doesn't have {day} days");
        }

        this.explicitDate = true;
        this.explicitYear = true;
        return new ParseSuccess(HumanTime.Date(year, month, day));
    }

    public override ParseResult VisitQuarter(HumanTimeGrammarParser.QuarterContext context)
    {
        int year = Now.Value.Year;
        int quarter = 0;
        
        if (context.Q1() != null)
        {
            quarter = 1;
        }
        else if (context.Q2() != null)
        {
            quarter = 2;
        }
        else if (context.Q3() != null)
        {
            quarter = 3;
        }
        else if (context.Q4() != null)
        {
            quarter = 4;
        }

        return new ParseSuccess(HumanTime.Quarter(quarter, year));
    }

    public override ParseResult VisitDateOffsetUnit(HumanTimeGrammarParser.DateOffsetUnitContext context)
    {
        if (context.DAYS() != null) return OffsetResult.OneDay;
        if (context.MONTHS() != null) return OffsetResult.OneMonth;
        if (context.YEARS() != null) return OffsetResult.OneYear;

        throw new ParseError("Unable to determine offset unit");
    }

    public override ParseResult VisitDateOffsetCount(HumanTimeGrammarParser.DateOffsetCountContext? context)
    {
        if (context == null) return new IntResult(1);
        
        return context.INT() == null ? 
                   new IntResult(1) : 
                   new IntResult(int.Parse(context.INT().GetText()));
    }

    public override ParseResult VisitOffsetDirection(HumanTimeGrammarParser.OffsetDirectionContext context)
    {
        if (context.FROM() != null || context.AFTER() != null)
        {
            return new IntResult(1);
        }
        
        if (context.BEFORE() != null)
        {
            return new IntResult(-1);
        }

        throw new ParseError("Unable to determine offset direction");
    }

    public override ParseResult VisitDateOffset(HumanTimeGrammarParser.DateOffsetContext context)
    {
        int count;
        var countResult = VisitDateOffsetCount(context.dateOffsetCount());
        switch (countResult)
        {
            case IntResult { Number: var number }:
                count = number;
                break;
            default: throw new ParseError($"Unknown type {countResult.GetType()}");
        }

        var directionResult = (IntResult) VisitOffsetDirection(context.offsetDirection());
        int direction = directionResult.Number;

        var unit = (OffsetResult)VisitDateOffsetUnit(context.dateOffsetUnit());

        return (direction * count) * unit;
    }

    public override ParseResult VisitRelativeDate(HumanTimeGrammarParser.RelativeDateContext context)
    {
        var dateResult = (ParseSuccess)VisitDate(context.date());
        var offset = (OffsetResult) VisitDateOffset(context.dateOffset());

        switch (dateResult.Output)
        {
            case HumanTimeWindow window:
                throw new ParseError($"Need more specific time than {window}");
            
            case HumanDateTime { DateTime: var dto}:
                return new ParseSuccess(new HumanDateTime{
                    DateTime = offset.Unit switch
                    {
                        OffsetUnit.Days   => dto.AddDays(offset.Count),
                        OffsetUnit.Months => dto.AddMonths(offset.Count),
                        OffsetUnit.Years  => dto.AddYears(offset.Count),
                        _                 => throw new ParseError("Unknown units")
                    },
                });
                    
            case HumanDate {Date: var date}:
                return new ParseSuccess(new HumanDate{
                    Date = offset.Unit switch
                    {
                        OffsetUnit.Days   => date.AddDays(offset.Count),
                        OffsetUnit.Months => date.AddMonths(offset.Count),
                        OffsetUnit.Years  => date.AddYears(offset.Count),
                        _                 => throw new ParseError("Unknown units")
                    },
                });
            
            default:
                throw new ParseError("Unexpected date result");
        }
    }
}