namespace InstantRunoffVoting;

public class Round
{
    public Dictionary<HashedCandidate, int> Counts { get; }
    private List<Ballot> Ballots { get; init; }

    public Round(List<Ballot> ballots)
    {
        this.Ballots = ballots;
        this.Counts = new();
    }

    public void RunRound()
    {
        foreach (Ballot ballot in Ballots)
        {
            if (ballot.Preferences.Count <= 0)
            {
                continue;
            }

            var preferredCandidate = new HashedCandidate(ballot.Preferences[0]);
            this.Counts[preferredCandidate] = this.Counts.GetValueOrDefault(preferredCandidate, 0) + 1;
        }
    }

    public Round GenerateNextRound()
    {
        Result result = GetResults();
        return new Round(Ballots.Select(b => b.WithoutCandidate(result.Loser)).ToList());
    }

    public Result GetResults()
    {
        HashedCandidate? winner = null;
        HashSet<HashedCandidate> losers = new();
        int minCount = int.MaxValue;
        int maxCount = int.MinValue;

        int totalCount = 0;
        
        foreach (var kvp in Counts)
        {
            if (kvp.Value > maxCount)
            {
                winner = kvp.Key;
                maxCount = kvp.Value;
            }

            if (kvp.Value == minCount)
            {
                losers.Add(kvp.Key);
            }
            else if (kvp.Value < minCount)
            {
                losers = new HashSet<HashedCandidate> { kvp.Key };
                minCount = kvp.Value;
            }

            totalCount += kvp.Value;
        }

        var randomLoser = losers.ElementAt(Random.Shared.Next(losers.Count));
        
        return new Result(winner!.Candidate, maxCount, randomLoser.Candidate, totalCount);
    }

    public record Result(ICandidate Winner, int WinnerCount, ICandidate Loser, int TotalValidBallots)
    {
        public bool WinnerHasMajority => WinnerCount > TotalValidBallots / 2;
    }
}