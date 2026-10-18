using Data.Models;

namespace App.Models;

public class UserActivityCommentModel
{
    public required User User { get; set; }
    public SiteViewModel? Site { get; set; }
    public required PaginatedList<CommentModel> Comments { get; set; }
}
