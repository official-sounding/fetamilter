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
using System.ServiceModel.Syndication;
using System;
using System.Xml;
using System.Text;

namespace App.Controllers;

public class HomeController(ISiteService siteService, IPostService postService, DataContext context, ILogger<HomeController> logger) : ControllerBase(siteService)
{
    public async Task<IActionResult> Index(int pageNumber = 1, CancellationToken ct = default)
    {
        logger.BeginScope(SiteSlug);
        logger.LogDebug("Load Homepage for {SubSite}", SiteSlug);

        var posts = context.Posts.Where(p => p.Site.ID == SubSite.ID);

        return View(new HomepageModel()
        {
            Posts = await PostModel.BuildPostList(posts, pageNumber)
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
            var tags = string.IsNullOrWhiteSpace(post.TagList) ? [] : await postService.TagsFromString(post.TagList);
            var dbModel = new Post()
            {
                Body = post.Body ?? string.Empty,
                Title = post.Title ?? string.Empty,
                MoreInside = post.MoreInside,
                SiteID = SubSite.ID,
                PostedByID = User.GetUserId(),
                PostedOn = DateTime.UtcNow,
                Tags = [.. tags]
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

    [HttpGet("{postNum:int}/rss")]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> PostRss(int postNum)
    {
        var post = await postService.PostBySiteAndNumber(SubSite, postNum, true);

        if (post is null)
        {
            return NotFound();
        }


        var feed = new SyndicationFeed(post.Title, $"Comments on Post {postNum}", new Uri("https://example.com"), "RSSUrl", DateTime.Now);

        var items = new List<SyndicationItem>();
        foreach (var item in post.Comments)
        {
            var postUrl = new Uri($"{Url.Action(nameof(Post), "Home", new { postNum }, HttpContext.Request.Scheme)}#{item.ID}");
            var title = $"By {item.PostedBy.UserName}";
            var description = item.Body;
            items.Add(new SyndicationItem(title, description, postUrl, $"{SubSite.Slug}-comment-{item.ID}", item.PostedOn));
        }
        feed.Items = items;

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true,
        };
        using var stream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(stream, settings);

        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(xmlWriter);
        xmlWriter.Flush();

        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");

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
