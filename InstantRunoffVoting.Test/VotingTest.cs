namespace InstantRunoffVoting.Test;

public class VotingTest
{
    [Fact]
    public void SimpleElection1()
    {
        var election = new Election();

        var bob = new TestCandidate("Bob");
        var sue = new TestCandidate("Sue");
        var bill = new TestCandidate("Bill");
        
        election.AddCandidate(bob);
        election.AddCandidate(sue);
        election.AddCandidate(bill);
        
        election.AddBallot(new Ballot(new[] {bob, bill, sue})); 
        election.AddBallot(new Ballot(new[] {sue, bob, bill})); 
        election.AddBallot(new Ballot(new[] {bill, sue, bob})); 
        election.AddBallot(new Ballot(new[] {bob, bill, sue})); 
        election.AddBallot(new Ballot(new[] {sue, bob, bill}));

        var rounds = election.RunElection();
        Assert.Equal(2, rounds.Count);

        var lastRound = rounds.Last();
        Assert.Equal(sue, lastRound.GetResults().Winner);
    }
    
    [Fact]
    public void SimpleElection2()
    {
        var election = new Election();

        var alice = new TestCandidate("Alice");
        var bob = new TestCandidate("Bob");
        var claire = new TestCandidate("Claire");
        var david = new TestCandidate("David");
        var eve = new TestCandidate("Eve");
        
        election.AddBallot(new Ballot(new[] {bob, claire, alice, david, eve})); 
        election.AddBallot(new Ballot(new[] {bob, claire, alice, david, eve})); 
        election.AddBallot(new Ballot(new[] {bob, claire, alice, david, eve})); 
        
        election.AddBallot(new Ballot(new[] {claire, alice, david, bob, eve})); 
        election.AddBallot(new Ballot(new[] {claire, alice, david, bob, eve})); 
        election.AddBallot(new Ballot(new[] {claire, alice, david, bob, eve})); 
        election.AddBallot(new Ballot(new[] {claire, alice, david, bob, eve})); 
        
        election.AddBallot(new Ballot(new[] {bob, david, claire, alice, eve})); 
        election.AddBallot(new Ballot(new[] {bob, david, claire, alice, eve})); 
        election.AddBallot(new Ballot(new[] {bob, david, claire, alice, eve})); 
        election.AddBallot(new Ballot(new[] {bob, david, claire, alice, eve})); 
        
        election.AddBallot(new Ballot(new[] {david, claire, alice, eve, bob})); 
        election.AddBallot(new Ballot(new[] {david, claire, alice, eve, bob})); 
        election.AddBallot(new Ballot(new[] {david, claire, alice, eve, bob})); 
        election.AddBallot(new Ballot(new[] {david, claire, alice, eve, bob})); 
        election.AddBallot(new Ballot(new[] {david, claire, alice, eve, bob})); 
        election.AddBallot(new Ballot(new[] {david, claire, alice, eve, bob})); 
        
        election.AddBallot(new Ballot(new[] {bob, eve, alice, claire, david}));
        election.AddBallot(new Ballot(new[] {bob, eve, alice, claire, david}));
        
        election.AddBallot(new Ballot(new[] { eve, alice, david, bob, claire}));

        var rounds = election.RunElection();
        Assert.Equal(4, rounds.Count);

        var lastRound = rounds.Last();
        Assert.Equal(david, lastRound.GetResults().Winner);
    }

    public record TestCandidate (string Name): ICandidate
    {
        public int Id => Name.GetHashCode();
    }
}