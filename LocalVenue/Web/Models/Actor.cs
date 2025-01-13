using System.ComponentModel.DataAnnotations;
using LocalVenue.Core.Enums;

namespace LocalVenue.Web.Models;

public class Actor
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }
}
