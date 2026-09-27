using Microsoft.Playwright;

using static Microsoft.Playwright.Assertions;

namespace App.IntegrationTests;

public class IndexPage(IPage page)
{
    public async Task LoadAsync()
    {
        await page.GotoAsync("/");
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task LoginAsync(string username)
    {
        await page.GetByText("Login").ClickAsync();
        await Expect(page.GetByText("Log In")).ToBeVisibleAsync();

        await page.GetByLabel("Username").FillAsync(username);
        await page.GetByLabel("Password").FillAsync("password");

        await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        await Expect(page.GetByText($"{username} | Logout")).ToBeVisibleAsync();
    }
}
