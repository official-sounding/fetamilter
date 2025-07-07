using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Models;

public record PostModel(Post Post, int CommentCount, int FavoriteCount)
{
    public static async Task<PaginatedList<PostModel>> BuildPostList(IQueryable<Post> posts, int pageIndex, int pageSize = 50)
    {
        var query = posts
        .Include(p => p.PostedBy)
        .OrderByDescending(p => p.PostedOn)
        .AsNoTracking()
        .Select(p => new PostModel(p, p.Comments.Count, p.Favorites.Count));
        return await PaginatedList<PostModel>.CreateAsync(query, pageIndex, pageSize);
    }
}

