namespace App.Models;

public partial class HomepageModel
{
    public required PaginatedList<PostModel> Posts { get; init; }
}
