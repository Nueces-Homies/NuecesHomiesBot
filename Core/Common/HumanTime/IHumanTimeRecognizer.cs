namespace Core.Common.HumanTime;

internal interface IHumanTimeRecognizer
{
    internal bool TryRecognize(string query, out Common.HumanTime.HumanTime time);
}