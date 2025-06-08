using App.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.ViewComponents;


public class Header : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var subsite = Request.Host.Host.Split('.')[0] ?? "www";
        return View(new HeaderModel(subsite, User.Identity?.IsAuthenticated ?? false, User.Identity?.Name));
    }
}