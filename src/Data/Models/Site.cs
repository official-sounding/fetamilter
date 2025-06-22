namespace Data.Models;

public class Site
{
    public int ID { get; set; }
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public int Order { get; set; }
    public string? Tagline { get; set; }
}