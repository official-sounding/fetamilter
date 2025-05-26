using Data.Models;

namespace App.Models;

public class UserpageModel
{
    public required User User { get; set; }
    public required List<UserSiteCount> Counts { get; set; }
}

public record UserSiteCount(string site, long posts, long comments);