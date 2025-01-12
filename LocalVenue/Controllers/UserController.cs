﻿using LocalVenue.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LocalVenue.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
    SignInManager<Customer> signInManager,
    UserManager<Customer> userManager
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest("Invalid login attempt.");
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );
        if (result.Succeeded)
        {
            // Get the authentication cookie
            var authCookie = HttpContext
                .Response.Headers["Set-Cookie"]
                .FirstOrDefault(header =>
                    header != null && header.StartsWith(".AspNetCore.Identity.Application")
                );

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
