using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LocalVenue.Core.Entities;

public class Customer : IdentityUser
{
    [Required]
    [MaxLength(255)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(255)]
    public required string LastName { get; set; }

    public bool IsGoldenMember { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
