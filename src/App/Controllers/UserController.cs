using System.Security.Claims;
using System.Threading.Tasks;
using App.Authorization;
using App.Models;
using App.Services;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public class UserController(ISiteService siteService, IAccountService accountService) : ControllerBase(siteService)
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

    [HttpGet("activity/{id:int}/posts/{slug?}")]
    public async Task<IActionResult> PostActivity(int id, string? slug = null, [FromQuery] int page = 1)
    {
        var user = await accountService.BuildPostActivityModel(id, slug, page);
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loggedIn = await accountService.AuthenticateUser(model.Username, model.Password, model.RememberMe);

        if (loggedIn)
        {
            return LocalRedirect(model.ReturnUrl ?? "/");
        }
        

        // Something failed. Redisplay the form.
        ModelState.AddModelError(nameof(LoginModel.Username), "Invalid login attempt.");
        return View(model);
    }


    [Authorize]
    [HttpGet("logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return LocalRedirect("/");
    }

    [HttpGet("signup")]
    [AllowAnonymous]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> Create(CreateUserModel user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        if (!await accountService.IsUsernameAvailable(user.Username ?? string.Empty))
        {
            ModelState.AddModelError(nameof(CreateUserModel.Username), "That username is already in use");
            return View(user);
        }

        var result = await accountService.CreateUser(user);
        if (result)
        {
            return LocalRedirect("/");
        }

        ModelState.AddModelError(nameof(CreateUserModel.Username), "Failed to create user");
        return View(user);
    }
}