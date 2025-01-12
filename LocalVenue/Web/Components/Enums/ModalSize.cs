namespace LocalVenue.Shared.Enums;

public enum ModalSize
{
    Small = 1,
    Default = 2,
    Large = 3,
    ExtraLarge = 4,
}

public static class ModalSizeExtensions
{
    public static string ToCssClass(this ModalSize modalSize)
    {
        return modalSize switch
        {
            ModalSize.Small => "modal-sm",
            ModalSize.Default => "",
            ModalSize.Large => "modal-lg",
            ModalSize.ExtraLarge => "modal-xl",
            _ => throw new ArgumentOutOfRangeException(nameof(modalSize), modalSize, null),
        };
    }
}
