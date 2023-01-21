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

    internal static Parser<char, Unit> Punctuation(char punctuation) =>
        Try(Char(punctuation)).Before(EndOfString).ThenReturn(Unit.Value);

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

    internal static readonly Parser<char, int> OrdinalNumber = Number.Before(OrdinalSuffix.Optional());

    private static readonly Parser<char, int> Integer = Number.Before(EndOfString);
    
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
        Number.Before(Punctuation('/')),
        Number.Before(Punctuation('/')),
        Number);
}