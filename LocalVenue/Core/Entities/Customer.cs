using Microsoft.AspNetCore.Identity;

namespace LocalVenue.Core.Entities;

public class Customer : IdentityUser
{
    public bool IsGoldenMember { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}