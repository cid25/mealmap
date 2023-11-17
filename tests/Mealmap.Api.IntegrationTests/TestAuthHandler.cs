using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mealmap.Api.IntegrationTests;

public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public static readonly string AUTH_SCHEME = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Test user"), new Claim("scp", "access") };
        var identity = new ClaimsIdentity(claims, "JWT");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AUTH_SCHEME);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
