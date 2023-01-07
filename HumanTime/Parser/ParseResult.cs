namespace HumanTimeParser.Parser;

internal class ParseError : Exception
{
    public ParseError(string message) : base(message)
    {
    }
}

internal record ParseResult;

internal record ParseSuccess(HumanTime Output) : ParseResult;

internal record TimeResult(TimeOnly Time, TimeZoneInfo TimeZone) : ParseResult;

internal record IntResult(int Number) : ParseResult;

internal enum OffsetUnit
{
    Days,
    Months,
    Years,
}

internal record OffsetResult(int Count, OffsetUnit Unit) : ParseResult
{
    public static readonly OffsetResult OneDay = new(1, OffsetUnit.Days);
    public static readonly OffsetResult OneMonth = new(1, OffsetUnit.Months);
    public static readonly OffsetResult OneYear = new(1, OffsetUnit.Years);
    
    public static OffsetResult operator *(int factor, OffsetResult offset)
    {
        return new OffsetResult(factor * offset.Count, offset.Unit);
    }
    
    public static OffsetResult operator *(OffsetResult offset, int factor)
    {
        return factor * offset;
    }
}