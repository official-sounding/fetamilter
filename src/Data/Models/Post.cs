namespace Data.Models;

public class Post
{
    public int ID { get; set; }
    public int Number { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public string? MoreInside { get; set; }

    public Site? Site { get; set; }
    public User? PostedBy { get; set; }
    public int SiteID { get; set; }
    public int PostedByID { get; set; }
    public DateTime PostedOn { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];

    public string PostedByUsername => PostedBy?.UserName ?? string.Empty;

}