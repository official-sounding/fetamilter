using App.Models;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace App.ViewComponents;


public class Header(DataContext context) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var subsite = Request.Host.Host.Split('.')[0] ?? "www";
        var site = context.Sites.Single(s => s.Slug == subsite);
        return View(new HeaderModel(site, User.Identity?.IsAuthenticated ?? false, User.Identity?.Name));
    }
}