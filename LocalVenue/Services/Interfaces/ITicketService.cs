using LocalVenue.Core.Entities;

namespace LocalVenue.Core.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket> GetTicket(long id);
        Task<Ticket> AddTicket(Ticket ticket);
        Task<Ticket> UpdateTicket(Ticket ticket);
        Task<Ticket> DeleteTicket(long id);
    }
}