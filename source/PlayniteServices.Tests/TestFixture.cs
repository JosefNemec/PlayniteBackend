using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PlayniteServices.Tests;

[CollectionDefinition("DefaultCollection")]
public class DefaultTestCollection : ICollectionFixture<TestFixture>
{
}

public class TestFixture : IDisposable
{
    private readonly WebApplicationFactory<Program> app;
    public HttpClient Client { get; }

    public TestFixture()
    {
        app = new WebApplicationFactory<Program>();
        Client = app.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
        app.Dispose();
    }
}
