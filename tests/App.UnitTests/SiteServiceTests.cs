using System.Collections.Immutable;
using Data.Models;
using Microsoft.Extensions.Options;
using App.Config;
using App.Services;

namespace App.UnitTests;

public class SiteServiceTests
{
    private IOptions<SiteConfig> someOptions = Options.Create(new SiteConfig() { RootDomain = "example.com" });

    [Theory]
    [InlineData("ask", "ask")]
    [InlineData("www", "www")]
    [InlineData("dne", "www")]
    public void SiteBySlug(string input, string expectedSlug)
    {
        var dict = new [] {
            new Site() { ID = 1, Slug = "www", Title = "Web", Order = 1}, 
            new Site() { ID = 2, Slug = "ask", Title = "Ask", Order = 2}
        }.ToImmutableDictionary(s => s.Slug);

        var svc = new SiteService(dict, someOptions);

        var site = svc.SiteBySlug(input);

        Assert.Equal(expectedSlug, site.Slug);
    }

    [Fact]
    public void SiteBySlug_ThrowsIfEmpty()
    {
        var someOptions = Options.Create(new SiteConfig() { RootDomain = "example.com" });

        var svc = new SiteService([], someOptions);
        Assert.Throws<InvalidProgramException>(() => svc.SiteBySlug("any"));
    }

    [Fact]
    public void AllSites()
    {
        var dict = new [] {
            new Site() { ID = 1, Slug = "www", Title = "Web", Order = 1}, 
            new Site() { ID = 2, Slug = "ask", Title = "Ask", Order = 2}
        }.ToImmutableDictionary(s => s.Slug);

        var svc = new SiteService(dict, someOptions);

        var sites = svc.AllSites().ToList();

        Assert.Equal(2, sites.Count);

        Assert.Equal("www", sites[0].Slug);
        Assert.Equal("ask", sites[1].Slug);
    }
}
