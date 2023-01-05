namespace HumanTimeParser.Parser;

using System.Xml;

internal class HumanTimeParser : HumanTimeGrammarBaseVisitor<ParseResult>
{
    private static readonly TimeZoneCache TimeZoneCache = new();

    private bool explicitYear = false;
    private bool explicitDate = false;
    
    private static DateTimeOffset Now
    {
        get
        {
            var centralTimeZone = HumanTimeParser.TimeZoneCache["CT"];
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);
        }
    }

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
                if (this.explicitYear == false && date.Date < DateOnly.FromDateTime(DateTime.Now.Date))
                {
                    dateOutput = date.WithYear(Now.Year + 1);
                }

                return new ParseSuccess(dateOutput);
            
            case ParseSuccess {Output: HumanDateTime dateTime}:
                HumanTime dateTimeOutput = dateTime;
                if (this.explicitDate == false && dateTime.DateTime < DateTime.Now.Date)
                {
                    dateTimeOutput = dateTime.WithYear(Now.Year + 1);
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
            case ParseError parseError:
                return parseError;
            case ParseSuccess { Output: HumanDate exactDate}:
                date = exactDate.Date;
                break;
            default:
                return new ParseError($"Date can only be {nameof(HumanDate)}. Got {dateResult.GetType()}");
        }

        TimeOnly time;
        TimeZoneInfo timeZone;
        var timeResult = VisitTime(context.time());
        switch (timeResult)
        {
            case ParseError parseError:
                return parseError;
            case TimeResult {Time: var resultTime, TimeZone: var resultTz}:
                time = resultTime;
                timeZone = resultTz;
                break;
            default:
                return new ParseError($"Time can only be {nameof(TimeResult)}. Got {timeResult.GetType()}");
        }

        return new ParseSuccess(HumanTime.DateTime(date, time, timeZone));
    }

    public override ParseResult VisitDate(HumanTimeGrammarParser.DateContext context)
    {
        var now = HumanTimeParser.Now;
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
        if (!int.TryParse(yearText, out var year))
        {
            return new ParseError($"Unable to parse year {yearText}");
        }

        if (year < 100)
        {
            year += 2000;
        }
        
        ParseResult result = this.VisitLongDateWithoutYear(context.longDateWithoutYear());
        
        switch (result)
        {
            case ParseError error:
                return error;
            case ParseSuccess { Output: HumanDate dateResult}:
                DateOnly date = dateResult.Date;
                this.explicitDate = true;
                return new ParseSuccess(HumanTime.Date(year, date.Month, date.Day));
            default:
                return new ParseError($"Unexpected result. Got {result.GetType()}");
        }
    }

    public override ParseResult VisitLongDateWithoutYear(HumanTimeGrammarParser.LongDateWithoutYearContext context)
    {
        ParseResult result = this.VisitMonth(context.month());

        int month;
        switch (result)
        {
            case ParseError error:
                return error;
            case MonthResult monthNode:
                month = monthNode.MonthNumber;
                break;
            default:
                return new ParseError($"Expected a month. Got {result.GetType()}");
        } 
        
        string dayText = context.INT().GetText();
        if (!int.TryParse(dayText, out var day))
        {
            return new ParseError($"Unable to parse day {dayText}");
        }
        
        int year = HumanTimeParser.Now.Year;

        if (day > DateTime.DaysInMonth(year, month))
        {
            return new ParseError($"{day} is greater than number of days in month {month}");
        }

        this.explicitDate = true;
        return new ParseSuccess(HumanTime.Date(year, month, day));
    }

    public override ParseResult VisitMonth(HumanTimeGrammarParser.MonthContext context)
    {
        if (context.JANUARY() != null) return new MonthResult(1);
        if (context.FEBRUARY() != null) return new MonthResult(2);
        if (context.MARCH() != null) return new MonthResult(3);
        if (context.APRIL() != null) return new MonthResult(4);
        if (context.MAY() != null) return new MonthResult(5);
        if (context.JUNE() != null) return new MonthResult(6);
        if (context.JULY() != null) return new MonthResult(7);
        if (context.AUGUST() != null) return new MonthResult(1);
        if (context.SEPTEMBER() != null) return new MonthResult(9);
        if (context.OCTOBER() != null) return new MonthResult(10);
        if (context.NOVEMBER() != null) return new MonthResult(11);
        if (context.DECEMBER() != null) return new MonthResult(12);

        return new ParseError($"Unrecognized month {context.GetText()}");
    }

    /// <summary>
    /// Turns a string like "5/24" into May 24th and "5/35" into May 2035
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override ParseResult VisitTwoNumbers(HumanTimeGrammarParser.TwoNumbersContext context)
    {
        var intNodes = context.INT();

        int year = HumanTimeParser.Now.Year;
        var monthText = intNodes[0].GetText();
        var dayOrYearText = intNodes[1].GetText();

        if (!int.TryParse(monthText, out var month))
        {
            return new ParseError($"Unable to parse month {monthText}");
        }

        if (month > 12)
        {
            return new ParseError($"There aren't {month} months in a year!");
        }

        if (!int.TryParse(dayOrYearText, out var dayOrYear))
        {
            return new ParseError($"Unable to parse day {dayOrYearText}");
        }

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

        if (!int.TryParse(monthText, out var month))
        {
            return new ParseError($"Unable to parse month {monthText}");
        }

        if (month > 12)
        {
            return new ParseError($"There aren't {month} months in a year");
        }

        if (!int.TryParse(dayText, out var day))
        {
            return new ParseError($"Unable to parse day {dayText}");
        }

        if (!int.TryParse(yearText, out var year))
        {
            return new ParseError($"Unable to parse year {yearText}");
        }

        if (day > DateTime.DaysInMonth(year, month))
        {
            return new ParseError($"Month {month} doesn't have {day} days");
        }

        this.explicitDate = true;
        this.explicitYear = true;
        return new ParseSuccess(HumanTime.Date(year, month, day));
    }

    public override ParseResult VisitQuarter(HumanTimeGrammarParser.QuarterContext context)
    {
        int year = Now.Year;
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
}