using App.Models;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("/tags")]
public class TagController(ISiteService siteService, IPostService postService) : ControllerBase(siteService, postService)
{
    [HttpGet("{tagName}")]
    public async Task<IActionResult> ByName(string tagName, [FromQuery] int? pageNumber = null)
    {
        var posts = _postService.PostList(SubSite, p => p.Tags.Any(t => t.Name == tagName));


        return View(new PostListModel()
        {
            Posts = await PaginatedList<PostModel>.CreateAsync(posts, pageNumber ?? 1, PageSize)
        });
    }
}
