using BladeMatch.Models;

namespace BladeMatch.Services;

public class ScoreCalculator
{
    /// <summary>
    /// Calcule le score final d'un joueur selon les règles du tournoi
    /// </summary>
    /// <param name="matches">Liste des résultats de combat dans l'ordre chronologique</param>
    /// <param name="isDisqualified">True si le joueur est disqualifié</param>
    /// <param name="penaltyPoints">Points de pénalité (nombre positif)</param>
    /// <returns>Score final (jamais négatif)</returns>
    
    public int CalculateScore(List<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
    {
        if (isDisqualified) {return 0;}
        if  (matches.Count == 0) {return 0;}

        int score = 0;
        foreach (var match in matches)
        {
            int winStreak = 0;
            switch (match.Outcome)
            {
                case MatchResult.Result.Win:
                    score += 3;
                    winStreak++;
                    break;
                case MatchResult.Result.Draw:
                    score += 1;
                    winStreak = 0;
                    break;
                case MatchResult.Result.Loss:
                    winStreak = 0;
                    break;
                default:
                    break;
            }

            if (winStreak == 3)
            {
                score += 5;
            } 
        }
        return score;
    }

}