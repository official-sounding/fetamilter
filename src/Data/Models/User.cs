namespace Data.Models;


public class User
{
    public int ID { get; set; }
    public required string UserName { get; set; }
    public required string EmailAddress { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedOn { get; set; }

    public ICollection<Post> Posts { get; set; } = [];
}