namespace App.Config;

public class SiteConfig
{
    public const string SECTION = "Site";
    public string? RootDomain { get; set; }
    public int? Port { get; set; }
    public bool UseHttps { get; set; }

    public Uri BuildUri(string subDomain) {

            var builder = new UriBuilder() { Scheme = UseHttps ? "https" : "http", Host = $"{subDomain}.{RootDomain}" };
            if (Port.HasValue)
            {
                builder.Port = Port.Value;
            }

            return builder.Uri;
    }
}