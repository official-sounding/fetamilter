using App.Models;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace App.Services;

public interface IAccountService
{
    Task<User?> AuthenticateUser(string? username, string? password);
    Task<UserpageModel?> BuildUserpageModel(int userId);
}

public class AccountService(ILogger<AccountService> logger, DataContext context) : IAccountService
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
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            logger.LogDebug("Cannot log in {username}, password mismatch", username);
            return null;
        }
        return user;
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

        var counts = (await conn.QueryAsync<UserSiteCount>(@"select MAX(s.Title) as site, COUNT(*) as posts, 0 as comments
from post p
join site s on p.SiteID = s.ID
where PostedByID = @userId
group by s.ID", new { userId })).ToList();
        return new() { User = user, Counts = counts };
    }
}


public class Either<T, Err>
{
    private readonly T _left;
    private readonly Err _right;
    private readonly bool _isLeft;

    private Either(T left, Err right, bool isLeft)
    {
        _left = left;
        _right = right;
        _isLeft = isLeft;
    }

    public static Either<T, Err> FromLeft(T left) => new(left, default!, true);
    public static Either<T, Err> FromRight(Err right) => new(default!, right, false);

    public R HandleResult<R>(Func<T, R> forLeft, Func<Err, R> forRight)
    {
        if (_isLeft)
        {
            return forLeft(_left);
        }
        else
        {
            return forRight(_right);
        }
    }
}