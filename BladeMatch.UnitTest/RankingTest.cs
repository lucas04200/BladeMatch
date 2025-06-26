using BladeMatch.Services;
using BladeMatch.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace BladeMatch.UnitTest
{
    public class RankingTest
    {
        private readonly TournamentRanking _rank;

        public RankingTest()
        {
            var calculator = new ScoreCalculator();
            _rank = new TournamentRanking(calculator);
        }

        [Fact(DisplayName = "Classement par score décroissant")]
        public void GetRanking_WithValidMatches_ShouldReturnCorrectOrder()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player
                {
                    Name = "Alice",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Draw),  // 1 pt
                        new MatchResult(MatchResult.Result.Loss)   // 0 pt
                    }
                },
                new Player
                {
                    Name = "Bob",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Win),   // 3 pt
                        new MatchResult(MatchResult.Result.Win)    // 3 pt
                    }
                },
                new Player
                {
                    Name = "Charlie",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Win),   // 3 pt
                        new MatchResult(MatchResult.Result.Draw)   // 1 pt
                    }
                }
            };

            // Act
            var ranking = _rank.GetRanking(players);

            // Assert
            Assert.Equal("Bob", ranking[0].Name);       // 6 pts
            Assert.Equal("Charlie", ranking[1].Name);   // 4 pts
            Assert.Equal("Alice", ranking[2].Name);     // 1 pt
        }


        [Fact(DisplayName = "Classement avec égalité de score")]
        public void GetRanking_WithEqualScores_ShouldHandleTie()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player
                {
                    Name = "Alice",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Win),
                        new MatchResult(MatchResult.Result.Loss)
                    }
                },
                new Player
                {
                    Name = "Bob",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Loss),
                        new MatchResult(MatchResult.Result.Win)
                    }
                },
                new Player
                {
                    Name = "Charlie",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Loss),
                        new MatchResult(MatchResult.Result.Loss)
                    }
                }
            };

            // Act
            var ranking = _rank.GetRanking(players);

            // Assert
            Assert.Equal(3, players[0].Score); // Alice
            Assert.Equal(3, players[1].Score); // Bob
            Assert.Equal(0, players[2].Score); // Charlie

            Assert.Contains(ranking, p => p.Name == "Alice");
            Assert.Contains(ranking, p => p.Name == "Bob");
            Assert.Equal("Alice", ranking[0].Name);
            Assert.Equal("Bob", ranking[1].Name);
            Assert.Equal("Charlie", ranking[2].Name);
        }

        [Fact(DisplayName = "Champion avec meilleur score")]
        public void GetChampion_WithValidMatches_ShouldReturnTopPlayer()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player
                {
                    Name = "Alice",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Win)
                    }
                },
                new Player
                {
                    Name = "Bob",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Win),
                        new MatchResult(MatchResult.Result.Win)
                    }
                }
            };

            // Act
            var champion = _rank.GetChampion(players);

            // Assert
            Assert.Equal("Bob", champion.Name); // 6 pts contre 3
        }

        [Fact(DisplayName = "Classement avec joueurs disqualifiés (0 point)")]
        public void GetRanking_WithOnlyLosses_ShouldReturnZeroScores()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player
                {
                    Name = "Alice",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Loss)
                    }
                },
                new Player
                {
                    Name = "Bob",
                    Matches = new List<MatchResult>
                    {
                        new MatchResult(MatchResult.Result.Loss),
                        new MatchResult(MatchResult.Result.Loss)
                    }
                }
            };

            // Act
            var ranking = _rank.GetRanking(players);

            // Assert
            Assert.All(ranking, p => Assert.Equal(0, p.Score));
        }


        [Fact(DisplayName = "Classement avec joueurs vide")]
        public void GetRanking_WithEmptyPlayers_ShouldThrowException()
        {
            // Arrange
            var players = new List<Player>();
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _rank.GetRanking(players));
            Assert.Equal("No players found", exception.Message);
        }

        [Fact(DisplayName = "Champion avec joueurs vide")]
        public void GetChampion_WithEmptyPlayers_ShouldThrowException()
        {
            // Arrange
            var players = new List<Player>();
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _rank.GetChampion(players));
            Assert.Equal("No players found", exception.Message);
        }

    }
}
