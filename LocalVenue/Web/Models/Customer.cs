using System.ComponentModel.DataAnnotations;

namespace LocalVenue.Web.Models;

public class Customer
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    [Required]
    public string? Email { get; set; }
}
