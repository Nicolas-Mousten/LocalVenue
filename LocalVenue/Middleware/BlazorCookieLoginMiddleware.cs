using System.Collections.Concurrent;
using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Shared.WebComponents;

public class LoginInfo
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; } = "/";
}

public class BlazorCookieLoginMiddleware(RequestDelegate next)
{
    public static IDictionary<Guid, LoginInfo> Logins { get; private set; } =
        new ConcurrentDictionary<Guid, LoginInfo>();

    public async Task Invoke(
        HttpContext context,
        SignInManager<Customer> signInMgr,
        UserManager<Customer> userManager
    )
    {
        if (context.Request.Path == "/login" && context.Request.Query.ContainsKey("key"))
        {
            if (
                !context.Request.Query.TryGetValue("key", out var keyString)
                || string.IsNullOrEmpty(keyString)
            )
            {
                context.Response.Redirect("/loginfailed");
                return;
            }
            var parseResult = Guid.TryParse(keyString, out var key);
            if (parseResult == false)
            {
                context.Response.Redirect("/loginfailed");
                return;
            }

            var info = Logins[key];

            var user = await userManager.FindByEmailAsync(info.Email);

            if (user is null)
            {
                context.Response.Redirect("/loginfailed");
                return;
            }

            var result = await signInMgr.PasswordSignInAsync(
                user,
                info.Password,
                false,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                Logins.Remove(key);
                context.Response.Redirect(info.ReturnUrl ?? "/");
            }
            else if (result.RequiresTwoFactor)
            {
                context.Response.Redirect("/loginwith2fa/" + key);
            }
            else
            {
                context.Response.Redirect("/loginfailed");
            }
        }
        else
        {
            await next.Invoke(context);
        }
    }
}
