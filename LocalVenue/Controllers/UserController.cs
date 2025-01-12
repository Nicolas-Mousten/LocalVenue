using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LocalVenue.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly SignInManager<Customer> _signInManager;
    private readonly UserManager<Customer> _userManager;

    public UserController(SignInManager<Customer> signInManager, UserManager<Customer> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest("Invalid login attempt.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            // Get the authentication cookie
            var authCookie = HttpContext.Response.Headers["Set-Cookie"].FirstOrDefault(header => header != null && header.StartsWith(".AspNetCore.Identity.Application"));

            if (authCookie != null)
            {
                var cookieValue = authCookie.Split(';').FirstOrDefault();
                return Ok(new { CookieValue = cookieValue });
            }
        }

        if (result.IsLockedOut)
        {
            return BadRequest("User account locked out.");
        }

        return BadRequest("Invalid login attempt.");
    }
}
