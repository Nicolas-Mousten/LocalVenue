using LocalVenue.Helpers;
using Show = LocalVenue.Web.Models.Show;

namespace LocalVenue.Tests;

public class ShowPriceCalculatorTest
{
    [Theory]
    [InlineData("2023-10-01T18:00:00", "2023-10-01T20:30:00", true)] // More than 2 hours
    [InlineData("2023-10-01T18:00:00", "2023-10-01T19:30:00", false)] // Less than 2 hours
    [InlineData("2023-10-01T18:00:00", "2023-10-01T20:00:00", false)] // Exactly 2 hours
    [InlineData("2023-10-01T18:00:00", "2023-10-01T18:00:00", false)] // Zero duration
    [InlineData("2023-10-01T18:00:00", "2023-10-02T18:00:00", true)] // 24 hours
    public void ShowLastForMoreThanTwoHours_ShouldReturnExpectedResult(string startTime, string endTime,
        bool expectedResult)
    {
        // Arrange
        var show = new Show
        {
            StartTime = DateTime.Parse(startTime),
            EndTime = DateTime.Parse(endTime)
        };

        // Act
        var result = ShowPriceCalculator.ShowLastForMoreThanTwoHours(show);

        // Assert
        Assert.Equal(expectedResult, result);
    }
    
}