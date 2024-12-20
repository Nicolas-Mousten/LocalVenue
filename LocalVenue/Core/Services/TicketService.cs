using LocalVenue.Core.Models;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LocalVenue.Core.Services;

public class TicketService : GenericCRUDService<Ticket>, ITicketService
{
    private readonly VenueContext _context;
    private readonly IMapper _mapper;

    public TicketService(VenueContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TicketDTO_Nested> GetTicket(long id)
    {
        var returnTicket = await base.GetItem(id, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
        return _mapper.Map<TicketDTO_Nested>(returnTicket);
    }

    public async Task<TicketDTO_Nested> AddTicket(Ticket ticket)
    {
        //Check if the ticket's seat, customer, and show exist
        if (await _context.Seats.FindAsync(ticket.SeatId) == null)
        {
            throw new ArgumentException($"Seat with id '{ticket.SeatId}' does not exist");
        }
        if (!string.IsNullOrWhiteSpace(ticket.CustomerId))
        {
            if (await _context.Users.FindAsync(ticket.CustomerId) == null)
            {
                throw new ArgumentException($"Customer with id '{ticket.CustomerId}' does not exist");
            }
        }
        else
        {
            ticket.CustomerId = null;
        }
        if (await _context.Shows.FindAsync(ticket.ShowId) == null)
        {
            throw new ArgumentException($"Show with id '{ticket.ShowId}' does not exist");
        }

        //Check if the ticket for show and seat already exists
        if (await _context.Tickets.AnyAsync(t => t.ShowId == ticket.ShowId && t.SeatId == ticket.SeatId))
        {
            throw new ArgumentException($"Ticket for show '{ticket.ShowId}' already has seat '{ticket.SeatId}' assigned");
        }

        var returnTicket = await base.AddItem(ticket, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
        return _mapper.Map<TicketDTO_Nested>(returnTicket);
    }

    public async Task<TicketDTO_Nested> UpdateTicket(Ticket ticket)
    {
        if (await _context.Seats.FindAsync(ticket.SeatId) == null)
        {
            throw new ArgumentException($"Seat with id '{ticket.SeatId}' does not exist");
        }
        if (!string.IsNullOrWhiteSpace(ticket.CustomerId))
        {
            if (await _context.Users.FindAsync(ticket.CustomerId) == null)
            {
                throw new ArgumentException($"Customer with id '{ticket.CustomerId}' does not exist");
            }
        }
        else
        {
            ticket.CustomerId = null;
        }
        if (await _context.Shows.FindAsync(ticket.ShowId) == null)
        {
            throw new ArgumentException($"Show with id '{ticket.ShowId}' does not exist");
        }

        var returnTicket = await base.UpdateItem(ticket, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
        return _mapper.Map<TicketDTO_Nested>(returnTicket);
    }

    public async Task<TicketDTO_Nested> DeleteTicket(long id)
    {
        var returnTicket = await base.DeleteItem(id, ticket => ticket.Show!, ticket => ticket.Customer!, ticket => ticket.Seat!);
        return _mapper.Map<TicketDTO_Nested>(returnTicket);
    }
}