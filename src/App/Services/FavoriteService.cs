using App.Models;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services;

public interface IFavoriteService
{
    Task<FavoriteDetailModel> GetFavorites(FavoriteType entityType, int entityId);
    Task<FavoriteModel> AddFavorite(FavoriteType entityType, int entityId, int userId);
    Task<FavoriteModel> RemoveFavorite(FavoriteType entityType, int entityId, int userId);
}

public interface IFavoriteFacade
{
    FavoriteType EntityType { get; }

    Task<FavoriteDetailModel> Get(int entityId);
    Task<FavoriteModel> Add(int entityId, int userId);
    Task<FavoriteModel> Remove(int entityId, int userId);
}

public class FavroiteService(IEnumerable<IFavoriteFacade> facades) : IFavoriteService
{
    Dictionary<FavoriteType, IFavoriteFacade> facadeDict = facades.ToDictionary(f => f.EntityType);

    public Task<FavoriteDetailModel> GetFavorites(FavoriteType entityType, int entityId) => WithFacade(entityType).Get(entityId);
    public Task<FavoriteModel> AddFavorite(FavoriteType entityType, int entityId, int userId) => WithFacade(entityType).Add(entityId, userId);
    public Task<FavoriteModel> RemoveFavorite(FavoriteType entityType, int entityId, int userId) => WithFacade(entityType).Remove(entityId, userId);

    private IFavoriteFacade WithFacade(FavoriteType entityType) => facadeDict.GetValueOrDefault(entityType) ?? throw new ArgumentException($"no favorite handler for {entityType} found");
}


public class PostFavoriteFacade(DataContext context, TimeProvider timeProvider) : IFavoriteFacade
{
    public FavoriteType EntityType => FavoriteType.Post;

    public async Task<FavoriteDetailModel> Get(int entityId)
    {
        var favorites = await context.PostFavorites.Where(p => p.PostID == entityId)
            //.Include(pf => pf.User)
            .Select(pf => new FavoriteDetail(pf.UserID, pf.User!.UserName, pf.FavoritedOn))
            .ToListAsync();

        return new FavoriteDetailModel() { Favorites = favorites };
    }

    public async Task<FavoriteModel> Add(int entityId, int userId)
    {
        var successful = false;
        if ((await context.PostFavorites.CountAsync(pf => pf.PostID == entityId && pf.UserID == userId)) == 0)
        {
            context.PostFavorites.Add(new PostFavorite() { PostID = entityId, UserID = userId, FavoritedOn = timeProvider.GetUtcNow().DateTime });
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.PostFavorites.CountAsync(pf => pf.PostID == entityId);
        return new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful };
    }

    public async Task<FavoriteModel> Remove(int entityId, int userId)
    {
        var pf = await context.PostFavorites.Where(pf => pf.PostID == entityId && pf.UserID == userId).SingleOrDefaultAsync();

        var successful = false;
        if (pf != null)
        {
            context.PostFavorites.Remove(pf);
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.PostFavorites.CountAsync(pf => pf.PostID == entityId);
        return new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful };
    }
}

public class CommentFavoriteFacade(DataContext context, TimeProvider timeProvider) : IFavoriteFacade
{
    public FavoriteType EntityType => FavoriteType.Comment;

    public async Task<FavoriteDetailModel> Get(int entityId)
    {
        var favorites = await context.CommentFavorites.Where(p => p.CommentID == entityId)
            //.Include(pf => pf.User)
            .Select(pf => new FavoriteDetail(pf.UserID, pf.User!.UserName, pf.FavoritedOn))
            .ToListAsync();

        return new FavoriteDetailModel() { Favorites = favorites };
    }

    public async Task<FavoriteModel> Add(int entityId, int userId)
    {
        var successful = false;
        if ((await context.CommentFavorites.CountAsync(pf => pf.CommentID == entityId && pf.UserID == userId)) == 0)
        {
            context.CommentFavorites.Add(new CommentFavorite() { CommentID = entityId, UserID = userId, FavoritedOn = timeProvider.GetUtcNow().DateTime });
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.CommentFavorites.CountAsync(pf => pf.CommentID == entityId);
        return new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful };
    }

    public async Task<FavoriteModel> Remove(int entityId, int userId)
    {
        var pf = await context.CommentFavorites.Where(pf => pf.CommentID == entityId && pf.UserID == userId).SingleOrDefaultAsync();

        var successful = false;
        if (pf != null)
        {
            context.CommentFavorites.Remove(pf);
            await context.SaveChangesAsync();
            successful = true;
        }

        var currentCount = await context.CommentFavorites.CountAsync(pf => pf.CommentID == entityId);
        return new FavoriteModel() { CurrentCount = currentCount, ActionSuccessful = successful };
    }
}

public enum FavoriteType
{
    Post = 1,
    Comment = 2
}