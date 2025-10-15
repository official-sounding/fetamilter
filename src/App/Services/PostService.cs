using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;
using App.Models;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services;

public interface IPostService
{
    IQueryable<PostModel> PostList(SiteViewModel site, Expression<Func<Post, bool>>? filterExp = null);
    IQueryable<PostModel> CrossSitePostList(Expression<Func<Post, bool>>? filterExp = null);
    Task<Post?> PostBySiteAndNumber(SiteViewModel site, int postNum, bool includeDetails = false);
    Task<IEnumerable<Tag>> TagsFromString(string tagStr);
    Task<Post?> CreatePost(CreatePostModel post, SiteViewModel subSite, int userId);
    byte[] CreatePostRSS(Post post, Uri postUri);
    Task AddComment(Post post, CreateCommentModel model, int userId);
}

public class PostService(DataContext context) : IPostService
{
    public IQueryable<PostModel> PostList(SiteViewModel site, Expression<Func<Post, bool>>? filterExp = null)
    {
        var posts = context.Posts.Where(p => p.Site.ID == site.ID && p.State != PostState.Deleted);

        if (filterExp is not null)
        {
            posts = posts.Where(filterExp);
        }

        return posts.Include(p => p.PostedBy).OrderByDescending(p => p.PostedOn).AsNoTracking().Select(p => new PostModel(p, p.Comments.Count(), p.Favorites.Count()));
    }

    public IQueryable<PostModel> CrossSitePostList(Expression<Func<Post, bool>>? filterExp = null)
    {
        IQueryable<Post> posts = context.Posts;

        if (filterExp is not null)
        {
            posts = posts.Where(filterExp);
        }

        return posts.Include(p => p.PostedBy).OrderByDescending(p => p.PostedOn).AsNoTracking().Select(p => new PostModel(p, p.Comments.Count(), p.Favorites.Count()));
    }

    public async Task<Post?> PostBySiteAndNumber(SiteViewModel site, int postNum, bool includeDetails = false)
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

    public async Task<Post?> CreatePost(CreatePostModel post, SiteViewModel subSite, int userId)
    {
        var tags = string.IsNullOrWhiteSpace(post.TagList) ? [] : await TagsFromString(post.TagList);
        var dbModel = new Post()
        {
            Body = post.Body ?? string.Empty,
            Title = post.Title ?? string.Empty,
            MoreInside = post.MoreInside,
            SiteID = subSite.ID,
            PostedByID = userId,
            PostedOn = DateTime.UtcNow,
            Tags = [.. tags]
        };

        context.Posts.Add(dbModel);
        await context.SaveChangesAsync();
        return dbModel;
    }

    public byte[] CreatePostRSS(Post post, Uri postUri)
    {
        var feed = new SyndicationFeed(post.Title, $"Comments on Post {post.Number}", postUri, "RSSUrl", DateTime.Now);

        var items = new List<SyndicationItem>();
        foreach (var item in post.Comments)
        {
            var commentUri = new UriBuilder(postUri)
            {
                Fragment = $"{item.ID}"
            };
            var title = $"By {item.PostedBy.UserName}";
            var description = item.Body;
            items.Add(new SyndicationItem(title, description, commentUri.Uri, $"{post.Site.Slug}-comment-{item.ID}", item.PostedOn));
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

        return stream.ToArray();
    }

    public async Task AddComment(Post post, CreateCommentModel model, int userId)
    {
        var comment = new Comment()
        {
            Body = HttpUtility.HtmlEncode(model.Body?.Trim()) ?? string.Empty,
            PostedOn = DateTime.UtcNow,
            PostID = post.ID,
            PostedByID = userId
        };

        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }
}