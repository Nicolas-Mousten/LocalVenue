using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LocalVenue.Core.Enums;

namespace LocalVenue.Core.Models;

public class Actor
{
    [MaxLength(255)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }
}
