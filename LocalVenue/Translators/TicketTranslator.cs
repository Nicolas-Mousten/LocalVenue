using System.Diagnostics.CodeAnalysis;

namespace LocalVenue.Translators;

public class TicketTranslator
{
    [return: NotNullIfNotNull(nameof(ticket.Seat))]
    public static Web.Models.Ticket? Translate(Core.Entities.Ticket ticket)
    {
        if (ticket.Seat is null)
        {
            return null;
        }

        return new Web.Models.Ticket
        {
            Id = ticket.TicketId,
            Seat = SeatTranslator.Translate(ticket.Seat),
            Price = ticket.Price,
            Status = ticket.Status,
            SoldToCustomerId = ticket.CustomerId,
        };
    }
}