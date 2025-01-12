using LocalVenue.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LocalVenue.Web.Models;

public class Actor
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Gender Gender { get; set; }
}
