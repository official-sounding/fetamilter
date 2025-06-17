namespace Data.Models;

public class PostFavorite()
{
    public int ID { get; set; }
    public int PostID { get; set; }
    public int UserID { get; set; }

    public Post? Post { get; set; }
    public User? User { get; set; }
}