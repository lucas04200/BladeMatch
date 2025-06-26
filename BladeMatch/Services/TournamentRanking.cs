using BladeMatch.Models;

namespace BladeMatch.Services;

public class TournamentRanking
{
    private readonly ScoreCalculator _scoreCalculator;
    public TournamentRanking(ScoreCalculator scoreCalculator)
    {
        _scoreCalculator = scoreCalculator;
    }
    /// <summary>
    /// Classe les joueurs par score décroissant
    /// </summary>
    public List<Player> GetRanking(List<Player> players)
    {
        if (players.Count == 0)
        {
            throw new Exception("No players found");
        }

        foreach (var player in players)
        {
            player.Score = _scoreCalculator.CalculateScore(player.Matches);
        }
        
        players.Sort((a, b) => b.Score.CompareTo(a.Score));
        return players;
    }
    /// <summary>
    /// Trouve le champion (joueur avec le meilleur score)
    /// </summary>
    public Player GetChampion(List<Player> players)
    {
        if (players.Count == 0)
        {
            throw new Exception("No players found");
        }
        
        return GetRanking(players)[0];
    }

}