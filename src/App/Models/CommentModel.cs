using Data.Models;

namespace App.Models;

public record CommentModel(Comment Comment, int FavoriteCount);
