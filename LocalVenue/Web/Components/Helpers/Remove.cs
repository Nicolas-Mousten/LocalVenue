using Microsoft.JSInterop;

namespace LocalVenue.Helpers;

public static class Remove
{
    public static async Task RemoveListItem(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.RemoveItem.RemoveListItem", id);
    }

    public static async Task StateChangeAfterRemoveListItem(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.RemoveItem.StateChangeAfterRemoveListItem", id);
    }
}
