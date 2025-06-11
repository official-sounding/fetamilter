namespace Data.Models;

public class Comment
{
    public int ID { get; set; }
    public int PostedByID { get; set; }
    public User PostedBy { get; set; }
    public DateTime PostedOn { get; set; }

    public int PostID { get; set; }
    public Post Post { get; set; }
    public required string Body { get; set; }
    public bool Removed { get; set; }
}