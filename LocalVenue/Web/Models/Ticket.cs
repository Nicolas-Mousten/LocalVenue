using LocalVenue.Core.Enums;

namespace LocalVenue.Web.Models;

public class Ticket
{
    public long Id { get; set; }
    public Seat Seat { get; set; }
    public decimal Price { get; set; }
    public Status Status { get; set; }
    public string? SoldToCustomerId { get; set; }
    
    
}