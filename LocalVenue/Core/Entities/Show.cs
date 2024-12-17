using System.ComponentModel.DataAnnotations;
using LocalVenue.Core.Enums;

namespace LocalVenue.Core.Entities;

public class Show
{
    [Key]
    public long ShowId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Genre Genre { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; }
    
}