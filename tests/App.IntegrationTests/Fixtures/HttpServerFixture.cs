using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace App.IntegrationTests.Fixtures;
/// <summary>
/// A test server fixture that hosts the application on an HTTPS port so
/// that the application can be accessed through a browser for UI tests.
/// </summary>
public sealed class HttpServerFixture : AppFixture
{
    public HttpServerFixture()
    {
        // Configure the address for the server to listen on for HTTPS
        // requests on a dynamic port with a self-signed TLS certificate.
        UseKestrel((s) => s.Listen(IPAddress.Loopback, 0));
        // UseKestrel(
        //     (server) => server.Listen(
        //         IPAddress.Loopback, 0, (listener) => listener.UseHttps(
        //             (https) => https.ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile("localhost-dev.pfx", "Pa55w0rd!"))));
    }

    public string ServerAddress
    {
        get
        {
            StartServer();
            return $"http://fetamilter.localhost:{ClientOptions.BaseAddress.Port}";
        }
    }
}
