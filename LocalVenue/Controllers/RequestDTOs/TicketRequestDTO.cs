using LocalVenue.Core.Enums;

namespace LocalVenue.RequestDTOs;

public class TicketRequestDTO
{
    public long ShowId { get; set; }

    public long SeatId { get; set; }

    public decimal Price { get; set; }

    public Status Status { get; set; }

    public string? CustomerId { get; set; }
}