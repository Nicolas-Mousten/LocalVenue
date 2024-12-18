namespace LocalVenue.Shared.Enums;

public enum PopOverSize
{
    Small = 1,
    Default = 2,
    Large = 3,
}

public static class PopOverSizeExtensions
{
    public static string ToCssClass(this PopOverSize modalSize)
    {
        return modalSize switch
        {
            PopOverSize.Small => "ui-popover--sd",
            PopOverSize.Default => "ui-popover--md",
            PopOverSize.Large => "ui-popover--ld",
            _ => throw new ArgumentOutOfRangeException(nameof(modalSize), modalSize, null)
        };
    }
}