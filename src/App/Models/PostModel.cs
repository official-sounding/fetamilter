using Data.Models;

namespace App.Models;

public record PostModel(Post Post, int CommentCount, int FavoriteCount);

