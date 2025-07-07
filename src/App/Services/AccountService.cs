using App.Models;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Net.Mime;

namespace App.Services;

public interface IAccountService
{
    Task<User?> AuthenticateUser(string? username, string? password);
    Task<UserpageModel?> BuildUserpageModel(int userId);
    Task<UserActivityPostModel?> BuildPostActivityModel(int userId, string? slug, int page = 1);
    Task<User?> CreateUser(CreateUserModel model);
    Task<bool> IsUsernameAvailable(string username);
}

public class AccountService(ILogger<AccountService> logger, DataContext context, ISiteService siteService) : IAccountService
{
    public async Task<User?> AuthenticateUser(string? username, string? password)
    {
        var user = await context.Users
        .Where(u => u.UserName.Equals(username))
        .Include(u => u.Role)
        .FirstOrDefaultAsync();

        if (user is null)
        {
            logger.LogDebug("Cannot log in {username}, User not found", username);
            return null;
        }

        if (user.Disabled)
        {
            logger.LogDebug("Cannot log in {username}, account is disabled", username);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            logger.LogDebug("Cannot log in {username}, password mismatch", username);
            return null;
        }
        return user;
    }

    public async Task<UserActivityPostModel?> BuildPostActivityModel(int userId, string? slug, int page = 1)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.ID == userId);
        var site = slug == null ? null : siteService.SiteBySlug(slug);
        if (user is null)
        {
            return null;
        }

        var posts = context.Posts
            .Include(p => p.PostedBy)
            .Include(p => p.Site)
            .Where(p => p.PostedByID == userId && (site == null || p.SiteID == site.ID));

        return new UserActivityPostModel()
        {
            Site = site,
            User = user,
            Posts = await PostModel.BuildPostList(posts, page)
        };
    }

    public async Task<UserpageModel?> BuildUserpageModel(int userId)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.ID == userId);
        if (user is null)
        {
            logger.LogDebug("Unable to load user {userId}", userId);
            return null;
        }

        using var conn = context.Database.GetDbConnection();
        await conn.OpenAsync();

        var counts = (await conn.QueryAsync<UserSiteCount>(@"
        with posts as (
	select p.SiteID as site, COUNT(*) as posts
	from post p
	where p.PostedByID = @userId
	group by p.SiteID
),
 comments as (
	select p.SiteID as site, COUNT(*) as comments
	from comment c
	join post p on c.PostID = p.ID
	where c.PostedByID = @userId
	group by p.SiteID
)
SELECT
	s.Title as site,
    s.Slug as slug,
	ifnull(posts.posts, 0) as posts,
	ifnull(comments.comments, 0) as comments
from Site s
left join posts on posts.site = s.ID
left join comments on comments.site = s.ID
order by s.ID", new { userId })).ToList();
        return new() { User = user, Counts = counts };
    }

    public async Task<User?> CreateUser(CreateUserModel model)
    {
        try
        {
            var userRole = context.Roles.FirstAsync(r => r.Name == Role.UserRoleName);
            var dbModel = new User()
            {
                UserName = model.Username ?? string.Empty,
                EmailAddress = model.Email ?? string.Empty,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedOn = DateTime.UtcNow,
                RoleID = userRole.Id
            };

            await context.Users.AddAsync(dbModel);
            await context.SaveChangesAsync();
            logger.LogInformation("Created New User {username}", model.Username);
            return dbModel;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to create new user");
            return null;
        }
    }
    public async Task<bool> IsUsernameAvailable(string username) => (await context.Users.CountAsync(u => u.UserName == username)) == 0;
}
