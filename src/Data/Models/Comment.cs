namespace Data.Models;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.