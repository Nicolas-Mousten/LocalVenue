using LocalVenue.Helpers;
using LocalVenue.Web.Models;

namespace LocalVenue.Tests
{
    public class DiscountCalculatorTest
    {
        [Fact]
        public void ApplyDiscount_NoTickets_ReturnsEmptyList()
        {
            // Arrange
            var tickets = new List<Ticket>();

            // Act
            var result = tickets.ApplyDiscount(0);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplyDiscount_FewerThanFourTickets_AppliesDiscountToAll()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new() { Price = 100m, Seat = new Seat() },
                new() { Price = 200m, Seat = new Seat() },
            };

            // Act
            var result = tickets.ApplyDiscount(0);

            // Assert
            Assert.Equal(80m, result[0].Price);
            Assert.Equal(160m, result[1].Price);
        }

        [Fact]
        public void ApplyDiscount_ExactlyFourTickets_AppliesDiscountToAll()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new Ticket { Price = 100m, Seat = new Seat() },
                new Ticket { Price = 200m, Seat = new Seat() },
                new Ticket { Price = 300m, Seat = new Seat() },
                new Ticket { Price = 400m, Seat = new Seat() },
            };

            // Act
            var result = tickets.ApplyDiscount(0);

            // assert
            Assert.Equal(80m, result[0].Price);
            Assert.Equal(160m, result[1].Price);
            Assert.Equal(240m, result[2].Price);
            Assert.Equal(320m, result[3].Price);
        }

        [Fact]
        public void ApplyDiscount_MoreThanFourTickets_AppliesDiscountToTopFour()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new() { Price = 100m, Seat = new Seat() },
                new() { Price = 200m, Seat = new Seat() },
                new() { Price = 300m, Seat = new Seat() },
                new() { Price = 400m, Seat = new Seat() },
                new() { Price = 500m, Seat = new Seat() },
            };

            // Act
            var result = tickets.ApplyDiscount(0);

            // Assert
            Assert.Equal(80m, result[0].Price);
            Assert.Equal(160m, result[1].Price);
            Assert.Equal(240m, result[2].Price);
            Assert.Equal(320m, result[3].Price);
            Assert.Equal(500m, result[4].Price); // No discount applied to the fifth ticket
        }
    }
}
