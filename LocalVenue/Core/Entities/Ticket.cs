using System.ComponentModel.DataAnnotations;
using LocalVenue.Core.Enums;

namespace LocalVenue.Core.Entities;

public class Ticket
{
    [Key]
    public long TicketId { get; set; }
    public long ShowId { get; set; }
    public Show? Show { get; set; }
    public long SeatId { get; set; }
    public Seat? Seat { get; set; }
    public decimal Price { get; set; }
    public Status Status { get; set; }
    
    public string? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
}