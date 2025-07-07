using Data.Models;

namespace App.Models;

public class UserActivityPostModel
{
    public required User User { get; set; }
    public SiteViewModel? Site { get; set; }
    public required PaginatedList<PostModel> Posts { get; set; }
}