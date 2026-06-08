using System.Collections.Immutable;
using App.Config;
using App.Models;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace App.Services;

public interface ISiteService
{
    IEnumerable<SiteViewModel> AllSites();
    SiteViewModel SiteBySlug(string slug);
}

public class SiteService(ImmutableDictionary<string, Site> siteBySlug, IOptions<SiteConfig> siteConfig) : ISiteService
{
    public SiteViewModel SiteBySlug(string slug)
    {
        if (siteBySlug.TryGetValue(slug, out var site) || siteBySlug.TryGetValue("www", out site))
        {
            return SiteViewModel.BuildViewModel(site, siteConfig.Value);
        }

        throw new InvalidProgramException("Sites Table is not initialized");
    }

    public IEnumerable<SiteViewModel> AllSites()
    {
        return siteBySlug.Values
            .OrderByDescending(s => s.Order)
            .Select(s => SiteViewModel.BuildViewModel(s, siteConfig.Value));
    }

    public static SiteService Initialize(IServiceProvider svcs)
    {
        using var scope = svcs.CreateScope();

        var config = svcs.GetRequiredService<IOptions<SiteConfig>>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var sites = context.Sites.AsNoTracking().ToList();
        var siteBySlug = sites.ToImmutableDictionary(s => s.Slug);
        return new SiteService(siteBySlug, config);
    }
}