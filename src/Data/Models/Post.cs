namespace Data.Models;

public class Post
{
    public int ID { get; set; }
    public int Number { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public string? MoreInside { get; set; }

    public Site Site { get; set; } = null!;
    public User PostedBy { get; set; } = null!;
    public int SiteID { get; set; }
    public int PostedByID { get; set; }
    public int? StateUpdatedByID { get; set; }
    public DateTime PostedOn { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<PostFavorite> Favorites { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];

    public string PostedByUsername => PostedBy?.UserName ?? string.Empty;
    public string StateUpdatedByUsername => StateUpdatedBy?.UserName ?? string.Empty;

    public PostState State { get; set; } = PostState.Active;
    public DateTime? StateUpdatedOn { get; set; }
    public User? StateUpdatedBy { get; set; }
    public string? StateMessage { get; set; }

    public bool AppearsInLists => State == PostState.Active || State == PostState.Closed;

    public bool PostIsExpired(TimeProvider timeProvider, int siteOpenDays)
    {
        if (siteOpenDays < 0)
        {
            return false;
        }

        if ((timeProvider.GetUtcNow() - PostedOn).Days > siteOpenDays)
        {
            return true;
        }

        return false;
    }
    public bool PostIsOpen(TimeProvider timeProvider)
    {
        if (State != PostState.Active)
        {
            return false;
        }

        if (Site?.AutoCloseDays is not null && PostIsExpired(timeProvider, Site.AutoCloseDays.Value))
        {
            return false;
        }

        return true;
    }
}

public enum PostState
{
    Active = 0,
    Closed = 1,
    Deleted = 2,
    Redacted = 3,
}