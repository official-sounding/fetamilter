using App.Models;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Net.Mime;
using System.Security.Claims;
using App.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace App.Services;

public interface IAccountService
{
    Task<bool> AuthenticateUser(string? username, string? password, bool rememberMe);
    Task<UserpageModel?> BuildUserpageModel(int userId);
    Task<UserActivityPostModel?> BuildPostActivityModel(int userId, string? slug, int page = 1);
    Task<bool> CreateUser(CreateUserModel model);
    Task<bool> IsUsernameAvailable(string username);
}

public class AccountService(ILogger<AccountService> logger, DataContext context, ISiteService siteService) : IAccountService
{
    public async Task<bool> AuthenticateUser(string? username, string? password, bool rememberMe)
    {
        var user = await context.Users
        .Where(u => u.UserName.Equals(username))
        .Include(u => u.Role)
        .FirstOrDefaultAsync();

        if (user is null)
        {
            logger.LogDebug("Cannot log in {username}, User not found", username);
            return false;
        }

        if (user.Disabled)
        {
            logger.LogDebug("Cannot log in {username}, account is disabled", username);
            return false;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            logger.LogDebug("Cannot log in {username}, password mismatch", username);
            return false;
        }

        return await SignUserInAsync(user, rememberMe);
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

    public async Task<bool> CreateUser(CreateUserModel model)
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

            if (dbModel is null)
            {
                return false;
            }

            return await SignUserInAsync(dbModel, true);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to create new user");
            return false;
        }
    }
    public async Task<bool> IsUsernameAvailable(string username) => (await context.Users.CountAsync(u => u.UserName == username)) == 0;

    private async Task<bool> SignUserInAsync(User user, bool rememberMe)
    {
        
        var claims = new List<Claim>
                {
                    new(ClaimTypes.Sid, $"{user.ID}"),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Role, Policy.MakeComment),
                    new(ClaimTypes.Role, Policy.MakePost),
                };

        if (user.Role?.Name == "Moderator")
        {
            claims.AddRange([
                new(ClaimTypes.Role, Policy.DeletePost),
                        new(ClaimTypes.Role, Policy.DeleteComment),
                        new(ClaimTypes.Role, Policy.DisableUser),
                        new(ClaimTypes.Role, Policy.PostOfficially),
                        new(ClaimTypes.Role, Policy.ViewFlags)
            ]);
        }

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            AllowRefresh = true,
            ExpiresUtc = DateTime.UtcNow.AddDays(365)
        };

        if (httpContextAccessor.HttpContext is null)
        {
            return false;
        }

        await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

        logger.LogInformation("User {Email} logged in at {Time}.",
            user.UserName, DateTime.UtcNow);

        return true;
    }
}
