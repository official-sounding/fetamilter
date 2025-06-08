namespace Data.Models;

public class Post
{
    public int ID { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public string? MoreInside { get; set; }

    public required Site Site { get; set; }
    public required User PostedBy { get; set; }
    public DateTime PostedOn { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];

}