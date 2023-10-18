using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mealmap.Api.BoundaryTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static readonly string AUTH_SCHEME = "TestScheme";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    { }



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
