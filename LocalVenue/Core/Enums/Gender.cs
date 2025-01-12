namespace LocalVenue.Core.Enums;

public enum Gender
{
    NotSpecified = 0,
    Female = 1,
    Male = 2,
    NonBinary = 3
}

public static class Gender1
{
    public static string ToNewString(this Gender Gender)
    {
        return Gender switch
        {
            Gender.NotSpecified => "Unknown",
            Gender.Female => "Female",
            Gender.Male => "Male",
            Gender.NonBinary => "Non-binary",
            _ => throw new ArgumentOutOfRangeException(nameof(Gender), Gender, null)
        };
    }
}