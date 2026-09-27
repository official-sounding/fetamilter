using System.Globalization;

namespace App.IntegrationTests.Fixtures;

public class BrowserFixtureOptions
{
    public string BrowserType { get; set; } = Microsoft.Playwright.BrowserType.Chromium;

    public string? BrowserChannel { get; set; }

    // Only record traces and videos in CI to prevent filling
    // up the local disk with videos from test runs.

    public bool CaptureTrace { get; set; } = BrowserFixture.IsRunningInGitHubActions;

    public bool CaptureVideo { get; set; } = BrowserFixture.IsRunningInGitHubActions;

    public string? TestName { get; set; }

    public string? Build { get; set; }

    public string? OperatingSystem { get; set; }

    public string? OperatingSystemVersion { get; set; }

    public string? PlaywrightVersion { get; set; }

    public string? ProjectName { get; set; }

    public bool UseBrowserStack { get; set; }

    public bool UseBrowserStackLocal { get; set; }

    public (string UserName, string AccessKey) BrowserStackCredentials { get; set; }

    public BrowserStackLocalOptions? BrowserStackLocalOptions { get; set; }

    public Uri BrowserStackEndpoint { get; set; } = new("wss://cdp.browserstack.com/playwright", UriKind.Absolute);
}

public sealed class BrowserStackLocalOptions
{
    //// See https://www.browserstack.com/local-testing/binary-params

    public string? LocalIdentifier { get; set; }

    public string? ProxyHostName { get; set; }

    public string? ProxyPassword { get; set; }

    public int? ProxyPort { get; set; }

    public string? ProxyUserName { get; set; }

    internal static IList<string> BuildCommandLine(string apiKey, BrowserStackLocalOptions? options)
    {
        ArgumentNullException.ThrowIfNull(apiKey);

        List<string> arguments =
        [
            "--key",
            apiKey,
            "--only-automate"
        ];

        if (!string.IsNullOrWhiteSpace(options?.LocalIdentifier))
        {
            arguments.Add("--local-identifier");
            arguments.Add(options.LocalIdentifier);
        }

        if (!string.IsNullOrWhiteSpace(options?.ProxyHostName))
        {
            if (!options.ProxyPort.HasValue)
            {
                throw new ArgumentException("No proxy port number specified.", nameof(options));
            }

            arguments.Add("--proxy-host");
            arguments.Add(options.ProxyHostName);
            arguments.Add("--proxy-port");
            arguments.Add(options.ProxyPort.Value.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(options.ProxyUserName))
            {
                if (string.IsNullOrWhiteSpace(options.ProxyPassword))
                {
                    throw new ArgumentException("No proxy password specified.", nameof(options));
                }

                arguments.Add("--proxy-user");
                arguments.Add(options.ProxyUserName);
                arguments.Add("--proxy-pass");
                arguments.Add(options.ProxyPassword);
            }
        }

        return arguments;
    }
}
