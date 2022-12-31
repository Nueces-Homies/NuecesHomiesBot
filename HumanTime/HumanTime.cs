namespace HumanTimeParser;

public enum HumanTimeType
{
    Date,
    DateTime,
}

public abstract class HumanTime
{
}

public class HumanDate : HumanTime
{
    public required DateOnly Date { get; init; }
}

public class HumanDateTime : HumanTime
{
    public required DateTimeOffset DateTime { get; init; }
}