using Microsoft.AspNetCore.Identity;

namespace LocalVenue.Core.Entities;

public class Customer : IdentityUser
{
    public bool IsGoldenMember { get; set; }
    ICollection<Ticket> Tickets { get; set; }
}