using AutoMapper;
using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;
using LocalVenue.Core.Services;
using LocalVenue.Helpers;
using LocalVenue.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Services;

public class TicketService(IDbContextFactory<VenueContext> contextFactory, IMapper mapper)
    : GenericCRUDService<Ticket>(contextFactory),
        ITicketService
{
    private readonly IDbContextFactory<VenueContext> _contextFactory = contextFactory;

    public async Task<Ticket> GetTicket(long id)
    {
        return await base.GetItem(
            id,
            ticket => ticket.Show!,
            ticket => ticket.Customer!,
            ticket => ticket.Seat!
        );
    }

    public async Task<Ticket> AddTicket(Ticket ticket, VenueContext? context = null)
    {
        if (context == null)
        {
            context = await _contextFactory.CreateDbContextAsync();
        }

        //Check if the ticket's seat, customer, and show exist
        if (await context.Seats.FindAsync(ticket.SeatId) == null)
        {
            throw new ArgumentException($"Seat with id '{ticket.SeatId}' does not exist");
        }
        if (!string.IsNullOrWhiteSpace(ticket.CustomerId))
        {
            if (await context.Users.FindAsync(ticket.CustomerId) == null)
            {
                throw new ArgumentException(
                    $"Customer with id '{ticket.CustomerId}' does not exist"
                );
            }
        }
        else
        {
            ticket.CustomerId = null;
        }

        var show = await context.Shows.FindAsync(ticket.ShowId);
        if (show == null)
        {
            throw new ArgumentException($"Show with id '{ticket.ShowId}' does not exist");
        }

        //Check if the ticket for show and seat already exists
        if (
            await context.Tickets.AnyAsync(t =>
                t.ShowId == ticket.ShowId && t.SeatId == ticket.SeatId
            )
        )
        {
            throw new ArgumentException(
                $"Ticket for show '{ticket.ShowId}' already has seat '{ticket.SeatId}' assigned"
            );
        }

        ticket.Seat = await context.Seats.FindAsync(ticket.SeatId); // price calculator needs seat
        ticket.Price = ShowPriceCalculator.CalculatePrice(show, ticket, show.OpeningNight, mapper);
        ticket.Seat = null; // remove seat from ticket to avoid duplicate insert

        return await base.AddItem(
            ticket,
            ticket => ticket.Show!,
            ticket => ticket.Customer!,
            ticket => ticket.Seat!
        );
    }

    public async Task<Ticket> UpdateTicket(Ticket ticket)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (await context.Seats.FindAsync(ticket.SeatId) == null)
        {
            throw new ArgumentException($"Seat with id '{ticket.SeatId}' does not exist");
        }
        if (!string.IsNullOrWhiteSpace(ticket.CustomerId))
        {
            if (await context.Users.FindAsync(ticket.CustomerId) == null)
            {
                throw new ArgumentException(
                    $"Customer with id '{ticket.CustomerId}' does not exist"
                );
            }
        }
        else
        {
            ticket.CustomerId = null;
        }

        var show = await context.Shows.FindAsync(ticket.ShowId);
        if (show == null)
        {
            throw new ArgumentException($"Show with id '{ticket.ShowId}' does not exist");
        }

        ticket.Seat = await context.Seats.FindAsync(ticket.SeatId); // price calculator needs seat
        ticket.Price = ShowPriceCalculator.CalculatePrice(show, ticket, show.OpeningNight, mapper);
        ticket.Seat = null; // remove seat from ticket to avoid duplicate insert

        return await base.UpdateItem(
            ticket,
            ticket => ticket.Show!,
            ticket => ticket.Customer!,
            ticket => ticket.Seat!
        );
    }

    public async Task<Ticket> DeleteTicket(long id)
    {
        return await base.DeleteItem(
            id,
            ticket => ticket.Show!,
            ticket => ticket.Customer!,
            ticket => ticket.Seat!
        );
    }

    public async Task JoinShow(
        long showId,
        List<LocalVenue.Web.Models.Ticket> tickets,
        string customerId
    )
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        Show? show = await context.Shows.FindAsync(showId);
        if (show == null)
        {
            throw new ArgumentException($"Show with id '{showId}' does not exist");
        }
        if (await context.Users.FindAsync(customerId) == null)
        {
            throw new ArgumentException($"Customer with id '{customerId}' does not exist");
        }
        
        foreach (var ticket in tickets)
        {
            Ticket newTicket = new Ticket
            {
                TicketId = ticket.Id,
                ShowId = ticket.ShowId,
                Show = show,
                SeatId = ticket.SeatId,
                Seat = new Seat
                {
                    SeatId = ticket.SeatId,
                    Section = ticket.Seat.Section,
                    Row = ticket.Seat.Row,
                    Number = ticket.Seat.Number,
                },
                Price = ticket.Price,
                Status = ticket.Status,
                CustomerId = null,
                Customer = null,
            };
            if (ticket.Status != Status.Available)
            {
                throw new ArgumentException($"Ticket {ticket.Id} is not available for purchase");
            }

            if (ticket.SoldToCustomerId != null)
            {
                throw new ArgumentException(
                    $"Ticket {ticket.Id} is already purchased by another customer"
                );
            }
            newTicket.Status = Status.Sold;
            newTicket.CustomerId = customerId.ToString();
            await UpdateTicket(newTicket);
        }

        await context.SaveChangesAsync();
    }

    public async Task<string> LeaveShow(
        long showId,
        List<LocalVenue.Web.Models.Ticket> tickets,
        string customerId
    )
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        Show? show = await context.Shows.FindAsync(showId);
        if (show == null)
        {
            throw new ArgumentException($"Show with id '{customerId}' does not exist");
        }
        if (await context.Users.FindAsync(customerId) == null)
        {
            throw new ArgumentException($"Customer with id '{customerId}' does not exist");
        }

        //Check if the time is less than a day before show starts.
        if (lessThanADayLeft(show))
        {
            return "It is not possible to return a ticket a day before the show starts.";
        }

        //add the customer to those seats
        foreach (var ticket in tickets)
        {
            Ticket newTicket = new Ticket
            {
                TicketId = ticket.Id,
                ShowId = ticket.ShowId,
                Show = show,
                SeatId = ticket.SeatId,
                Seat = new Seat
                {
                    SeatId = ticket.SeatId,
                    Section = ticket.Seat.Section,
                    Row = ticket.Seat.Row,
                    Number = ticket.Seat.Number,
                },
                Price = ticket.Price,
                Status = ticket.Status,
                CustomerId = null,
                Customer = null,
            };
            newTicket.Status = Status.Available;
            newTicket.CustomerId = null;
            await UpdateTicket(newTicket);
        }

        await context.SaveChangesAsync();
        return "Tickets have been returned";
    }

    public bool lessThanADayLeft(Show show)
    {
        return show.StartTime <= DateTime.Now.AddHours(24);
    }
}
