using Data.Models;

namespace App.Models;

public class HeaderModel()
{
    public required SiteViewModel[] AllSites { get; set; }
    public required SiteViewModel Site { get; set; }
    public bool IsLoggedIn { get; set; }
    public string? Username { get; set; }
}