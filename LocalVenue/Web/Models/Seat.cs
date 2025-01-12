namespace LocalVenue.Web.Models;

public class Seat
{
    public long SeatId { get; set; }
    public string Section { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
}
