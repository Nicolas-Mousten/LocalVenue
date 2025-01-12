using AutoMapper;
using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;
using LocalVenue.Core.Services;
using LocalVenue.Helpers;
using LocalVenue.Services;
using LocalVenue.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Show = LocalVenue.Web.Models.Show;


public class ShowServiceTest
{
    private readonly ServiceProvider serviceProvider;

    public ShowServiceTest()
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
    public async Task TestShowServiceCreateShowWithTickets()
    {
        // create a lot of seats for the database
        
        var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        int seatCount;
        
        await using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            context.Seats.Add(new Seat {SeatId = 2, Section = "Front", Row = 1, Number = 1 });
            context.Seats.Add(new Seat {SeatId = 3, Section = "Front", Row = 1, Number = 2 });
            context.Seats.Add(new Seat {SeatId = 4, Section = "Front", Row = 1, Number = 3 });
            context.Seats.Add(new Seat {SeatId = 5, Section = "Front", Row = 1, Number = 4 });
            context.Seats.Add(new Seat {SeatId = 6, Section = "Front", Row = 1, Number = 5 });
            await context.SaveChangesAsync();
            seatCount = context.Seats.Count();
        }
        
        
        // Act
        var contextFactoryRetrieve = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();  // Assuming ShowProfile contains your mappings
        });
        var mapper = config.CreateMapper();
        
        var ticketService = new TicketService(contextFactoryRetrieve, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);
        
        var service = new ShowService(contextFactoryRetrieve, mapper, actorService, ticketService);

        var show = new Show
        {
            Id = 111,
            Title = "Test show",
            Description = "Test description",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        await service.CreateShowAsync(show);

        var dataBaseShow = await service.GetShow(111);
        
        // Assert
        Assert.NotNull(dataBaseShow);
        Assert.Equal(show.Id, dataBaseShow.ShowId);
        Assert.Equal(show.Title, dataBaseShow.Title);
        Assert.Equal(show.Description, dataBaseShow.Description);
        Assert.Equal(show.StartTime, dataBaseShow.StartTime);
        Assert.Equal(show.EndTime, dataBaseShow.EndTime);
        Assert.Equal(show.Genre, dataBaseShow.Genre);
        Assert.Equal(show.OpeningNight, dataBaseShow.OpeningNight);
        Assert.NotNull(dataBaseShow.Tickets);
        Assert.Equal(seatCount, dataBaseShow.Tickets.Count);
        
        await serviceProvider.DisposeAsync();

    }
}