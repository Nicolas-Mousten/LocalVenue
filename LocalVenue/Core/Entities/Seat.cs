using System.ComponentModel.DataAnnotations;

namespace LocalVenue.Core.Entities;

public class Seat
{
    [Key]
    public long SeatId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Section { get; set; } = string.Empty;

    public int Row { get; set; }

    public int Number { get; set; }
}
