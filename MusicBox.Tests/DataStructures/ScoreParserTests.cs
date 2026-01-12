using System;
using System.Linq;
using MusicBox.DataStructures;
using MusicBox.Models;
using Xunit;

namespace MusicBox.Tests
{
    public class ScoreParserTests
    {
        [Fact]
        public void Parse_ValidScore_ReturnsCorrectNumberOfNotes()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do, negra), (Re, blanca), (Mi, corchea)";

            // Act
            var result = parser.Parse(score);

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Parse_ValidScore_CorrectNoteNames()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do, negra), (Re, blanca), (Mi, corchea)";

            // Act
            var result = parser.Parse(score);
            var notes = result.TraverseForward().ToList();

            // Assert
            Assert.Equal(NoteName.Do, notes[0].Name);
            Assert.Equal(NoteName.Re, notes[1].Name);
            Assert.Equal(NoteName.Mi, notes[2].Name);
        }

        [Fact]
        public void Parse_ValidScore_CorrectFigureTypes()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do, negra), (Re, blanca), (Mi, corchea)";

            // Act
            var result = parser.Parse(score);
            var notes = result.TraverseForward().ToList();

            // Assert
            Assert.Equal(FigureType.Negra, notes[0].Duration.Figure);
            Assert.Equal(FigureType.Blanca, notes[1].Duration.Figure);
            Assert.Equal(FigureType.Corchea, notes[2].Duration.Figure);
        }

        [Fact]
        public void Parse_InvalidNoteName_ThrowsArgumentException()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do, negra), (Xi, blanca)"; // "Xi" no es una nota válida

            // Act & Assert
            Assert.Throws<ArgumentException>(() => parser.Parse(score));
        }

        [Fact]
        public void Parse_InvalidFigureType_ThrowsArgumentException()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do, negra), (Re, invalida)"; // "invalida" no es una figura válida

            // Act & Assert
            Assert.Throws<ArgumentException>(() => parser.Parse(score));
        }

        [Fact]
        public void Parse_ScoreWithDifferentBaseDuration_CorrectDurations()
        {
            // Arrange
            double baseDuration = 2.0; // 2 segundos por negra
            var parser = new ScoreParser(baseDuration);
            string score = "(Do, negra), (Re, blanca), (Mi, corchea)";

            // Act
            var result = parser.Parse(score);
            var notes = result.TraverseForward().ToList();

            // Assert
            // Negra: 1 * 2.0 = 2.0s
            // Blanca: 2 * 2.0 = 4.0s
            // Corchea: 0.5 * 2.0 = 1.0s
            Assert.Equal(2.0, notes[0].Duration.GetSeconds());
            Assert.Equal(4.0, notes[1].Duration.GetSeconds());
            Assert.Equal(1.0, notes[2].Duration.GetSeconds());
        }

        [Fact]
        public void Parse_EmptyString_ThrowsArgumentException()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "";

            // Act & Assert
            // Según la implementación actual, un string vacío daría error de formato
            // Verificamos que se lanza la excepción esperada
            Assert.Throws<FormatException>(() => parser.Parse(score));
        }

        [Fact]
        public void Parse_MalformedFormat_ThrowsFormatException()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do negra), (Re, blanca)"; // Falta la coma después de Do

            // Act & Assert
            Assert.Throws<FormatException>(() => parser.Parse(score));
        }

        [Fact]
        public void Parse_ScoreWithSpacesAndCaseVariations_WorksCorrectly()
        {
            // Arrange
            var parser = new ScoreParser();
            string score = "(Do, NEGRA), ( re , blanca ), ( Mi , CORCHEA )";

            // Act
            var result = parser.Parse(score);

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Constructor_BaseDurationOutOfRange_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new ScoreParser(0.05)); // Menor que 0.1
            Assert.Throws<ArgumentException>(() => new ScoreParser(6.0)); // Mayor que 5.0
        }
    }
}
