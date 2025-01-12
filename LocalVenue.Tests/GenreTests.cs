using LocalVenue.Core.Enums;
using System;
using Xunit;

namespace LocalVenue.Tests
{
    public class GenreTest
    {
        [Theory]
        [InlineData(Genre.Musical, "Musical")]
        [InlineData(Genre.Comedy, "Komedie")]
        [InlineData(Genre.Drama, "Drama")]
        [InlineData(Genre.Horror, "Gyser")]
        [InlineData(Genre.Romance, "Romantik")]
        [InlineData(Genre.Documentary, "Dokumentar")]
        public void ToNewString_ValidGenre_ReturnsExpectedString(Genre genre, string expected)
        {
            // Act
            var result = genre.ToNewString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToNewString_InvalidGenre_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var invalidGenre = (Genre)999;

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => invalidGenre.ToNewString());
            Assert.Equal("Genre", exception.ParamName);
        }
    }
}