using Core.Common.HumanTime;

namespace Core.Test;

using Xunit.Abstractions;

public class HumanTimeTestCase : IXunitSerializable
{
    public string? InputString { get; private set; }
    public Common.HumanTime.HumanTime? Expected { get; private set; }
    
    public HumanTimeTestCase()
    {
    }

    public HumanTimeTestCase(string input, Common.HumanTime.HumanTime expected)
    {
        this.InputString = input;
        this.Expected = expected;
    }
        
    public void Deserialize(IXunitSerializationInfo info)
    {
        InputString = info.GetValue<string>("Input");

        string value = info.GetValue<string>("Value");
        var timeType = info.GetValue<HumanTimeType>("TimeType");
        Expected = timeType switch
        {
            HumanTimeType.Date     => new HumanDate { Date = DateOnly.Parse(value)},
            HumanTimeType.DateTime => new HumanDateTime { DateTime = DateTimeOffset.Parse(value)},
            HumanTimeType.Window => new HumanTimeWindow
            {
                WindowType = info.GetValue<HumanTimeWindowType>("WindowType"),
                StartDate = DateOnly.Parse(value),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        Assert.NotNull(InputString);
        Assert.NotNull(Expected);
            
        info.AddValue("Input", InputString);
        info.AddValue("TimeType", Expected.TimeType);

        switch (Expected)
        {
            case HumanDate {Date: var date}:
                info.AddValue("Value", date.ToString());
                break;
            case HumanDateTime {DateTime: var dto}:
                info.AddValue("Value", dto.ToString());
                break;
            case HumanTimeWindow window:
                info.AddValue("Value", window.StartDate.ToString());
                info.AddValue("WindowType", window.WindowType);
                break;
        }
    }

    public override string ToString()
    {
        return $"""Input: "{InputString}" => Output: {Expected}""";
    }
}