using System.Security.Claims;
using App.Models;
using App.Services;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

public class UserController(ILogger<UserController> logger, IAccountService accountService) : ControllerBase
{

    [HttpGet("user/{id:int}")]
    public async Task<IActionResult> Index(int id)
    {
        var user = await accountService.BuildUserpageModel(id);
        if (user is null)
        {
            return NotFound();
        }

        return View(user);
    }

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
            var user = await accountService.AuthenticateUser(model.Username, model.Password);

            if (user is not null)
            {
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


        }

        // Something failed. Redisplay the form.
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(new LoginModel() { Username = model.Username, ReturnUrl = model.ReturnUrl });
    }


    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return LocalRedirect("/");
    }
}