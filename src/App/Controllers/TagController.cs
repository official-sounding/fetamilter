using App.Models;
using App.Services;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("/tags")]
public class TagController(ISiteService siteService, DataContext context) : ControllerBase(siteService)
{
    [HttpGet("{tagName}")]
    public async Task<IActionResult> ByName(string tagName, [FromQuery] int? pageNumber = null)
    {
        var posts = context.Posts
            .Where(p => p.SiteID == SubSite.ID)
            .Where(p => p.Tags.Any(t => t.Name == tagName));


        return View(new HomepageModel()
        {
            Posts = await PostModel.BuildPostList(posts, pageNumber ?? 1)
        });
    }
}