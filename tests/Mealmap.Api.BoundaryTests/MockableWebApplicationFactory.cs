﻿using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Mealmap.Api.BoundaryTests;

public class MockableWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _serviceReplacements;

    public MockableWebApplicationFactory(Action<IServiceCollection> serviceReplacements)
    {
        _serviceReplacements = serviceReplacements;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            _serviceReplacements?.Invoke(services);
            services
                .AddAuthentication(defaultScheme: TestAuthHandler.AUTH_SCHEME)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AUTH_SCHEME, options => { });
        });
    }

    public HttpClient CreateAuthorizedClient()
    {
        var client = CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AUTH_SCHEME);

        return client;
    }
}
