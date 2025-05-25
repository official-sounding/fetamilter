using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace App.Controllers;

public class HomeController(DataContext context, ILogger<HomeController> logger) : ControllerBase
{
    private const int PageSize = 25;
    public async Task<IActionResult> Index(int? pageNumber, CancellationToken ct = default)
    {
        logger.BeginScope(SubSite);
        logger.LogDebug("Load Homepage for {SubSite}", SubSite);
        var site = await context.Sites.FirstOrDefaultAsync((s) => s.Slug == SubSite, ct);
        site ??= await context.Sites.FirstAsync((s) => s.Slug == "www", ct);

        var posts = context.Posts.Where(p => p.Site == site).Include(p => p.PostedBy);

        return View(new HomepageModel()
        {
            Site = site,
            Posts = await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), pageNumber ?? 1, PageSize)
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
