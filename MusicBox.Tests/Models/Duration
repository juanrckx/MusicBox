using MusicBox.Models;
using Xunit;

namespace MusicBox.Tests
{
    public class DurationTests
    {
        [Theory]
        [InlineData(FigureType.Redonda, 1.0, 4.0)]    // 4 * 1.0 = 4.0
        [InlineData(FigureType.Blanca, 1.0, 2.0)]     // 2 * 1.0 = 2.0
        [InlineData(FigureType.Negra, 1.0, 1.0)]      // 1 * 1.0 = 1.0
        [InlineData(FigureType.Corchea, 1.0, 0.5)]    // 0.5 * 1.0 = 0.5
        [InlineData(FigureType.Semicorchea, 1.0, 0.25)] // 0.25 * 1.0 = 0.25
        public void GetSeconds_ForEachFigure_ReturnsCorrectValue(
            FigureType figure, double baseSeconds, double expected)
        {
            // Arrange
            var duration = new Duration
            {
                Figure = figure,
                BaseSeconds = baseSeconds
            };

            // Act
            var result = duration.GetSeconds();

            // Assert
            Assert.Equal(expected, result, 2); // Precisión de 2 decimales
        }

        [Fact]
        public void GetSeconds_WithCustomBaseDuration_CalculatesCorrectly()
        {
            // Arrange
            var duration = new Duration
            {
                Figure = FigureType.Negra,
                BaseSeconds = 0.5 // Medio segundo por negra
            };

            // Act
            var result = duration.GetSeconds();

            // Assert
            Assert.Equal(0.5, result);
        }
    }
}