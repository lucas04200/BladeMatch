namespace BladeMatch.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<MatchResult> Matches { get; set; } = new();
    public bool IsDisqualified { get; set; }
    public int PenaltyPoints { get; set; }

    public void AddMatch(MatchResult match)
    {
        Matches.Add(match);
    }
    
}