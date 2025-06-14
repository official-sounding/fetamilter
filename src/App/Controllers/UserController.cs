using System.Security.Claims;
using App.Authorization;
using App.Models;
using App.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public class UserController(ILogger<UserController> logger, ISiteService siteService, IAccountService accountService) : ControllerBase(siteService)
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
                    new(ClaimTypes.Sid, $"{user.ID}"),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Role, Policy.MakeComment),
                    new(ClaimTypes.Role, Policy.MakePost),
                };

                if (user.Role.Name == "Moderator")
                {
                    claims.AddRange([
                        new(ClaimTypes.Role, Policy.DeletePost),
                        new(ClaimTypes.Role, Policy.DeleteComment),
                        new(ClaimTypes.Role, Policy.DisableUser),
                        new(ClaimTypes.Role, Policy.PostOfficially),
                        new(ClaimTypes.Role, Policy.ViewFlags)
                    ]);
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(365)
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