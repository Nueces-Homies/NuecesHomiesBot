namespace InstantRunoffVoting;

public interface ICandidate
{
    public int Id { get; }
}

public class HashedCandidate
{
    public ICandidate Candidate { get; }
    public override int GetHashCode() => this.Candidate.Id;

    public override bool Equals(object? obj)
    {
        if (obj is HashedCandidate other)
        {
            return other.Candidate.Id == this.Candidate.Id;
        }

        return false;
    }

    public HashedCandidate(ICandidate c) => this.Candidate = c;
}

