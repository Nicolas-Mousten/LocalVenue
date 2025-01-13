using AutoMapper;
using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;
using LocalVenue.Helpers;
using LocalVenue.Services;
using LocalVenue.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Show = LocalVenue.Web.Models.Show;

namespace LocalVenue.Tests;

public class ShowServiceTest
{
    private readonly ServiceProvider serviceProvider;

    public ShowServiceTest()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<VenueContext>(options =>
        {
            options
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });

        serviceProvider = services.BuildServiceProvider();
    }

    [Theory]
    [InlineData(1, 10, "TestTitleTwo", "Title", 1, null)] //find one object with title "TestTitleTwo"
    [InlineData(1, 10, "TestTitleTwo", null, 1, null)] //find one object with title "TestTitleTwo"
    [InlineData(1, 10, "TestDescriptionTwo", "Description", 1, null)] //find one object with description "TestDescriptionTwo"
    [InlineData(1, 2, null, null, 2, 2)] //expect two items on page #1 with pagesize 2 with a next page of 2
    [InlineData(2, 2, null, null, 1, null)] //expect one item on page #2 with pagesize 2 with no next page
    public async Task TestGetPagedListShow(
        int pageNumber,
        int pageSize,
        string searchParameter,
        string? searchProperty,
        int expectedCount,
        int? expectedNext
    )
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactory, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);

        var service = new ShowService(contextFactory, mapper, actorService, ticketService);

        var show1 = new Show
        {
            Id = 0,
            Title = "TestTitle",
            Description = "TestDescription",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };
        var show2 = new Show
        {
            Id = 0,
            Title = "TestTitleTwo",
            Description = "TestDescriptionTwo",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };
        var show3 = new Show
        {
            Id = 0,
            Title = "TestTitleThree",
            Description = "TestDescriptionThree",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        await service.CreateShowAsync(show1);
        await service.CreateShowAsync(show2);
        await service.CreateShowAsync(show3);

        // Act
        var pagedShows = await service.GetShows(
            pageNumber,
            pageSize,
            searchParameter,
            searchProperty
        );

        // Assert
        Assert.NotNull(pagedShows);
        Assert.Equal(expectedCount, pagedShows.Count);
        Assert.Equal(expectedNext, pagedShows.Next);
        Assert.IsType<List<Core.Entities.Show>>(pagedShows.Results);
        Assert.True(pagedShows.Results.Count <= pageSize);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TestDeleteShow()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactory, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);

        var service = new ShowService(contextFactory, mapper, actorService, ticketService);

        var show = new Show
        {
            Id = 1,
            Title = "TestTitle",
            Description = "TestDescription",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        await service.CreateShowAsync(show);

        var dataBaseShow = await service.GetShow(1);

        // Act
        await service.DeleteShow(1);

        // Assert
        Assert.NotNull(dataBaseShow);
        Assert.Equal(1, dataBaseShow.ShowId);
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetShow(1));

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TestUpdateShow()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactory, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);

        var service = new ShowService(contextFactory, mapper, actorService, ticketService);

        var show = new Show
        {
            Id = 1,
            Title = "TestTitle",
            Description = "TestDescription",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        await service.CreateShowAsync(show);

        var updatedShow = new Show
        {
            Id = 1,
            Title = "UpdatedTitle",
            Description = "UpdatedDescription",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(3),
            Genre = Genre.Comedy,
            OpeningNight = true,
        };

        // Act
        await service.UpdateShowAsync(updatedShow);

        var dataBaseShow = await service.GetShow(1);

        // Assert
        Assert.NotNull(dataBaseShow);
        Assert.Equal(updatedShow.Id, dataBaseShow.ShowId);
        Assert.Equal(updatedShow.Title, dataBaseShow.Title);
        Assert.Equal(updatedShow.Description, dataBaseShow.Description);
        Assert.Equal(updatedShow.StartTime, dataBaseShow.StartTime);
        Assert.Equal(updatedShow.EndTime, dataBaseShow.EndTime);
        Assert.Equal(updatedShow.Genre, dataBaseShow.Genre);
        Assert.Equal(updatedShow.OpeningNight, dataBaseShow.OpeningNight);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TestGetAvailableTicketsForShow()
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactory, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);

        var service = new ShowService(contextFactory, mapper, actorService, ticketService);

        await using (var context = await contextFactory.CreateDbContextAsync())
        {
            // Create seats for the database
            context.Seats.Add(
                new Seat
                {
                    SeatId = 2,
                    Section = "Front",
                    Row = 1,
                    Number = 1,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 3,
                    Section = "Front",
                    Row = 1,
                    Number = 2,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 4,
                    Section = "Front",
                    Row = 1,
                    Number = 3,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 5,
                    Section = "Front",
                    Row = 1,
                    Number = 4,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 6,
                    Section = "Front",
                    Row = 1,
                    Number = 5,
                }
            );
            await context.SaveChangesAsync();
        }

        var show = new Show
        {
            Id = 1,
            Title = "TestTitle",
            Description = "TestDescription",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        await service.CreateShowAsync(show);

        // Act
        var tickets = await service.GetAvailableTicketsForShow(1);

        // Assert
        Assert.NotNull(tickets);
        Assert.True(tickets.Count == 5);

        await serviceProvider.DisposeAsync();
    }

    [Theory]
    [InlineData("TestTitleTwo", "Title")]
    [InlineData("TestTitleTwo", null)]
    [InlineData("TestDescriptionTwo", "Description")]
    public async Task TestGetShowSearch(string searchParameter, string? searchProperty)
    {
        // Arrange
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactory, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);

        var service = new ShowService(contextFactory, mapper, actorService, ticketService);

        var show1 = new Show
        {
            Id = 0,
            Title = "TestTitle",
            Description = "TestDescription",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };
        var show2 = new Show
        {
            Id = 0,
            Title = "TestTitleTwo",
            Description = "TestDescriptionTwo",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        await service.CreateShowAsync(show1);
        await service.CreateShowAsync(show2);

        // Act
        var dataBaseShow = new Core.Entities.Show();
        if (searchProperty == null)
        {
            dataBaseShow = await service.GetShow(searchParameter);
        }
        else
        {
            dataBaseShow = await service.GetShow(searchParameter, searchProperty!);
        }

        // Assert
        Assert.NotNull(dataBaseShow);
        Assert.Equal(2, dataBaseShow.ShowId);
        Assert.Equal(show2.Title, dataBaseShow.Title);
        Assert.Equal(show2.Description, dataBaseShow.Description);
        Assert.True(new TimeSpan(0, 0, 10) > dataBaseShow.StartTime - show2.StartTime); // Test if the time is within 10 seconds
        Assert.True(new TimeSpan(0, 0, 10) > dataBaseShow.EndTime - show2.EndTime); // Test if the time is within 10 seconds
        // Assert.Equal(show.StartTime, dataBaseShow.StartTime);
        // Assert.Equal(show.EndTime, dataBaseShow.EndTime);
        Assert.Equal(show2.Genre, dataBaseShow.Genre);
        Assert.Equal(show2.OpeningNight, dataBaseShow.OpeningNight);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TestShowServiceCreateShowWithTickets()
    {
        // Create seats for the database
        var dbContextFactory = serviceProvider.GetRequiredService<
            IDbContextFactory<VenueContext>
        >();

        int seatCount;

        await using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            context.Seats.Add(
                new Seat
                {
                    SeatId = 2,
                    Section = "Front",
                    Row = 1,
                    Number = 1,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 3,
                    Section = "Front",
                    Row = 1,
                    Number = 2,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 4,
                    Section = "Front",
                    Row = 1,
                    Number = 3,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 5,
                    Section = "Front",
                    Row = 1,
                    Number = 4,
                }
            );
            context.Seats.Add(
                new Seat
                {
                    SeatId = 6,
                    Section = "Front",
                    Row = 1,
                    Number = 5,
                }
            );
            await context.SaveChangesAsync();
            seatCount = context.Seats.Count();
        }

        // Act
        var contextFactoryRetrieve = serviceProvider.GetRequiredService<
            IDbContextFactory<VenueContext>
        >();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactoryRetrieve, mapper);
        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);

        var service = new ShowService(contextFactoryRetrieve, mapper, actorService, ticketService);

        var show = new Show
        {
            Id = 0, //EF should auto increment starting at 1
            Title = "Test show",
            Description = "Test description",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Genre = Genre.Musical,
            OpeningNight = false,
        };

        // Assert item does not exist before creating
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetShow(1));

        await service.CreateShowAsync(show);

        var dataBaseShow = await service.GetShow(1);

        // Assert
        Assert.NotNull(dataBaseShow);
        Assert.Equal(1, dataBaseShow.ShowId);
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
