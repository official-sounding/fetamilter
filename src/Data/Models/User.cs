namespace Data.Models;


public class User
{
    public int ID { get; set; }
    public required string DisplayName { get; set; }
    public DateTime CreatedOn { get; set; }

    public ICollection<Post> Posts { get; set; } = [];
}