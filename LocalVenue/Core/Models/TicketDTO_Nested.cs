using LocalVenue.Core.Entities;
using LocalVenue.Core.Enums;

namespace LocalVenue.Core.Models;

public class TicketDTO_Nested
{
    public long TicketId { get; set; }
    public long ShowId { get; set; }
    public ShowDTO? Show { get; set; }
    public long SeatId { get; set; }
    public Seat? Seat { get; set; }
    public decimal Price { get; set; }
    public Status Status { get; set; }

    public string? CustomerId { get; set; }
    public Customer? Customer { get; set; }

}