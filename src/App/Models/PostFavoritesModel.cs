using Data.Models;

namespace App.Models;

public class PostFavoritesModel
{

    public required List<PostFavorite> Favorites { get; set; }
    public int PostNum { get; set; }
}