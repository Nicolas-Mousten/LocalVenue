using System.ComponentModel.DataAnnotations;

namespace LocalVenue.Core;

public class Show
{
    [Key]
    public long ShowId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    
}