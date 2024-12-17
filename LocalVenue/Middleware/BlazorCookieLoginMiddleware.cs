using System.Collections.Concurrent;
using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Shared.WebComponents;


public class LoginInfo
{
    public string Email { get; set; }

    public string Password { get; set; }

    public  string? ReturnUrl { get; set; } = "/";
}

public class BlazorCookieLoginMiddleware
{
    public static IDictionary<Guid, LoginInfo> Logins { get; private set; }
        = new ConcurrentDictionary<Guid, LoginInfo>();


    private readonly RequestDelegate _next;

    public BlazorCookieLoginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, SignInManager<Customer> signInMgr, UserManager<Customer> userManager)
    {
        if (context.Request.Path == "/login" && context.Request.Query.ContainsKey("key"))
        {
            var key = Guid.Parse(context.Request.Query["key"]);
            var info = Logins[key];

            var user = await userManager.FindByEmailAsync(info.Email);

            if (user is null)
            {
                context.Response.Redirect("/loginfailed");
                return;
            }

            var result = await signInMgr.PasswordSignInAsync(user, info.Password, false, lockoutOnFailure: true);
            info.Password = null;
            if (result.Succeeded)
            {
                Logins.Remove(key);
                context.Response.Redirect(info.ReturnUrl ?? "/");
                return;
            }
            else if (result.RequiresTwoFactor)
            {
                //TODO: redirect to 2FA razor component
                context.Response.Redirect("/loginwith2fa/" + key);
                return;
            }
            else
            {
                //TODO: Proper error handling
                context.Response.Redirect("/loginfailed");
                return;
            }
        }
        else
        {
            await _next.Invoke(context);
        }
    }
}