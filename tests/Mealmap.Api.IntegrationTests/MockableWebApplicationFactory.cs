using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Mealmap.Api.IntegrationTests;

public class MockableWebApplicationFactory(Action<IServiceCollection>? serviceReplacements)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            serviceReplacements?.Invoke(services);
            services
                .AddAuthentication(defaultScheme: TestAuthHandler.AUTH_SCHEME)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AUTH_SCHEME, options => { });
        });
    }
}
