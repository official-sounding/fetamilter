using System.Text.RegularExpressions;
using App.IntegrationTests.Fixtures;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit.Abstractions;

namespace App.IntegrationTests;

[Collection(HttpServerCollection.Name)]
public class SmokeTests(HttpServerFixture fixture, ITestOutputHelper outputHelper) : IAsyncLifetime
{
    [Fact]
    public async Task GetIndex()
    {
        // Arrange
        var browserType = BrowserType.Chromium;
        var browserChannel = "chrome";

        var options = new BrowserFixtureOptions
        {
            BrowserType = browserType,
            BrowserChannel = browserChannel
        };

        var browser = new BrowserFixture(options, outputHelper);
        await browser.WithPageAsync(async page =>
        {

            await page.GotoAsync(fixture.ServerAddress);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            Assert.Equal("FetaMilter", await page.InnerTextAsync("h1"));
        });
    }

    public Task InitializeAsync()
    {
        InstallPlaywright();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return Task.CompletedTask;
    }

    private static void InstallPlaywright()
    {
        var exitCode = Microsoft.Playwright.Program.Main(["install"]);

        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Playwright exited with code {exitCode}");
        }
    }
}
