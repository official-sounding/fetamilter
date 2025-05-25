using System.Security.Claims;
using App.Models;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

public class UserController(ILogger<UserController> logger, DataContext context) : ControllerBase
{

    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        return View(new LoginModel() { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await AuthenticateUser(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(new LoginModel() { Username = model.Username, ReturnUrl = model.ReturnUrl });
            }

            var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, "make_post"),
            new(ClaimTypes.Role, "make_comment"),
        };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            logger.LogInformation("User {Email} logged in at {Time}.",
                user.UserName, DateTime.UtcNow);

            return LocalRedirect(model.ReturnUrl ?? "/");
        }

        // Something failed. Redisplay the form.
        return View(new LoginModel() { Username = model.Username, ReturnUrl = model.ReturnUrl });
    }


    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return LocalRedirect("/");
    }

    private async Task<User?> AuthenticateUser(string? username, string? password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));

        if (user is not null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }
}