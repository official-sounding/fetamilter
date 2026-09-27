using App.IntegrationTests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

using static Microsoft.Playwright.Assertions;

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
            BrowserChannel = browserChannel,
            BaseUrl = fixture.ServerAddress
        };

        var browser = new BrowserFixture(options, outputHelper);
        await browser.WithPageAsync(async page =>
        {
            var index = new IndexPage(page);

            await index.LoadAsync();

            await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "FetaMilter" })).ToBeVisibleAsync();
            await Expect(page.GetByText("Test Post", new() { Exact = true })).ToBeVisibleAsync();

            await page.GetByText("Login").ClickAsync();

            await Expect(page.GetByText("Log In")).ToBeVisibleAsync();
        });
    }

    [Fact]
    public async Task CanPost()
    {
        // Arrange
        var browserType = BrowserType.Chromium;
        var browserChannel = "chrome";

        var options = new BrowserFixtureOptions
        {
            BrowserType = browserType,
            BrowserChannel = browserChannel,
            BaseUrl = fixture.ServerAddress
        };

        var browser = new BrowserFixture(options, outputHelper);
        await browser.WithPageAsync(async page =>
        {
            var index = new IndexPage(page);

            await index.LoadAsync();
            await index.LoginAsync("user1");

            await page.GotoAsync("/create");
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            var title = $"A New Post {Guid.NewGuid()}";

            await page.GetByLabel("Title").FillAsync(title);
            await page.GetByLabel("Body").FillAsync("This is a post created by a test");
            await page.GetByLabel("More Inside").FillAsync("This is below the fold");
            await page.GetByLabel("Tags").FillAsync("these-tags are space-delimited");

            await page.GetByText("Create Post").ClickAsync();

            var titleLoc = page.GetByText(title);

            await Expect(titleLoc).ToBeVisibleAsync();
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
