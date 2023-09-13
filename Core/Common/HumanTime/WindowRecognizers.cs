using System.Text.RegularExpressions;

namespace Core.HumanTime;

public class QuarterRecognizer : IHumanTimeRecognizer
{
    private readonly Regex regex = new(@"^q(1|2|3|4) (\d\d\d\d)$", RegexOptions.Compiled);

    public bool TryRecognize(string query, out HumanTime time)
    {
        var match = regex.Match(query);
        if (!match.Success)
        {
            time = HumanTime.Unknown;
            return false;
        }

        var quarterGroup = match.Groups[1];
        var quarter = int.Parse(quarterGroup.Value); 

        var yearGroup = match.Groups[2];
        var year = int.Parse(yearGroup.Value);

        time = HumanTime.Quarter(quarter, year);
        return true;
    }
}

public class TrimesterRecognizer : IHumanTimeRecognizer
{
    private readonly Regex regex = new(@"^((early)|(mid)|(late)) (\d\d\d\d)$", RegexOptions.Compiled);

    public bool TryRecognize(string query, out HumanTime time)
    {
        var match = regex.Match(query);
        if (!match.Success)
        {
            time = HumanTime.Unknown;
            return false;
        }

        var trimesterGroup = match.Groups[1];
        var trimester = trimesterGroup.Value switch
        {
            "early" => 1,
            "mid" => 2,
            "late" => 3,
            _ => 0
        };

        if (trimester == 0)
        {
            time = HumanTime.Unknown;
            return false;
        }

        var yearGroup = match.Groups[5];
        var year = int.Parse(yearGroup.Value);

        time = HumanTime.Trimester(trimester, year);
        return true;
    }
}

public class HolidayRecognizer : IHumanTimeRecognizer
{
    private readonly Regex regex = new(@"^holiday (\d\d\d\d)$", RegexOptions.Compiled);

    public bool TryRecognize(string query, out HumanTime time)
    {
        var match = regex.Match(query);
        if (!match.Success)
        {
            time = HumanTime.Unknown;
            return false;
        }

        var yearGroup = match.Groups[1];
        var year = int.Parse(yearGroup.Value);

        time = HumanTime.Holiday(year);
        return true;
    }
}

public class YearRecognizer : IHumanTimeRecognizer
{
    private readonly Regex regex = new(@"^(\d\d\d\d)$", RegexOptions.Compiled);

    public bool TryRecognize(string query, out HumanTime time)
    {
        var match = regex.Match(query);
        if (!match.Success)
        {
            time = HumanTime.Unknown;
            return false;
        }

        var yearGroup = match.Groups[1];
        var year = int.Parse(yearGroup.Value);

        time = HumanTime.Year(year);
        return true;
    }
}