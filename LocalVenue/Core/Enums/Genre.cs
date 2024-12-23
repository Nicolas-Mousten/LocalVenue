using System.ComponentModel;
using Microsoft.OpenApi.Attributes;

namespace LocalVenue.Core.Enums;

public enum Genre
{
    Musical = 0,
    Comedy = 1,
    Drama = 2,
    Horror = 3,
    Romance = 4,
    Documentary = 5,
}

public static class Genre1
{
    public static string ToNewString(this Genre Genre)
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






