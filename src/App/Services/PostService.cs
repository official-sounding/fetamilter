using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services;

public interface IPostService
{
    Task<Post?> PostBySiteAndNumber(Site site, int postNum, bool includeDetails = false);
}

public class PostService(DataContext context) : IPostService
{
    public async Task<Post?> PostBySiteAndNumber(Site site, int postNum, bool includeDetails = false)
    {
        var query = context.Posts
            .Where(p => p.Number == postNum)
            .Where(p => p.SiteID == site.ID);

        if (includeDetails)
        {
            query = query
            .Include(p => p.Site)
            .Include(p => p.Favorites)
            .Include(p => p.PostedBy);
        }

        var post = await query.OrderBy(p => p.ID).SingleOrDefaultAsync();

        if (includeDetails && post is not null)
        {
            await context.Comments.Where(c => c.Post.ID == post.ID).Include(c => c.PostedBy).Include(c => c.Favorites).LoadAsync();
        }

        return post;
    }
}