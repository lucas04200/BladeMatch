using BladeMatch.Models;
using BladeMatch.Services;
using FluentAssertions; 
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BladeMatch.UnitTest
{
    public class CalculTest
    {
        private readonly ScoreCalculator _calculator;

        public CalculTest()
        {
            _calculator = new ScoreCalculator();
        }


        // DataMatches pour les tests
        public static IEnumerable<object[]> DataMatchesTest()
        {
            yield return new object[] { new[] { "Win", "Draw", "Loss" }, 4 }; // 4 points
            yield return new object[] { new[] { "Win", "Win", "Draw" }, 7 }; // 6 + 1 draw
            yield return new object[] { new[] { "Loss", "Loss", "Loss" }, 0 }; // 0 points
            yield return new object[] { new[] { "Win", "Win" }, 6 }; // 9 points
            yield return new object[] { new[] { "Win", "Draw", "Win" }, 7 }; // 7 points
        }

        #region test de calcul simple
        // Tests de base 
        [Theory]
        [MemberData(nameof(DataMatchesTest))]
        // Test sans bonus avec MemberData
        public void CalculateScore_WithVariousInput_ReturnsExcpected(string[] results, int expectedScore)
        {
            var matches = results.Select(r => new MatchResult(
                r switch
                {
                    "Win" => MatchResult.Result.Win,
                    "Draw" => MatchResult.Result.Draw,
                    "Loss" => MatchResult.Result.Loss,
                    _ => throw new ArgumentException("Invalid result")
                }
            )).ToList();

            var score = _calculator.CalculateScore(matches);
            score.Should().Be(expectedScore);
        }
        #endregion

        #region Test du bonus de series 

        [Theory]
        [InlineData(new[] { "Win", "Win", "Win" }, 14)] // 9 points + 5 bonus
        [InlineData(new[] { "Win", "Win", "Win", "Win" }, 17)] // 17 points 
        [InlineData(new[] { "Win", "Win", "Loss", "Win" }, 9)] //  9 points
        [InlineData(new[] { "Win", "Draw", "Win", "Win" }, 10)] // 9 points
        [InlineData(new[] { "Win", "Win", "Win", "Loss", "Win", "Win", "Win" }, 28)] // 28
        [InlineData(new[] { "Win", "Win", "Win", "Loss", "Win", "Win", "Win", "Win" }, 31)] // 31
        [InlineData(new[] { "Win", "Win", "Win", "Draw", "Win", "Win", "Win", "Win" }, 32)] // 32
        [InlineData(new[] { "Win", "Win", "Win", "Draw", "Win", "Win", "Win" }, 29)] // 29
        // Test du bonus pour trois victoires consécutives
        public void CalculateScore_AddBonusForConsecutiveWins_ReturnsExpected(string[] result, int expectedScore)
        {
            var matches = result.Select(r => new MatchResult(
                r switch
                {
                    "Win" => MatchResult.Result.Win,
                    "Draw" => MatchResult.Result.Draw,
                    "Loss" => MatchResult.Result.Loss,
                    _ => throw new ArgumentException("Invalid result")
                }
            )).ToList();


            var score = _calculator.CalculateScore(matches);
            score.Should().Be(expectedScore);
        }

        #endregion

        #region Test de disqualification

        [Fact]
        // Test de disqualification avec des résultats positifs
        public void CalculateScore_WhenDisqualified_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Win),
                new MatchResult(MatchResult.Result.Draw),
                new MatchResult(MatchResult.Result.Loss)
            };

            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: true);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        // Test de disqualification sans aucun match
        public void CalculateScore_WhenDisqualifiedAndZeroMatch_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>(); // Aucun match
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: true);

            // Assert
            score.Should().Be(0);
        }


        #endregion

        #region Test des pénalités 

        [Fact]
        // Test des pénalités normales
        public void CalculateScore_WithOnePenalityPoint_ReturnsThree()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Win),
                new MatchResult(MatchResult.Result.Draw) // 3 + 1 = 4
            };
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: false, penaltyPoints: 1);

            // Assert
            score.Should().Be(3);
        }

        [Fact]
        // Test des pénalités supérieures
        public void CalculateScore_WithHigherPenalityPoint_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Win),
                new MatchResult(MatchResult.Result.Win) // 6 points
            };
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: false, penaltyPoints: 10);

            // Assert
            score.Should().Be(0); // Score jamais négatif
        }

        [Fact]
        // Test des pénalités égales
        public void CalculateScore_WithEqualPenalityPointAndScore_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Win),
                new MatchResult(MatchResult.Result.Loss) // 3 points
            };
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: false, penaltyPoints: 3);

            // Assert
            score.Should().Be(0);
        }

        #endregion

        #region Tests des cas limites

        [Fact]
        // Test pour un score final de zéro quand disqualified
        public void CalculateScore_WithDisqualification_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Win),
                new MatchResult(MatchResult.Result.Draw)
            };
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: true);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        // Test pour un score négatif final
        public void CalculateScore_WithNegativeFinalScore_ReturnsZero()
        {

            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Draw) // 1 point
            };
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches, isDisqualified: false, penaltyPoints: 5);

            // Assert
            score.Should().Be(0); // Score jamais négatif
        }

        [Fact]
        // Test pour une liste vide de combats
        public void CalculateScore_WithZeroMatche_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>(); // liste vide
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        // Test pour une liste null de combats
        public void CalculateScore_WithNullMatche_ReturnsZero()
        {
            // Arrange
            List<MatchResult> matches = null;
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(0);

        }

        [Fact]
        // Test avec des pénalités négatives 
        public void CalculateScore_WithNegativesPenalities_ReturnsArgumentException()
        {

            // Arrange
            var matches = new List<MatchResult>
            {
                new MatchResult(MatchResult.Result.Win),
                new MatchResult(MatchResult.Result.Draw) // score = 3 + 1 = 4
            };
            var calculator = new ScoreCalculator();

            // Act
            Action act = () => calculator.CalculateScore(matches, isDisqualified: false, penaltyPoints: -5);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*penalty*");
        }


        [Fact]
        // Test pour un très long tournoi (>100 combats)    
        public void CalculateScore_WithLongTournament_ReturnsZero()
        {
            // Arrange
            var matches = new List<MatchResult>();
            for (int i = 0; i < 120; i++)
            {
                // Alternance entre victoires, matchs nuls et défaites
                matches.Add(new MatchResult(i % 3 == 0 ? MatchResult.Result.Win :
                                             i % 3 == 1 ? MatchResult.Result.Draw :
                                                          MatchResult.Result.Loss));
            }

            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches);

            // Assert
            score.Should().BeGreaterThanOrEqualTo(0); // Le score doit être non-négatif
        }
        #endregion

    }
}
