using App.IntegrationTests.Fixtures;

namespace App.IntegrationTests;

[CollectionDefinition(Name)]
public class HttpServerCollection : ICollectionFixture<HttpServerFixture>
{
    public const string Name = "App HTTP server collection";
}
