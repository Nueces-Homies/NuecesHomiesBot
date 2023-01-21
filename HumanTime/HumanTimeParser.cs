using System.Runtime.CompilerServices;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

[assembly: InternalsVisibleTo("HumanTime.Test")]

namespace HumanTime;

// Look at Unit tests to see what kind of inputs the component parsers are looking for
public static class HumanTimeParser
{
    private static readonly Parser<char, Unit> EndOfString = OneOf(SkipWhitespaces, End); 
    
    internal static Parser<char, string> Keyword(string value) =>
        Try(String(value)).Before(EndOfString);

    private static Parser<char, int> Number => Int(@base:10);
    private static readonly Parser<char, int> Integer = Number.Before(EndOfString);

    internal static Parser<char, Unit> Punctuation(char punctuation) =>
        Try(Char(punctuation)).Before(EndOfString).IgnoreResult();

    private static Parser<char, int> GenericMonth(string name, int monthNumber) => Try(Keyword(name).ThenReturn(monthNumber));
    private static readonly Parser<char, int> January =  GenericMonth("january", 1);
    private static readonly Parser<char, int> February = GenericMonth("february", 2);
    private static readonly Parser<char, int> March = GenericMonth("march", 3);
    private static readonly Parser<char, int> April = GenericMonth("april", 4);
    private static readonly Parser<char, int> May = GenericMonth("may", 5);
    private static readonly Parser<char, int> June = GenericMonth("june", 6);
    private static readonly Parser<char, int> July = GenericMonth("july", 7);
    private static readonly Parser<char, int> August = GenericMonth("august", 8);
    private static readonly Parser<char, int> September = GenericMonth("september", 9);
    private static readonly Parser<char, int> October = GenericMonth("october", 10);
    private static readonly Parser<char, int> November = GenericMonth("november", 11);
    private static readonly Parser<char, int> December = GenericMonth("december", 12);

    internal static Parser<char, int> Month => OneOf(
        January, February, March, April, May, June, July, August, September, October, November, December);

    private static readonly Parser<char, Unit> OrdinalSuffix =
        OneOf(Keyword("th"), Keyword("nd"), Keyword("rd"), Keyword("th")).ThenReturn(Unit.Value);

    internal static readonly Parser<char, int> OrdinalNumber = Integer.Before(OrdinalSuffix.Optional());
    
    private static Parser<char, (int, int)> MonthDay => Map((m, d) => (m, d), Month, OrdinalNumber);

    private static Parser<char, (int, int)> DayOfMonth =>
        Map((d, m) => (m, d), OrdinalNumber.Before(Keyword("of").Optional()), Month);

    internal static Parser<char, (int, int)> LongDateWithoutYear => OneOf(MonthDay, DayOfMonth);

    internal static Parser<char, DateOnly> LongDateWithYear => Map(
        (monthDay, y) => new DateOnly(y, monthDay.Item1, monthDay.Item2),
        LongDateWithoutYear.Before(Punctuation(',').Optional().Then(SkipWhitespaces)),
        Integer
    );

    internal static Parser<char, (int, int)> TwoNumbers => Map((num1, num2) => (num1, num2), Number.Before(Punctuation('/')), Number);

    internal static readonly Parser<char, DateOnly> TripleNumbers = Map(
        (month, day, year) => new DateOnly(year, month, day),
        Integer.Before(Punctuation('/')),
        Integer.Before(Punctuation('/')),
        Number);


    private static Parser<char, string> Pluralizable(this Parser<char, string> parser)
    {
        return parser.Before(Char('s').Optional()).Before(EndOfString);
    }
    
    public enum OffsetUnit
    {
        Days,
        Months,
        Years,
    }

    private static readonly Parser<char, OffsetUnit> DaysOffset = String("day").Pluralizable().ThenReturn(OffsetUnit.Days);

    private static readonly Parser<char, OffsetUnit> MonthsOffset =
        String("month").Pluralizable().ThenReturn(OffsetUnit.Months);

    private static readonly Parser<char, OffsetUnit> YearsOffset =
        String("year").Pluralizable().ThenReturn(OffsetUnit.Years);

    internal static readonly Parser<char, OffsetUnit> OffsetUnitParser = OneOf(DaysOffset, MonthsOffset, YearsOffset);

    private static readonly Parser<char, Unit> Article = Keyword("the").Or(Keyword("a")).ThenReturn(Unit.Value);

    internal static readonly Parser<char, int> Count = OneOf(
        Try(Article.Then(Integer)),
        Try(Article.ThenReturn(1)),
        Integer
    );

    private static readonly Parser<char, int> Direction = OneOf(
        Keyword("from").ThenReturn(1),
        Keyword("before").ThenReturn(-1),
        Keyword("after").ThenReturn(1));

    internal static readonly Parser<char, (OffsetUnit, int)> Offset = Map(
        (count, unit, dir) => (unit, count*dir),
        Count.Optional().Select(maybe => maybe.HasValue ? maybe.Value : 1),
        OffsetUnitParser,
        Direction
    );

    private static readonly Parser<char, HumanTime> RelativeDate = Map(
        (offsetSize, baseDate) => baseDate.AddOffset(offsetSize.Item1,  offsetSize.Item2),
        Offset,
        Rec(() => Date)
    );

    internal static readonly Parser<char, HumanTime> Date = OneOf(
        Try(LongDateWithYear.Select(HumanTime.Date)),
        Try(LongDateWithoutYear.Select(monthDay => HumanTime.Date(monthDay.Item1, monthDay.Item2))),
        Try(TripleNumbers.Select(date => (HumanTime) new HumanDate {Date = date})),
        Try(TwoNumbers.Select(monthDay => HumanTime.Date(monthDay.Item1, monthDay.Item2))),
        Try(Keyword("today").Select(_ => HumanTime.Today)),
        Try(Keyword("tomorrow").Select(_ => HumanTime.Today.AddOffset(OffsetUnit.Days, 1))),
        Try(Keyword("yesterday").Select(_ => HumanTime.Today.AddOffset(OffsetUnit.Days, -1))),
        Try(RelativeDate)
    );

    private static HumanTime AddOffset(this HumanTime baseTime, OffsetUnit unit, int number)
    {
        if (baseTime is HumanDate baseDate)
        {
            return HumanTime.Date(unit switch
            {
                OffsetUnit.Days => baseDate.Date.AddDays(number),
                OffsetUnit.Months => baseDate.Date.AddMonths(number),
                OffsetUnit.Years => baseDate.Date.AddYears(number),
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
            });
        }

        throw new NotImplementedException("Need to support offsets for other time types");
    }
}