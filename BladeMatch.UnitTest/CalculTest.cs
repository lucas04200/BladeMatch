using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions; 

namespace BladeMatch.UnitTest
{
    public class CalculTest
    {
        private readonly ScoreCalculator _calculator;

        public ScoreCalculatorTests()
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
        public void Should_Calculate_Score_Without_Bonus() { }

        [Fact]
        // Test avec que des victoires sans bonus
        public void Should_Calculate_Score_With_Two_Wins() { }

        [Fact]
        // Test avec que des nulls
        public void Should_Return_Three_When_No_Results() { }
        
        [Fact]
        // Test avec que des défaites 
        public void Should_Return_Zero_When_All_Losses() { }
        
        // Tests avec données paramétrées 
        [Theory]
        [InlineData(new[] { "Win", "Win", "Win" }, 14)] // 9 points + 5 bonus
        [InlineData(new[] { "Win", "Draw", "Win" }, 7)]  // 7 points, pas de bonus
        public void Should_Calculate_Score_Correctly(string[] results, intexpectedScore){ }

        #endregion

        #region Test du bonus de series 

        [Fact]
        // Tests avec bonus
        public void Should_Add_Bonus_For_Three_Consecutive_Wins() { }

        [Fact]
        // Test du bonus mais avec 4 victoires
        public void Should_Add_Bonus_For_Four_Consecutive_Wins() { }

        [Fact]
        // Test de pas mettre de bonus si interruption de série
        public void Should_Not_Add_Bonus_When_Series_Broken() { }

        [Fact]
        // Test pour des bonus multiples 
        public void Should_Add_Multiple_Bonuses_For_Consecutive_Wins() { }

        [Fact]
        // Test pour un bonus non accordé si la série est interrompue
        public void Should_Not_Add_Bonus_When_Series_Interrupted() { }

        #endregion

        #region Test de disqualification

        [Fact]
        // Test de disqualification avec un score positif
        public void Should_Return_Zero_When_Disqualified_With_Positive_Score() { }

        [Fact]
        // Test de disqualification sans combat 
        public void Should_Return_Zero_When_Disqualified_Without_Fight() { }

        #endregion

        #region Test des pénalités 

        [Fact]
        // Test des pénalités normales
        public void Should_Apply_Normal_Penalty() { }

        [Fact]
        // Test des pénalités supérieures
        public void Should_Apply_Higher_Penalty() { }

        [Fact]
        // Test des pénalités égales
        public void Should_Apply_Equal_Penalty() { }

        #endregion

        #region Tests des cas limites
         
        [Fact]
        // Test pour un score final de zéro quand disqualified
        public void Should_Return_Zero_When_Disqualified() { }

        [Fact]
        // Test pour un score non-négatif final
        public void Should_Not_Allow_Negative_Final_Score() { }

        [Fact]
        // Test pour une liste vide de combats
        public void Should_Return_Zero_When_No_Fights() { }

        [Fact]
        // Test pour une liste null de combats
        public void Should_Return_Zero_When_Fights_Is_Null() { }

        [Fact]
        // Test pour une liste vide de résultats
        public void Should_Return_Zero_When_Results_Are_Empty() { }

        [Fact] 
        // Test avec des pénalités négatives 
        public void Should_Not_Allow_Negative_Penalties() { }

        [Fact]
        // Test pour un très long tournoi (>100 combats)    
        public void Should_Handle_Long_Tournament_Without_Error() { }
        
        #endregion
    }
}
