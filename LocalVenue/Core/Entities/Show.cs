using LocalVenue.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LocalVenue.Core.Entities;

public class Show
{
    [Key]
    public long ShowId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Genre Genre { get; set; }
    public bool OpeningNight { get; set; }

    public ICollection<Ticket>? Tickets { get; set; }

}