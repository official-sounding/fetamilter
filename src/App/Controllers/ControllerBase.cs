using App.Services;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public abstract class ControllerBase(ISiteService siteService) : Controller
{
    protected readonly ISiteService _siteService = siteService;
    public string SiteSlug => Request.Host.Host.Split('.')[0] ?? "www";
    public Site SubSite => _siteService.SiteBySlug(SiteSlug);
}