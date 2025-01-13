using Microsoft.JSInterop;

namespace LocalVenue.Helpers;

public static class Shake
{
    public static async Task NormalHorizontalShake(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.Shaking.normalHorizontalShake", id);
    }

    public static async Task ErrorHorizontalShake(string id, IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("LocalVenue.Shaking.errorHorizontalShake", id);
    }
}
