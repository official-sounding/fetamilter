namespace App.Models;

public class PostListModel
{
    public required PaginatedList<PostModel> Posts { get; init; }
}
