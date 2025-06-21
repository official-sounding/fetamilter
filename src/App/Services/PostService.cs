using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services;

public interface IPostService
{
    Task<Post?> PostBySiteAndNumber(Site site, int postNum, bool includeDetails = false);
    Task<IEnumerable<Tag>> TagsFromString(string tagStr);
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
            .Include(p => p.PostedBy)
            .Include(p => p.Tags);
        }

        var post = await query.OrderBy(p => p.ID).SingleOrDefaultAsync();

        if (includeDetails && post is not null)
        {
            await context.Comments.Where(c => c.Post.ID == post.ID).Include(c => c.PostedBy).Include(c => c.Favorites).LoadAsync();
        }

        return post;
    }

    public async Task<IEnumerable<Tag>> TagsFromString(string tagStr)
    {
        var split = tagStr.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var tags = await context.Tags.Where(p => split.Any(s => s == p.Name)).ToListAsync();

        if (tags.Count != split.Length)
        {
            var tagSet = tags.Select(t => t.Name.ToLowerInvariant()).ToHashSet();
            var newTags = split
                .Where(s => !tagSet.Contains(s))
                .Select(s => new Tag() { Name = s })
                .ToArray();

            await context.Tags.AddRangeAsync(newTags);
            await context.SaveChangesAsync();

            return tags.Concat(newTags);
        }


        return tags;
    }
}