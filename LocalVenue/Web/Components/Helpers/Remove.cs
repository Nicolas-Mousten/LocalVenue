using Microsoft.JSInterop;

namespace DineTime.Helpers;

public static class Remove
{
    public static async void RemoveListItem(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("DineTime.RemoveItem.RemoveListItem", id);
    }
    
    public static async void StateChangeAfterRemoveListItem(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("DineTime.RemoveItem.StateChangeAfterRemoveListItem", id);
    }
}