namespace Data.Models;

public class CommentFavorite()
{
    public int ID { get; set; }
    public int CommentID { get; set; }
    public int UserID { get; set; }
    public DateTime FavoritedOn { get; set; }

    public Comment? Comment { get; set; }
    public User? User { get; set; }
}