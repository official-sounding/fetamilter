namespace Data.Models;

public class PostFavorite()
{
    public int ID { get; set; }
    public int PostID { get; set; }
    public int UserID { get; set; }
    public DateTime FavoritedOn { get; set; }


    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
}