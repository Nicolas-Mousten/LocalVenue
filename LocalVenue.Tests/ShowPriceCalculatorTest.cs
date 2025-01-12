using LocalVenue.Helpers;
using LocalVenue.Web.Models;
using Seat = LocalVenue.Web.Models.Seat;
using Show = LocalVenue.Web.Models.Show;

namespace LocalVenue.Tests;

public class ShowPriceCalculatorTest
{

    [Theory]
    [InlineData("2023-10-01T18:00:00", "2023-10-01T20:30:00", true)] // More than 2 hours
    [InlineData("2023-10-01T18:00:00", "2023-10-01T19:30:00", false)] // Less than 2 hours
    [InlineData("2023-10-01T18:00:00", "2023-10-01T20:00:00", false)] // Exactly 2 hours
    [InlineData("2023-10-01T18:00:00", "2023-10-01T18:00:00", false)] // Zero duration
    [InlineData("2023-10-01T18:00:00", "2023-10-02T18:00:01", true)] // 1d 00h 00m 01s    
    [InlineData("2023-10-01T18:00:00", "2023-10-01T20:00:01", true)] // 2h 0m 1s
    [InlineData("2023-10-01T18:00:00", "2023-10-01T19:59:59", false)] // 1h 59m 59s
    [InlineData("2023-10-01T18:00:00", "2023-10-01T17:59:59", false)] // -1 second
    [InlineData("2023-10-01T18:00:00", "2023-10-01T18:00:01", false)] // 1 second
    [InlineData("0001-01-01T00:00:00", "9999-12-31T23:59:59", true)] // Timespan max

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

    [Theory]
    [InlineData(false, 17, false, 3, 120)]
    [InlineData(false, 17, false, 2, 150)]
    [InlineData(false, 17, true, 3, 138)]
    [InlineData(false, 17, true, 2, 168)]
    [InlineData(false, 18, false, 3, 140)]
    [InlineData(false, 18, false, 2, 170)]
    [InlineData(false, 18, true, 3, 158)]
    [InlineData(false, 18, true, 2, 188)]
    [InlineData(true, 17, false, 3, 144)]
    [InlineData(true, 17, false, 2, 174)]
    [InlineData(true, 17, true, 3, 162)]
    [InlineData(true, 17, true, 2, 192)]
    [InlineData(true, 18, false, 3, 164)]
    [InlineData(true, 18, false, 2, 194)]
    [InlineData(true, 18, true, 3, 182)]
    [InlineData(true, 18, true, 2, 212)]
    public void CalculatePrice_ShouldReturnExpectedResult(bool openingNight, int startHour, bool moreThanTwoHours,
        int row, decimal expectedPrice)
    {
        // Arrange
        var show = new Show
        {
            StartTime = new DateTime(2023, 10, 01, startHour, 00, 00),
            EndTime = moreThanTwoHours
                ? new DateTime(2023, 10, 01, startHour + 3, 00, 00)
                : new DateTime(2023, 10, 01, startHour + 1, 30, 00),
            OpeningNight = openingNight
        };
        var ticket = new Ticket
        {
            Seat = new Seat { Row = row }
        };

        // Act
        var result = ShowPriceCalculator.CalculatePrice(show, ticket, openingNight);

        // Assert
        Assert.Equal(expectedPrice, result);
    }

    [Fact]
    public void CalculatePrice_InvalidSeat_ThrowsArgumentException()
    {
        // Arrange
        var show = new Show
        {
            StartTime = new DateTime(2023, 10, 01, 18, 00, 00),
            EndTime = new DateTime(2023, 10, 01, 20, 30, 00),
            OpeningNight = true
        };
        var ticket = new Ticket
        {
            Seat = null!
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ShowPriceCalculator.CalculatePrice(show, ticket, true));
    }

    [Fact]
    public void AddPrice_ShouldCalculatePriceForAllTickets()
    {
        // Arrange
        var shows = new List<Show>
        {
            new Show
            {
                StartTime = new DateTime(2023, 10, 01, 18, 00, 00),
                EndTime = new DateTime(2023, 10, 01, 20, 30, 00),
                OpeningNight = true,
                Tickets =
                [
                    new() { Seat = new Seat { Row = 1 } },
                    new() { Seat = new Seat { Row = 3 } }
                ]
            }
        };

        // Act
        var result = shows.AddPrice();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result[0].Tickets?.Count);
        Assert.Equal(212, result[0].Tickets?[0].Price); // Calculated price for row 1
        Assert.Equal(182, result[0].Tickets?[1].Price); // Calculated price for row 3
    }

    [Fact]
    public void AddPrice_ShouldHandleShowsWithoutTickets()
    {
        // Arrange
        var shows = new List<Show>
        {
            new Show
            {
                StartTime = new DateTime(2023, 10, 01, 18, 00, 00),
                EndTime = new DateTime(2023, 10, 01, 20, 30, 00),
                OpeningNight = true,
                Tickets = null
            }
        };

        // Act
        var result = shows.AddPrice();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result[0].Tickets);
    }

    [Fact]
    public void AddPrice_ShouldHandleEmptyListOfShows()
    {
        // Arrange
        var shows = new List<Show>();

        // Act
        var result = shows.AddPrice();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void AddPrice_ShouldHandleShowWithEmptyTickets()
    {
        // Arrange
        var shows = new List<Show>
        {
            new Show
            {
                StartTime = new DateTime(2023, 10, 01, 18, 00, 00),
                EndTime = new DateTime(2023, 10, 01, 20, 30, 00),
                OpeningNight = true,
                Tickets = []
            }
        };

        // Act
        var result = shows.AddPrice();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result[0].Tickets!);
    }
}