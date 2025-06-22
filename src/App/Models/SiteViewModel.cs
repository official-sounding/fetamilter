using App.Config;
using Data.Models;

namespace App.Models;

public class SiteViewModel
{
    public int ID { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public string? Tagline { get; set; }
    public required string Url { get; set; }
    public required string ThemeCssPath { get; set; }

    public static SiteViewModel BuildViewModel(Site dbModel, SiteConfig config)
    {
        var scheme = config.UseHttps ? "https" : "http";
        return new SiteViewModel()
        {
            ID = dbModel.ID,
            Title = dbModel.Title,
            Tagline = dbModel.Tagline,
            Slug = dbModel.Slug,
            Url = $"{scheme}://{dbModel.Slug}.{config.RootDomain}/",
            ThemeCssPath = $"/css/themes/{dbModel.Slug}",
        };
    }
}