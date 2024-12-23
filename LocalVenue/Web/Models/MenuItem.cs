namespace LocalVenue.Web.Models;

public class MenuItem
{
    public required string Name { get; set; }
    public required string Icon { get; set; }
    public required string Link { get; set; }
    public bool Visible { get; set; } = true;
}