using LocalVenue.Core;
using LocalVenue.Core.Entities;

namespace LocalVenue.Services.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket> GetTicket(long id);
        Task<Ticket> AddTicket(Ticket ticket, VenueContext? context = null);
        Task<Ticket> UpdateTicket(Ticket ticket);
        Task<Ticket> DeleteTicket(long id);
        Task JoinShow(List<Ticket> tickets, string customerId);
        Task<string> LeaveShow(List<Ticket> tickets, string customerId);
    }
}