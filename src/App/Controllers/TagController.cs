using App.Models;
using App.Services;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[Route("/tags")]
public class TagController(ISiteService siteService, DataContext context) : ControllerBase(siteService)
{
    [HttpGet("{tagName}")]
    public async Task<IActionResult> ByName(string tagName, [FromQuery] int? pageNumber = null)
    {
        var posts = context.Posts
            .Where(p => p.SiteID == SubSite.ID)
            .Where(p => p.Tags.Any(t => t.Name == tagName))
            .Include(p => p.PostedBy)
            .OrderByDescending(p => p.PostedOn);


        return View(new HomepageModel()
        {
            Site = SubSite,
            Posts = await PaginatedList<HomepageModel.PostModel>.CreateAsync(posts.AsNoTracking().Select(p => new HomepageModel.PostModel(p, p.Comments.Count(), p.Favorites.Count())), pageNumber ?? 1, PageSize)
        });
    }
}