using System.Collections.Immutable;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services;

public interface ISiteService
{
    Site SiteBySlug(string slug);
}

public class SiteService(IServiceProvider svcs) : ISiteService
{
    private readonly Lock _lockObj = new();
    private ImmutableDictionary<string, Site> _siteBySlug = ImmutableDictionary<string, Site>.Empty;

    private bool _initialized = false;

    public Site SiteBySlug(string slug)
    {
        if (!_initialized)
        {
            lock (_lockObj)
            {
                if (!_initialized)
                {
                    InitializeDictionary();
                }
            }
        }

        if (_siteBySlug.TryGetValue(slug, out var site) || _siteBySlug.TryGetValue("www", out site))
        {
            return site;
        }

        throw new Exception("Sites Table is not initialized");
    }

    private void InitializeDictionary()
    {
        using var scope = svcs.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var sites = context.Sites.AsNoTracking().ToList();
        _siteBySlug = sites.ToDictionary(s => s.Slug).ToImmutableDictionary();
        _initialized = true;
    }
}