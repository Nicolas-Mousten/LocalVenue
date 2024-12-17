using System.ComponentModel.DataAnnotations;

namespace LocalVenue.Core.Entities;

public class Seat
{
    [Key]
    public long SeatId { get; set; }
    public required string Section { get; set; }
    public int Row { get; set; }
    public int Number { get; set; }
}