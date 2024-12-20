using LocalVenue.Core.Models;
using LocalVenue.Core.Entities;

namespace LocalVenue.Core.Interfaces
{
    public interface ITicketService
    {
        Task<TicketDTO_Nested> GetTicket(long id);
        Task<TicketDTO_Nested> AddTicket(Ticket ticket);
        Task<TicketDTO_Nested> UpdateTicket(Ticket ticket);
        Task<TicketDTO_Nested> DeleteTicket(long id);
    }
}