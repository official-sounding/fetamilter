using App.Models;
using App.Services;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Controllers;

public abstract class ControllerBase(ISiteService siteService) : Controller
{
    protected readonly ISiteService _siteService = siteService;
    public string SiteSlug => Request.Host.Host.Split('.')[0] ?? "www";
    public SiteViewModel SubSite => _siteService.SiteBySlug(SiteSlug);
    internal int PageSize = 50;

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        ViewData["SiteTitle"] = SubSite.Title;
        ViewData["Site"] = SubSite;
        ViewData["Sites"] = _siteService.AllSites().ToArray();
        ViewData["IsLoggedIn"] = User.Identity?.IsAuthenticated ?? false;
        ViewData["Username"] = User.Identity?.Name;
        base.OnActionExecuted(context);
    }
}