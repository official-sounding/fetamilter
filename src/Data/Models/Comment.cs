namespace Data.Models;

public class Comment
{
    public int ID { get; set; }
    public int PostedByID { get; set; }
    public int? RemovedByID { get; set; }

    public User PostedBy { get; set; } = null!;
    public DateTime PostedOn { get; set; }

    public int PostID { get; set; }
    public Post Post { get; set; } = null!;
    public required string Body { get; set; }
    public bool Removed { get; set; }
    public User? RemovedBy { get; set; }
    public DateTime? RemovedOn { get; set; }
    public string? RemovedNote { get; set; }

    public ICollection<CommentFavorite> Favorites { get; set; } = [];
}