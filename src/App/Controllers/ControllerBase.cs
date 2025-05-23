using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

public abstract class ControllerBase : Controller
{
    public string SubSite => Request.Host.Host.Split('.')[0] ?? "www";
}