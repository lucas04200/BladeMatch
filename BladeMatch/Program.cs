using BladeMatch.Models;
using BladeMatch.Services;

class Program
{
    static void Main(string[] args)
    {
        // Sample players
        var players = new List<Player>
        {
            new Player
            {
                Id = 1,
                Name = "Alice",
                Matches = new List<MatchResult>
                {
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Loss)
                },
                PenaltyPoints = 1,
                IsDisqualified = false
            },
            new Player
            {
                Id = 2,
                Name = "Bob",
                Matches = new List<MatchResult>
                {
                    new MatchResult(MatchResult.Result.Draw),
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Loss)
                },
                PenaltyPoints = 0,
                IsDisqualified = false
            },
            new Player
            {
                Id = 3,
                Name = "Charlie",
                Matches = new List<MatchResult>
                {
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Win),
                    new MatchResult(MatchResult.Result.Win)
                },
                PenaltyPoints = 2,
                IsDisqualified = false
            },
            new Player
            {
                Id = 4,
                Name = "Dave",
                Matches = new List<MatchResult>(),
                PenaltyPoints = 0,
                IsDisqualified = true
            }
        };

        var scoreCalculator = new ScoreCalculator();
        var rankingService = new TournamentRanking(scoreCalculator);

        var rankedPlayers = rankingService.GetRanking(players);
        var champion = rankingService.GetChampion(players);

        Console.WriteLine("--- Tournament Ranking ---");
        foreach (var player in rankedPlayers)
        {
            Console.WriteLine($"{player.Name}: {player.Score} points");
        }

        Console.WriteLine($"\nChampion: {champion.Name} with {champion.Score} points");

        Console.ReadLine();
    }
}
