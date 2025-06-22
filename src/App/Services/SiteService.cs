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

public class SiteService(IServiceProvider svcs, IOptions<SiteConfig> siteConfig) : ISiteService
{
    private readonly Lock _lockObj = new();
    private ImmutableDictionary<string, Site> _siteBySlug = ImmutableDictionary<string, Site>.Empty;

    private bool _initialized = false;

    public SiteViewModel SiteBySlug(string slug)
    {
        InitializeDictionary();
        if (_siteBySlug.TryGetValue(slug, out var site) || _siteBySlug.TryGetValue("www", out site))
        {
            return SiteViewModel.BuildViewModel(site, siteConfig.Value);
        }

        throw new Exception("Sites Table is not initialized");
    }

    public IEnumerable<SiteViewModel> AllSites()
    {
        InitializeDictionary();
        return _siteBySlug.Values
            .OrderByDescending(s => s.Order)
            .Select(s => SiteViewModel.BuildViewModel(s, siteConfig.Value));
    }

    private void InitializeDictionary()
    {
        if (!_initialized)
        {
            lock (_lockObj)
            {
                if (!_initialized)
                {
                    using var scope = svcs.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var sites = context.Sites.AsNoTracking().ToList();
                    _siteBySlug = sites.ToImmutableDictionary(s => s.Slug);
                    _initialized = true;
                }
            }
        }
    }
}