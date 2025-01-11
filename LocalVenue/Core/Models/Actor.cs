using LocalVenue.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LocalVenue.Core.Models;

public class Actor
{
    [MaxLength(255)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Gender Gender { get; set; }
}