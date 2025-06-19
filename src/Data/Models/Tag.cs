namespace Data.Models;

public class Tag
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public ICollection<Post> Posts { get; set; } = [];
}