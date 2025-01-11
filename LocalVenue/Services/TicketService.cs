using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;
using LocalVenue.Core.Services;
using LocalVenue.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Services;

public class TicketService(IDbContextFactory<VenueContext> contextFactory) : GenericCRUDService<Ticket>(contextFactory), ITicketService
{
    private readonly IDbContextFactory<VenueContext> _contextFactory = contextFactory;

    public async Task<Ticket> GetTicket(long id)
    {
        return await base.GetItem(id, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
    }

    public async Task<Ticket> AddTicket(Ticket ticket)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        //Check if the ticket's seat, customer, and show exist
        if (await context.Seats.FindAsync(ticket.SeatId) == null)
        {
            throw new ArgumentException($"Seat with id '{ticket.SeatId}' does not exist");
        }
        if (!string.IsNullOrWhiteSpace(ticket.CustomerId))
        {
            if (await context.Users.FindAsync(ticket.CustomerId) == null)
            {
                throw new ArgumentException($"Customer with id '{ticket.CustomerId}' does not exist");
            }
        }
        else
        {
            ticket.CustomerId = null;
        }
        if (await context.Shows.FindAsync(ticket.ShowId) == null)
        {
            throw new ArgumentException($"Show with id '{ticket.ShowId}' does not exist");
        }

        //Check if the ticket for show and seat already exists
        if (await context.Tickets.AnyAsync(t => t.ShowId == ticket.ShowId && t.SeatId == ticket.SeatId))
        {
            throw new ArgumentException($"Ticket for show '{ticket.ShowId}' already has seat '{ticket.SeatId}' assigned");
        }

        return await base.AddItem(ticket, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
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
                throw new ArgumentException($"Customer with id '{ticket.CustomerId}' does not exist");
            }
        }
        else
        {
            ticket.CustomerId = null;
        }
        if (await context.Shows.FindAsync(ticket.ShowId) == null)
        {
            throw new ArgumentException($"Show with id '{ticket.ShowId}' does not exist");
        }

        return await base.UpdateItem(ticket, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
    }

    public async Task<Ticket> DeleteTicket(long id)
    {
        return await base.DeleteItem(id, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
    }

    public async Task JoinShow(List<Ticket> tickets, string customerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        Show? show = await context.Shows.FindAsync(tickets.First().ShowId);
        if (await context.Users.FindAsync(customerId) == null)
        {
            throw new ArgumentException($"Customer with id '{customerId}' does not exist");
        }
        
        //add the customer to those seats
        foreach (var ticket in tickets)
        {
            ticket.TicketId = ticket.TicketId;
            ticket.ShowId = ticket.ShowId;
            ticket.SeatId = ticket.SeatId;
            ticket.Price = ticket.Price;
            if (ticket.Status != Status.Available)
            {
                throw new ArgumentException($"Ticket {ticket.TicketId} is not available for purchase");
            }

            if (ticket.CustomerId != null)
            {
                throw new ArgumentException($"Ticket {ticket.TicketId} is already purchased by another customer");
            }
            ticket.Status = Status.Sold;
            ticket.CustomerId = customerId.ToString();
            await UpdateTicket(ticket);
        }

        await context.SaveChangesAsync();
    }

    public async Task<string> LeaveShow(List<Ticket> tickets, string customerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        Show? show = await context.Shows.FindAsync(tickets.First().ShowId);
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
            ticket.TicketId = ticket.TicketId;
            ticket.ShowId = ticket.ShowId;
            ticket.SeatId = ticket.SeatId;
            ticket.Price = ticket.Price;
            ticket.Status = Status.Available;
            ticket.CustomerId = null;
            await UpdateTicket(ticket);
        }
        
        await context.SaveChangesAsync();
        return "Tickets have been returned";
    }


    public bool lessThanADayLeft(Show show)
    {
        return show.StartTime <= DateTime.Now.AddHours(24);
    }
}