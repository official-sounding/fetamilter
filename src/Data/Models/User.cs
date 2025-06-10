namespace Data.Models;


public class User
{
    public int ID { get; set; }
    public required string UserName { get; set; }
    public required string EmailAddress { get; set; }
    public required string PasswordHash { get; set; }
    public bool Disabled { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedOn { get; set; }

    public required Role Role { get; set; }

    public ICollection<Post> Posts { get; set; } = [];
}