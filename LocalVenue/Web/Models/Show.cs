using System.ComponentModel.DataAnnotations;
using LocalVenue.Core.Enums;

namespace LocalVenue.Web.Models;

public class Show
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Skal udfyldes")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Skal udfyldes")]
    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Genre Genre { get; set; }
    public List<Ticket>? Tickets { get; set; }
}