namespace LocalVenue.Core.Enums;

public enum Status
{
    Available = 0,
    Sold = 1,
    Reserved = 2,
    [Obsolete("This status is not yet in use")]
    ReSeller = 3,
}