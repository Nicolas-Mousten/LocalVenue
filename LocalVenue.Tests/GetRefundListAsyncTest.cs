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

namespace LocalVenue.Tests;

public class GetRefundListAsyncTest
{
    private readonly ServiceProvider serviceProvider;

    public GetRefundListAsyncTest()
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

    [Fact]
    public async Task TestTicketServiceBuySeat()
    {
        // Arrange
        var dbContextFactory = serviceProvider.GetRequiredService<
            IDbContextFactory<VenueContext>
        >();
        Customer customer = new Customer
        {
            Id = "0c9cd65f-2054-4086-a569-2e50997a8be9",
            UserName = "Nicolas",
            NormalizedUserName = "NICOLAS",
            Email = "nicolas.mousten@gmail.com",
            NormalizedEmail = "NICOLAS.MOUSTEN@GMAIL.COM",
            EmailConfirmed = false,
            PasswordHash =
                "AQAAAAIAAYagAAAAEJWgg4FDKFWNh/AYIzVE/3nxRluYnwDmUfDnpc75ZUylWzJYkphFBrhqFkRAgm16YA==",
            SecurityStamp = "S4KC54SOPWVKCI7KA6MGFCDMBS5SVWXG",
            ConcurrencyStamp = "9ceff3cb-b5c8-4562-bcef-4a4f0d3c3761",
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            FirstName = "Nicolas",
            LastName = "Mousten",
            IsGoldenMember = false,
            Tickets = new List<Ticket>(),
        };
        Show show = new Show
        {
            ShowId = 1,
            Title = "TestShow",
            Description = "A show to test on",
            StartTime = DateTime.Now.AddHours(32),
            EndTime = DateTime.Now.AddHours(34),
            Genre = Genre.Musical,
            OpeningNight = false,
            Tickets = new List<Ticket>(),
        };
        Ticket ticket = new Ticket
        {
            TicketId = 1,
            ShowId = show.ShowId,
            Show = show,
            SeatId = 1,
            Seat = new Seat
            {
                SeatId = 1,
                Section = "Front",
                Row = 1,
                Number = 1,
            },
            Price = 50,
            Status = Status.Available,
            CustomerId = "0c9cd65f-2054-4086-a569-2e50997a8be9",
            Customer = customer,
        };
        Ticket ticket2 = new Ticket
        {
            TicketId = 2,
            ShowId = show.ShowId,
            Show = show,
            SeatId = 2,
            Seat = new Seat
            {
                SeatId = 2,
                Section = "Front",
                Row = 1,
                Number = 2,
            },
            Price = 50,
            Status = Status.Available,
            CustomerId = "0c9cd65f-2054-4086-a569-2e50997a8be9",
            Customer = customer,
        };
        Ticket ticket3 = new Ticket
        {
            TicketId = 3,
            ShowId = show.ShowId,
            Show = show,
            SeatId = 3,
            Seat = new Seat
            {
                SeatId = 3,
                Section = "Front",
                Row = 1,
                Number = 3,
            },
            Price = 50,
            Status = Status.Available,
            CustomerId = null,
            Customer = null,
        };

        show.Tickets.Add(ticket);
        show.Tickets.Add(ticket2);
        show.Tickets.Add(ticket3);
        await using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            context.Users.Add(customer);
            context.Shows.Add(show);
            await context.SaveChangesAsync();
        }
        // Act
        var contextFactoryRetrieve = serviceProvider.GetRequiredService<
            IDbContextFactory<VenueContext>
        >();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>(); // Assuming ShowProfile contains your mappings
        });
        var mapper = config.CreateMapper();

        var ticketService = new TicketService(contextFactoryRetrieve, mapper);
        var seatService = new SeatService(contextFactoryRetrieve);

        var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();
        var actorService = new ActorService(mockFactory.Object);
        var showService = new ShowService(
            contextFactoryRetrieve,
            mapper,
            actorService,
            ticketService
        );

        var refundList = await showService.GetRefundListAsync(1);

        // Assert
        Assert.NotNull(refundList);
        Assert.Equal(2, refundList[0].TicketCount);
        Assert.Equal(customer.FirstName, refundList[0].CustomerName);
        Assert.Equal(customer.Email, refundList[0].CustomerEmail);
        Assert.Equal(100, refundList[0].TotalAmount);
    }
}
