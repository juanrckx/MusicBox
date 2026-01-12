using MusicBox.Models;
using Xunit;

namespace MusicBox.Tests
{
    public class NoteTests
    {
        [Theory]
        [InlineData(NoteName.Do, 261.63)]
        [InlineData(NoteName.Re, 293.66)]
        [InlineData(NoteName.Mi, 329.63)]
        [InlineData(NoteName.Fa, 349.23)]
        [InlineData(NoteName.Sol, 392.00)]
        [InlineData(NoteName.La, 440.00)]
        [InlineData(NoteName.Si, 493.88)]
        public void GetFrequency_ForEachNote_ReturnsCorrectFrequency(
            NoteName noteName, double expectedFrequency)
        {
            // Act
            var frequency = Note.GetFrequency(noteName);

            // Assert
            Assert.Equal(expectedFrequency, frequency, 2); // Precisión de 2 decimales
        }

        [Fact]
        public void Note_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var duration = new Duration
            {
                Figure = FigureType.Negra,
                BaseSeconds = 1.0
            };

            // Act
            var note = new Note
            {
                Name = NoteName.Do,
                Frequency = 261.63,
                Duration = duration
            };

            // Assert
            Assert.Equal(NoteName.Do, note.Name);
            Assert.Equal(261.63, note.Frequency);
            Assert.Equal(duration, note.Duration);
            Assert.Equal(FigureType.Negra, note.Duration.Figure);
            Assert.Equal(1.0, note.Duration.BaseSeconds);
        }

        [Fact]
        public void Note_DurationGetSeconds_WorksFromNoteInstance()
        {
            // Arrange
            var note = new Note
            {
                Name = NoteName.Do,
                Frequency = 261.63,
                Duration = new Duration
                {
                    Figure = FigureType.Blanca,
                    BaseSeconds = 0.5
                }
            };

            // Act
            var seconds = note.Duration.GetSeconds();

            // Assert
            // Blanca = 2 * BaseSeconds = 2 * 0.5 = 1.0
            Assert.Equal(1.0, seconds);
        }
    }
}
