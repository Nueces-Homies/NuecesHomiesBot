namespace InstantRunoffVoting;

public class Ballot
{
    public IList<ICandidate> Preferences { get; init; }
    
    public Ballot(IEnumerable<ICandidate> preferences)
    {
        this.Preferences = preferences.ToList();
    }

    public Ballot WithoutCandidate(ICandidate rejectedCandidate)
    {
        return new Ballot(this.Preferences.Where(c => c.Id != rejectedCandidate.Id));
    }
}