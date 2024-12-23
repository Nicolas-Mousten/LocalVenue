using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LocalVenue.Tests;

public class SeatServiceTest
{
    private readonly ServiceProvider serviceProvider;

    public SeatServiceTest()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<VenueContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDb")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });

        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task TestSeatServiceGetById()
    {
        // Arrange
        var seatId = 1;
        var expectedSeat = new Seat { SeatId = seatId, Section = "Front", Row = 1, Number = 1 };

        var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();
        
        await using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            context.Seats.Add(expectedSeat);
            await context.SaveChangesAsync();
        }

        // Act
        var contextFactoryRetrieve = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();
    
        var service = new SeatService(contextFactoryRetrieve);
        var result = await service.GetSeat(seatId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSeat.SeatId, result.SeatId);
        Assert.Equal(expectedSeat.Section, result.Section);
        Assert.Equal(expectedSeat.Row, result.Row);
        Assert.Equal(expectedSeat.Number, result.Number);
        
    }
}