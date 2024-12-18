using LocalVenue.Shared.Enums;
using Microsoft.JSInterop;

namespace LocalVenue.Helpers;

public class Toast
{
    
    public static async void SimpleToast(ToastType toastType, string message, IJSRuntime jsRuntime, int timeInSeconds = 3)
    {
        timeInSeconds *= 1000;
        
        switch (toastType)
        {
            case ToastType.Success:
                await jsRuntime.InvokeAsync<string>("Toast.success", message, timeInSeconds);
                break;
            case ToastType.Info:
                await jsRuntime.InvokeAsync<object>("Toast.info", message, timeInSeconds);
                break;
            case ToastType.Error:
                await jsRuntime.InvokeAsync<object>("Toast.error", message, timeInSeconds);
                break;
            case ToastType.Warning:
                await jsRuntime.InvokeAsync<object>("Toast.warning", message, timeInSeconds);
                break;
            case ToastType.Message:
                await jsRuntime.InvokeAsync<object>("Toast.message", message, timeInSeconds);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(toastType), toastType, null);
        }
    }
    
}