namespace DineTime.Shared.Enums;

public enum Direction
{
    Top = 0,
    Bottom = 1,
    Left = 2,
    Right = 3
    
}

public static class DirectionExtensions
{
    public static string ToDirectionString(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => "top",
            Direction.Bottom => "bottom",
            Direction.Left => "left",
            Direction.Right => "right",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}