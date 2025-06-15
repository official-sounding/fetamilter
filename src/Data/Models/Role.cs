namespace Data.Models;

public class Role
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public string? NameTag { get; set; }

    public static string UserRoleName = "User";
    public static string ModRoleString = "Moderator";
}