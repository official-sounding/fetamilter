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
            Posts = await PaginatedList<HomepageModel.PostModel>.CreateAsync(posts.AsNoTracking().Select(p => new HomepageModel.PostModel(p, p.Comments.Count(), 0)), pageNumber ?? 1, PageSize)
        });
    }

    [HttpGet("{postId:int}")]
    public async Task<IActionResult> Post(int postId)
    {
        var post = await context.Posts
            .Where(p => p.ID == postId)
            .Where(p => p.Site.Slug == SubSite)
            .Include(p => p.Site)
            .Include(p => p.PostedBy)
            .SingleOrDefaultAsync();

        if (post is null)
        {
            return NotFound();
        }

        await context.Comments.Where(c => c.Post.ID == postId).Include(c => c.PostedBy).LoadAsync();

        return View(new PostpageModel() { Post = post });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
