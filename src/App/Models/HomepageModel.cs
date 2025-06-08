using Data.Models;

namespace App.Models;

public class HomepageModel
{
    public required Site Site { get; init; }
    public required PaginatedList<PostModel> Posts { get; init; }

    public record PostModel(Post Post, int CommentCount, int FavoriteCount);
}
