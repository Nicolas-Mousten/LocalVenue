using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LocalVenue.Core.Enums;
using LocalVenue.Core.Models;

namespace LocalVenue.Core.Entities;

public class Show
{
    [Key]
    public long ShowId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [NotMapped]
    public ICollection<Actor> Actors { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Genre Genre { get; set; }
    public bool OpeningNight { get; set; }

    public ICollection<Ticket>? Tickets { get; set; }

}