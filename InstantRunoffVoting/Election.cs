namespace InstantRunoffVoting;

public class Election
{
    public List<ICandidate> Candidates { get; } = new();
    public List<Ballot> InitialBallots { get; } = new();
    
    public void AddCandidate(ICandidate candidate)
    {
        Candidates.Add(candidate);    
    }

    public void AddBallot(Ballot ballot)
    {
        this.InitialBallots.Add(ballot);
    }

    public List<Round> RunElection()
    {
        var rounds = new List<Round> { new (InitialBallots) };

        while (true)
        {
            var currentRound = rounds.Last();
            currentRound.RunRound();
            if (currentRound.GetResults().WinnerHasMajority)
            {
                return rounds;
            }
            
            rounds.Add(currentRound.GenerateNextRound());
        }
    }
}