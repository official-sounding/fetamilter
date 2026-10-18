using System.Collections.Immutable;
using App.Config;
using App.Services;
using Data.Models;
using Microsoft.Extensions.Options;

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
        var sites = new[] {
            new Site() { ID = 1, Slug = "www", Title = "Web", Order = 1},
            new Site() { ID = 2, Slug = "ask", Title = "Ask", Order = 2}
        };
        var svc = new SiteService(sites, someOptions, 0);

        var site = svc.SiteBySlug(input);

        Assert.Equal(expectedSlug, site.Slug);
    }

    [Fact]
    public void SiteBySlug_ThrowsIfEmpty()
    {
        var someOptions = Options.Create(new SiteConfig() { RootDomain = "example.com" });

        var svc = new SiteService([], someOptions, 0);
        Assert.Throws<InvalidProgramException>(() => svc.SiteBySlug("any"));
    }

    [Fact]
    public void AllSites()
    {
        var allSites = new[] {
            new Site() { ID = 1, Slug = "www", Title = "Web", Order = 1},
            new Site() { ID = 2, Slug = "ask", Title = "Ask", Order = 2}
        };

        var svc = new SiteService(allSites, someOptions, 0);

        var sites = svc.AllSites().ToList();

        Assert.Equal(2, sites.Count);

        Assert.Equal("www", sites[0].Slug);
        Assert.Equal("ask", sites[1].Slug);
    }

    [Theory]
    [InlineData(1, 100, null, "http://www.example.com/100")]
    [InlineData(2, 200, null, "http://ask.example.com/200")]
    [InlineData(1, 300, 123, "http://www.example.com/300#123")]
    public void AbsolutePostUri(int siteId, int postNumber, int? commentId, string expected)
    {
        var post = new Post { SiteID = siteId, Number = postNumber, Title = string.Empty, Body = string.Empty };
        var allSites = new[] {
            new Site() { ID = 1, Slug = "www", Title = "Web", Order = 1},
            new Site() { ID = 2, Slug = "ask", Title = "Ask", Order = 2}
        };

        var svc = new SiteService(allSites, someOptions, 0);
        var result = svc.AbsolutePostUri(post, commentId);

        Assert.Equal(expected, result);
    }

}
