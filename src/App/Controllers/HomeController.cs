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

public class HomeController(ISiteService siteService, IPostService postService, DataContext context, ILogger<HomeController> logger) : ControllerBase(siteService)
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
            Posts = await PaginatedList<HomepageModel.PostModel>.CreateAsync(posts.AsNoTracking().Select(p => new HomepageModel.PostModel(p, p.Comments.Count(), p.Favorites.Count())), pageNumber ?? 1, PageSize)
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
        var post = await postService.PostBySiteAndNumber(SubSite, postNum, true);

        if (post is null)
        {
            return NotFound();
        }

        return View(new PostpageModel() { Post = post, CommentError = commentError });
    }

    [HttpGet("{postNum:int}/favorites")]
    public async Task<IActionResult> FavoriteDetails(int postNum)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum);

        if (post is null)
        {
            return NotFound();
        }

        var favorites = await context.PostFavorites
        .Where(p => p.PostID == post.ID)
        //.Include(pf => pf.User)
        .Select(pf => new FavoriteDetail(pf.UserID, pf.User!.UserName, pf.FavoritedOn))
        .ToListAsync();

        return View(new FavoriteDetailModel() { Favorites = favorites, PostNum = postNum });
    }

    [HttpGet("{postNum:int}/{commentId:int}/favorites")]
    public async Task<IActionResult> FavoriteDetails(int postNum, int commentId)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum);

        if (post is null)
        {
            return NotFound();
        }

        var favorites = await context.CommentFavorites
        .Where(p => p.CommentID == post.ID)
        //.Include(pf => pf.User)
        .Select(pf => new FavoriteDetail(pf.UserID, pf.User!.UserName, pf.FavoritedOn))
        .ToListAsync();

        return View(new FavoriteDetailModel() { Favorites = favorites, PostNum = postNum, CommentId = commentId });
    }

    [HttpPost("{postNum:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> AddFavorite(int postNum)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum);
        if (post is null)
        {
            return NotFound();
        }

        var successful = false;
        if ((await context.PostFavorites.CountAsync(pf => pf.PostID == post.ID && pf.UserID == User.GetUserId())) == 0)
        {
            context.PostFavorites.Add(new PostFavorite() { PostID = post.ID, UserID = User.GetUserId(), FavoritedOn = DateTime.UtcNow });
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.PostFavorites.CountAsync(pf => pf.PostID == post.ID);
        return Json(new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful });
    }

    [HttpDelete("{postNum:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> DeleteFavorite(int postNum)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum);
        if (post is null)
        {
            return NotFound();
        }

        var pf = await context.PostFavorites.Where(pf => pf.PostID == post.ID && pf.UserID == User.GetUserId()).SingleOrDefaultAsync();

        var successful = false;
        if (pf != null)
        {
            context.PostFavorites.Remove(pf);
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.PostFavorites.CountAsync(pf => pf.PostID == post.ID);
        return Json(new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful });
    }

    [HttpPost("{postNum:int}/{commentId:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> AddCommentFavorite(int postNum, int commentId)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum);
        if (post is null)
        {
            return NotFound();
        }

        var successful = false;
        if ((await context.CommentFavorites.CountAsync(pf => pf.CommentID == commentId && pf.UserID == User.GetUserId())) == 0)
        {
            context.CommentFavorites.Add(new CommentFavorite() { CommentID = commentId, UserID = User.GetUserId(), FavoritedOn = DateTime.UtcNow });
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.CommentFavorites.CountAsync(pf => pf.CommentID == commentId);
        return Json(new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful });
    }

    [HttpDelete("{postNum:int}/{commentId:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> DeleteCommentFavorite(int postNum, int commentId)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum);
        if (post is null)
        {
            return NotFound();
        }

        var pf = await context.CommentFavorites.Where(pf => pf.CommentID == commentId && pf.UserID == User.GetUserId()).SingleOrDefaultAsync();

        var successful = false;
        if (pf != null)
        {
            context.CommentFavorites.Remove(pf);
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.CommentFavorites.CountAsync(pf => pf.CommentID == commentId);
        return Json(new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful });
    }

    [HttpPost("{postNum:int}/comment")]
    [Authorize(Policy = Policy.MakeComment)]
    public async Task<IActionResult> CreateComment(int postNum, [FromForm] CreateCommentModel form)
    {
        if (string.IsNullOrWhiteSpace(form?.Body))
        {
            return RedirectToAction(nameof(Post), new { postNum, commentError = "cannot post an empty comment" });
        }

        var post = await postService.PostBySiteAndNumber(SubSite, postNum);

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
