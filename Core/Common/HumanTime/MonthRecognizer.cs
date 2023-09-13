using System.Text.RegularExpressions;

namespace Core.HumanTime;

public class MonthRecognizer : IHumanTimeRecognizer
{
    private readonly Regex regex = new(@"^([a-z]+)( (\d\d\d\d))?$", RegexOptions.Compiled);

    public bool TryRecognize(string query, out HumanTime time)
    {
        var match = regex.Match(query);
        if (!match.Success)
        {
            time = HumanTime.Unknown;
            return false;
        }

        var monthGroup = match.Groups[1];
        
        var month = monthGroup.Value switch
        {
            "january" => 1,
            "february" => 2,
            "march" => 3,
            "april" => 4,
            "may" => 5,
            "june" => 6,
            "july" => 7,
            "august" => 8,
            "september" => 9,
            "october" => 10,
            "november" => 11,
            "december" => 12,
            _ => 0,
        };

        if (month == 0)
        {
            time = HumanTime.Unknown;
            return false;
        }

        if (match.Groups[2].Value != "")
        {
            var yearGroup = match.Groups[2];
            var year = int.Parse(yearGroup.Value);
            time = HumanTime.Month(year, month);
        }
        else
        {
            time = HumanTime.Month(month);
        }
        
        return true;
    }
}