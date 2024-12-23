using System.ComponentModel;
using Microsoft.OpenApi.Attributes;

namespace LocalVenue.Core.Enums;

public enum Genre
{
    [Description("Musical")]
    Musical = 0,
    Comedy = 1,
    Drama = 2,
    Horror = 3,
    Romance = 4,
    Documentary = 5,
}

public class Genre1
{
    public Genre Genre { get; set; }
    public override string ToString()
    {
        return Genre switch
        {
            Genre.Musical => "Musical",
            Genre.Comedy => "Komedie",
            Genre.Drama => "Drama",
            Genre.Horror => "Gyser",
            Genre.Romance => "Romantik",
            Genre.Documentary => "Dokumentar",
            _ => throw new ArgumentOutOfRangeException(nameof(Genre), Genre, null)
        };
    }
}






