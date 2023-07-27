using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Mealmap.Api.NarrowIntegrationTests;

public class MockableWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _serviceReplacements;

    public MockableWebApplicationFactory(Action<IServiceCollection> serviceReplacements)
    {
        _serviceReplacements = serviceReplacements;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => _serviceReplacements?.Invoke(services));
    }
}
