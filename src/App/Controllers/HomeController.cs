using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using App.Authorization;
using App.Services;

namespace App.Controllers;

public class HomeController(ISiteService siteService, IPostService postService, ILogger<HomeController> logger) : ControllerBase(siteService, postService)
{
    public async Task<IActionResult> Index(int pageNumber = 1, CancellationToken ct = default)
    {
        logger.BeginScope(SiteSlug);
        logger.LogDebug("Load Homepage for {SubSite}", SiteSlug);

        var posts = _postService.PostList(SubSite);

        return View(new PostListModel()
        {
            Posts = await PaginatedList<PostModel>.CreateAsync(posts, pageNumber, PageSize)
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
            await _postService.CreatePost(post, SubSite, User.GetUserId());
            return RedirectToAction(nameof(Index));
        }

        return View(post);
    }

    [HttpGet("{postNum:int}")]
    public async Task<IActionResult> Post(int postNum, [FromQuery] string? commentError = null) => await WithPost(postNum, (post) =>
    {
        return Task.FromResult<IActionResult>(View(new PostpageModel() { Post = post, CommentError = commentError }));
    }, true);

    [HttpGet("{postNum:int}/rss")]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> PostRss(int postNum) => await WithPost(postNum, (post) =>
    {
        var postUrl = new Uri(Url.Action(nameof(Post), "Home", new { postNum }, HttpContext.Request.Scheme) ?? "");
        var rss = _postService.CreatePostRSS(post, postUrl);

        return Task.FromResult<IActionResult>(File(rss, "application/rss+xml; charset=utf-8"));
    });


    

    [HttpPost("{postNum:int}/comment")]
    [Authorize(Policy = Policy.MakeComment)]
    public async Task<IActionResult> CreateComment(int postNum, [FromForm] CreateCommentModel form)
    {
        if (string.IsNullOrWhiteSpace(form?.Body))
        {
            return RedirectToAction(nameof(Post), new { postNum, commentError = "cannot post an empty comment" });
        }

        var post = await _postService.PostBySiteAndNumber(SubSite, postNum);

        if (post is null)
        {
            return NotFound();
        }

        await _postService.AddComment(post, form, User.GetUserId());

        return RedirectToAction(nameof(Post), new { postNum });

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
