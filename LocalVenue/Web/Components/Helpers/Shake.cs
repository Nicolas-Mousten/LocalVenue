using Microsoft.JSInterop;

namespace DineTime.Helpers;

public static class Shake
{
    public static async void NormalHorizontalShake(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("DineTime.Shaking.normalHorizontalShake", id);
    }

    public static async void ErrorHorizontalShake(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("DineTime.Shaking.errorHorizontalShake", id);
    }
    
}