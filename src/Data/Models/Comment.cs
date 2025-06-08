namespace Data.Models;

public class Comment
{
    public int ID { get; set; }
    public required User PostedBy { get; set; }
    public DateTime PostedOn { get; set; }

    public required Post Post { get; set; }
    public required string Body { get; set; }
    public bool Removed { get; set; }
}