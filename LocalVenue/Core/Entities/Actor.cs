using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LocalVenue.Core.Entities;

public class Actor
{
    [MaxLength(255)]
    public required string Character { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    public required int Gender { get; set; }
}