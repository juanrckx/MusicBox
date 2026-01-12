using MusicBox.Audio;
using MusicBox.DataStructures;
using MusicBox.Models;
using Xunit;

namespace MusicBox.Tests
{
    public class AudioEngineTests
    {
        [Fact]
        public void PlayScore_WithNullScore_DoesNotThrow()
        {
            // Arrange
            using var audioEngine = new AudioEngine();

            // Act & Assert
            var exception = Record.Exception(() => audioEngine.PlayScore(null));
            Assert.Null(exception);
        }

        [Fact]
        public void PlayScore_WithEmptyList_DoesNotThrow()
        {
            // Arrange
            using var audioEngine = new AudioEngine();
            var score = new DoublyLinkedList<Note>();

            // Act & Assert
            var exception = Record.Exception(() => audioEngine.PlayScore(score));
            Assert.Null(exception);
        }

        [Fact]
        public void SetBaseDuration_ValidValue_UpdatesSuccessfully()
        {
            // Arrange
            using var audioEngine = new AudioEngine();
            double newDuration = 0.5;

            // Act
            audioEngine.SetBaseDuration(newDuration);

            // Assert
            // No hay forma directa de verificar el cambio, pero si no lanza excepción es exitoso
            var exception = Record.Exception(() => audioEngine.SetBaseDuration(newDuration));
            Assert.Null(exception);
        }

        [Fact]
        public void PlayScore_ForwardAndBackward_CanBeCalled()
        {
            // Arrange
            using var audioEngine = new AudioEngine();
            var score = new DoublyLinkedList<Note>();
            
            // Crear una nota simple
            score.AddLast(new Note
            {
                Name = NoteName.Do,
                Frequency = 261.63,
                Duration = new Duration
                {
                    Figure = FigureType.Negra,
                    BaseSeconds = 1.0
                }
            });

            // Act & Assert - Verificar que se pueden llamar ambos métodos sin excepción
            var exception1 = Record.Exception(() => audioEngine.PlayScore(score, forward: true));
            var exception2 = Record.Exception(() => audioEngine.PlayScore(score, forward: false));
            
            Assert.Null(exception1);
            Assert.Null(exception2);
        }
    }
}