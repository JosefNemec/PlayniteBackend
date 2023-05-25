using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
