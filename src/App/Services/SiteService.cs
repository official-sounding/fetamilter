using System.Collections.Immutable;
using App.Config;
using App.Models;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace App.Services;

public interface ISiteService
{
    IEnumerable<SiteViewModel> AllSites();
    SiteViewModel SiteBySlug(string slug);

    string AbsolutePostUri(Post post, int? commentId = null);
}

public class SiteService(IEnumerable<Site> sites, IOptions<SiteConfig> siteConfig, int port) : ISiteService
{
    private readonly ImmutableDictionary<string, Site> _siteBySlug = sites.ToImmutableDictionary(s => s.Slug);
    private readonly ImmutableDictionary<int, Site> _siteById = sites.ToImmutableDictionary(s => s.ID);

    public SiteViewModel SiteBySlug(string slug)
    {
        if (_siteBySlug.TryGetValue(slug, out var site) || _siteBySlug.TryGetValue("www", out site))
        {
            return SiteViewModel.BuildViewModel(site, siteConfig.Value, port);
        }

        throw new InvalidProgramException("Sites Table is not initialized");
    }

    public IEnumerable<SiteViewModel> AllSites()
    {
        return _siteBySlug.Values
            .OrderBy(s => s.Order)
            .Select(s => SiteViewModel.BuildViewModel(s, siteConfig.Value, port));
    }

    public static SiteService Initialize(IServiceProvider svcs)
    {
        using var scope = svcs.CreateScope();

        var config = svcs.GetRequiredService<IOptions<SiteConfig>>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var server = svcs.GetRequiredService<IServer>();
        var addressFeature = server.Features.Get<IServerAddressesFeature>();

        var address = addressFeature?.Addresses.LastOrDefault();
        var port = address is null ? 0 : new Uri(address).Port;

        var sites = context.Sites.AsNoTracking().ToList();
        return new SiteService(sites, config, port);
    }

    public string AbsolutePostUri(Post post, int? commentId = null)
    {
        if(!_siteById.TryGetValue(post.SiteID, out var site))
        {
            throw new ArgumentException("Site does not exist");
        }

        return AbsolutePostUri(site, post.Number, commentId);
    }

    private string AbsolutePostUri(Site site, int postNumber, int? commentId)
    {
        var uri = new UriBuilder(siteConfig.Value.BuildUri(site.Slug, port))
        {
            Path = $"/{postNumber}"
        };

        if (commentId is not null)
        {
            uri.Fragment = $"{commentId}";
        }

        return uri.Uri.ToString();
    }
}
