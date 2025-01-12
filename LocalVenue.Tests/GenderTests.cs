using System;
using LocalVenue.Core.Enums;
using Xunit;

namespace LocalVenue.Tests
{
    public class GenderTest
    {
        [Theory]
        [InlineData(Gender.NotSpecified, "Unknown")]
        [InlineData(Gender.Female, "Female")]
        [InlineData(Gender.Male, "Male")]
        [InlineData(Gender.NonBinary, "Non-binary")]
        public void ToNewString_ValidGender_ReturnsExpectedString(Gender gender, string expected)
        {
            // Act
            var result = gender.ToNewString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToNewString_InvalidGender_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var invalidGender = (Gender)999;

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => invalidGender.ToNewString()
            );
            Assert.Equal("gender", exception.ParamName);
        }
    }
}
