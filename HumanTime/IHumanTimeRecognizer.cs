namespace HumanTime;

internal interface IHumanTimeRecognizer
{
    internal bool TryRecognize(string query, out HumanTime time);
}