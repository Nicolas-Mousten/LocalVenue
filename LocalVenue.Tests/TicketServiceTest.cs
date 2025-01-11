using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;
using LocalVenue.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LocalVenue.Tests;
public class TicketServiceTest
{
    private readonly ServiceProvider serviceProvider;
    
    
    public TicketServiceTest()
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
    public async Task TestTicketServiceBuySeat()
    {
        // Arrange
        var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();
        
        Customer customer = new Customer
        {
            Id = "0c9cd65f-2054-4086-a569-2e50997a8be9",
            UserName = "Nicolas",
            NormalizedUserName = "NICOLAS",
            Email = "nicolas.mousten@gmail.com",
            NormalizedEmail = "NICOLAS.MOUSTEN@GMAIL.COM",
            EmailConfirmed = false,
            PasswordHash = "AQAAAAIAAYagAAAAEJWgg4FDKFWNh/AYIzVE/3nxRluYnwDmUfDnpc75ZUylWzJYkphFBrhqFkRAgm16YA==",
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
            Tickets = new List<Ticket>()
        };
        Show show = new Show
        {
            ShowId = 1,
            Title = "TestShow",
            Description = "A show to test on",
            StartTime = DateTime.Now.AddDays(1).AddHours(12),
            EndTime = DateTime.Now.AddDays(1).AddHours(14),
            Genre = Genre.Musical,
            OpeningNight = false,
            Tickets = new List<Ticket>()
        };
        Seat seat = new Seat
        {
            SeatId = 1,
            Section = "Front",
            Row = 1,
            Number = 1
        };
        Ticket ticket = new Ticket
        {
            TicketId = 1,
            ShowId = show.ShowId,
            Show = show,
            SeatId = 1,
            Seat = seat,
            Price = 50,
            Status = Status.Available,
            CustomerId = null,
            Customer = null
        };
        
        show.Tickets?.Add(ticket);
        await using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            context.Users.Add(customer);
            context.Shows.Add(show);
            await context.SaveChangesAsync();
        }

        // Act
        var contextFactoryRetrieve = serviceProvider.GetRequiredService<IDbContextFactory<VenueContext>>();

        var service = new TicketService(contextFactoryRetrieve);

        await service.JoinShow(show.Tickets.ToList(), customer.Id.ToString());
        var result = await service.GetTicket(ticket.TicketId);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticket.CustomerId, result.CustomerId);
        Assert.Equal(ticket.Status, result.Status);
    }
    
    
}
