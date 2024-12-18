using Microsoft.JSInterop;

namespace LocalVenue.Helpers;

public static class Shake
{
    public static async void NormalHorizontalShake(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.Shaking.normalHorizontalShake", id);
    }

    public static async void ErrorHorizontalShake(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.Shaking.errorHorizontalShake", id);
    }
    
}