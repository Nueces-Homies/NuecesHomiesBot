namespace HumanTimeParser.Parser;

internal record ParseResult;

internal record ParseSuccess(HumanTime Output) : ParseResult;
internal record ParseError(string Reason) : ParseResult;

internal record TimeResult(TimeOnly Time, TimeZoneInfo TimeZone) : ParseResult;
internal record MonthResult(int MonthNumber) : ParseResult;