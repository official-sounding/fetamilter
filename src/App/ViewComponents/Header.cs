using App.Models;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.ViewComponents;


public class Header(ISiteService svc) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var allSites = svc.AllSites();
        var subsite = Request.Host.Host.Split('.')[0] ?? "www";
        var site = svc.SiteBySlug(subsite);

        return View(new HeaderModel()
        {
            Site = site,
            AllSites = [.. allSites],
            IsLoggedIn = User.Identity?.IsAuthenticated ?? false,
            Username = User.Identity?.Name
        });
    }
}