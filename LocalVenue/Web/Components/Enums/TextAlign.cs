namespace LocalVenue.Shared.Enums;

public enum TextAlign
{
    Left = 0,
    Right = 1,
    Center = 2
}

public static class TextAlignExtensions
{
    public static string ToCss(this TextAlign textAlign)
    {
        return textAlign switch
        {
            TextAlign.Left => "justify-content-start",
            TextAlign.Right => "justify-content-end",
            TextAlign.Center => "justify-content-center",
            _ => throw new ArgumentOutOfRangeException(nameof(textAlign), textAlign, null)
        };
    }
}