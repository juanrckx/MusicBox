using MusicBox.DataStructures;
using MusicBox.Models;
using Xunit;

namespace MusicBox.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void FullWorkflow_FromStringToPlayback_WorksCorrectly()
        {
            // Arrange
            var parser = new ScoreParser(1.0);
            string scoreString = "(Do, negra), (Re, blanca), (Mi, corchea)";

            // Act
            var score = parser.Parse(scoreString);

            // Assert - Verificar que todo se procesó correctamente
            Assert.Equal(3, score.Count);
            
            var notes = score.TraverseForward().ToList();
            
            // Verificar notas
            Assert.Equal(NoteName.Do, notes[0].Name);
            Assert.Equal(NoteName.Re, notes[1].Name);
            Assert.Equal(NoteName.Mi, notes[2].Name);
            
            // Verificar figuras
            Assert.Equal(FigureType.Negra, notes[0].Duration.Figure);
            Assert.Equal(FigureType.Blanca, notes[1].Duration.Figure);
            Assert.Equal(FigureType.Corchea, notes[2].Duration.Figure);
            
            // Verificar duraciones
            Assert.Equal(1.0, notes[0].Duration.GetSeconds());
            Assert.Equal(2.0, notes[1].Duration.GetSeconds());
            Assert.Equal(0.5, notes[2].Duration.GetSeconds());
            
            // Verificar frecuencias
            Assert.Equal(261.63, notes[0].Frequency, 2);
            Assert.Equal(293.66, notes[1].Frequency, 2);
            Assert.Equal(329.63, notes[2].Frequency, 2);
        }

        [Fact]
        public void ReversePlayback_ProducesNotesInReverseOrder()
        {
            // Arrange
            var parser = new ScoreParser(1.0);
            string scoreString = "(Do, negra), (Re, blanca), (Mi, corchea)";
            var score = parser.Parse(scoreString);

            // Act
            var forwardNotes = score.TraverseForward().ToList();
            var backwardNotes = score.TraverseBackward().ToList();

            // Assert
            Assert.Equal(3, forwardNotes.Count);
            Assert.Equal(3, backwardNotes.Count);
            
            // Verificar orden inverso
            Assert.Equal(forwardNotes[0].Name, backwardNotes[2].Name);
            Assert.Equal(forwardNotes[1].Name, backwardNotes[1].Name);
            Assert.Equal(forwardNotes[2].Name, backwardNotes[0].Name);
        }
    }
}
