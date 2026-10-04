namespace App.Config;

public class SiteConfig
{
    public const string SECTION = "Site";
    public string? RootDomain { get; set; }
    public bool IncludePort { get; set; }
    public bool UseHttps { get; set; }

    public Uri BuildUri(string subDomain, int port)
    {

        var builder = new UriBuilder() { Scheme = UseHttps ? "https" : "http", Host = $"{subDomain}.{RootDomain}" };
        if (IncludePort)
        {
            builder.Port = port;
        }

        return builder.Uri;
    }
}
