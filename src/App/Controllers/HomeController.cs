using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using App.Authorization;
using System.Web;

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

    [HttpGet("{postNum:int}")]
    public async Task<IActionResult> Post(int postNum, [FromQuery] string? commentError)
    {
        var post = await context.Posts
            .Where(p => p.Number == postNum)
            .Where(p => p.Site.Slug == SubSite)
            .Include(p => p.Site)
            .Include(p => p.PostedBy)
            .OrderBy(p => p.ID)
            .FirstOrDefaultAsync();

        if (post is null)
        {
            return NotFound();
        }

        await context.Comments.Where(c => c.Post.ID == post.ID).Include(c => c.PostedBy).LoadAsync();

        return View(new PostpageModel() { Post = post, CommentError = commentError });
    }

    [HttpPost("{postNum:int}/comment")]
    [Authorize(Policy = Policy.MakeComment)]
    public async Task<IActionResult> MakeComment(int postNum, [FromForm] MakeCommentModel form)
    {
        if (string.IsNullOrWhiteSpace(form?.Body))
        {
            return RedirectToAction(nameof(Post), "Home", new { postNum, commentError = "cannot post an empty comment" });
        }
        var post = await context.Posts
                    .Where(p => p.Number == postNum)
                    .Where(p => p.Site.Slug == SubSite)
                    .OrderBy(p => p.ID)
                    .FirstOrDefaultAsync();

        if (post is null)
        {
            return NotFound();
        }

        var comment = new Comment()
        {
            Body = HttpUtility.HtmlEncode(form.Body.Trim()),
            PostedOn = DateTime.UtcNow,
            PostID = post.ID,
            PostedByID = User.GetUserId()
        };

        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Post), "Home", new { postNum });

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
