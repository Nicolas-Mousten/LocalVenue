using Microsoft.JSInterop;

namespace LocalVenue.Helpers;

public static class Remove
{
    public static async void RemoveListItem(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.RemoveItem.RemoveListItem", id);
    }
    
    public static async void StateChangeAfterRemoveListItem(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.RemoveItem.StateChangeAfterRemoveListItem", id);
    }
}