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
            yield return new object[] { new[] { "Win", "Draw", "Win" }, 7 }; // 7 points
        }   

        #region test de calcul simple
        // Tests de base 
        [Theory]
        [MemberData(nameof(DataMatchesTest))]
        // Test sans bonus avec MemberData
        public void CalculateScore_WithVariousInput(string[] results, int expectedScore)
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
        [InlineData(new[] { "Win", "Win", "Win", "Loss", "Win", "Win", "Win" }, 28)] // 28
        [InlineData(new[] { "Win", "Win", "Win", "Loss", "Win", "Win", "Win", "Win" }, 31)] // 31
        [InlineData(new[] { "Win", "Win", "Win", "Draw", "Win", "Win", "Win", "Win" }, 32)] // 32
        [InlineData(new[] { "Win", "Win", "Win", "Draw", "Win", "Win", "Win" }, 29)] // 29
        // Test du bonus pour trois victoires consécutives
        public void CalculateScore_AddBonusForConsecutiveWins_ReturnsCorrectResult(string[] result, int expectedScore)
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
        public void Should_Return_Zero_When_Disqualified_With_Positive_Score()
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
        // Test de disqualification avec des résultats négatifs
        public void Should_Return_Zero_When_Disqualified_Without_Fight()
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
        public void Should_Apply_Normal_Penalty() {
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
        public void Should_Apply_Higher_Penalty() {
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
        public void Should_Apply_Equal_Penalty() {
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
        public void Should_Return_Zero_When_Disqualified() {
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
        // Test pour un score non-négatif final
        public void Should_Not_Allow_Negative_Final_Score() {

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
        public void Should_Return_Zero_When_No_Fights() {
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
        public void Should_Return_Zero_When_Fights_Is_Null() {
            // Arrange
            List<MatchResult> matches = null;
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(0);

        }

        [Fact]
        // Test pour une liste vide de résultats
        public void Should_Return_Zero_When_Results_Are_Empty() {
            // Arrange
            var matches = new List<MatchResult>(); // liste vide
            var calculator = new ScoreCalculator();

            // Act
            var score = calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(0);
        }

        [Fact] 
        // Test avec des pénalités négatives 
        public void Should_Not_Allow_Negative_Penalties() {

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
        public void Should_Handle_Long_Tournament_Without_Error()
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
