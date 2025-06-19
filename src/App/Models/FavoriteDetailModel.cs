using Data.Models;

namespace App.Models;

public class FavoriteDetailModel
{

    public required List<FavoriteDetail> Favorites { get; set; }
    public int PostNum { get; set; }
    public int? CommentId { get; set; }
}

public record FavoriteDetail(int UserID, string Username, DateTime FavoritedOn);