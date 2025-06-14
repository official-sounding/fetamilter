using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using App.Authorization;
using System.Web;
using App.Services;

namespace App.Controllers;

public class HomeController(ISiteService siteService, DataContext context, ILogger<HomeController> logger) : ControllerBase(siteService)
{
    private const int PageSize = 25;
    public async Task<IActionResult> Index(int? pageNumber = 0, CancellationToken ct = default)
    {
        logger.BeginScope(SiteSlug);
        logger.LogDebug("Load Homepage for {SubSite}", SiteSlug);

        var posts = context.Posts.Where(p => p.Site == SubSite).Include(p => p.PostedBy).OrderByDescending(p => p.PostedOn);

        return View(new HomepageModel()
        {
            Site = SubSite,
            Posts = await PaginatedList<HomepageModel.PostModel>.CreateAsync(posts.AsNoTracking().Select(p => new HomepageModel.PostModel(p, p.Comments.Count(), 0)), pageNumber ?? 1, PageSize)
        });
    }

    [HttpGet("create")]
    [Authorize(Policy = Policy.MakePost)]
    public IActionResult CreatePost()
    {
        return View();
    }

    [HttpPost("create")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> CreatePost(CreatePostModel post)
    {

        if (ModelState.IsValid)
        {

            var dbModel = new Post()
            {
                Body = post.Body ?? string.Empty,
                Title = post.Title ?? string.Empty,
                MoreInside = post.MoreInside,
                SiteID = SubSite.ID,
                PostedByID = User.GetUserId(),
                PostedOn = DateTime.UtcNow
            };

            context.Posts.Add(dbModel);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(post);
    }

    [HttpGet("{postNum:int}")]
    public async Task<IActionResult> Post(int postNum, [FromQuery] string? commentError = null)
    {
        var post = await context.Posts
            .Where(p => p.Number == postNum)
            .Where(p => p.SiteID == SubSite.ID)
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
    public async Task<IActionResult> CreateComment(int postNum, [FromForm] CreateCommentModel form)
    {
        if (string.IsNullOrWhiteSpace(form?.Body))
        {
            return RedirectToAction(nameof(Post), new { postNum, commentError = "cannot post an empty comment" });
        }
        var post = await context.Posts
                    .Where(p => p.Number == postNum)
                    .Where(p => p.SiteID == SubSite.ID)
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

        return RedirectToAction(nameof(Post), new { postNum });

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
