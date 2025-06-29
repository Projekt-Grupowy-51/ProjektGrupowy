using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Authentication;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // In development, create a fake user for testing
        var claims = new[]
        {
            new Claim("sub", "test-user-id"),
            new Claim("preferred_username", "testuser"),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("name", "Test User"),
            new Claim("email", "test@example.com"),
            new Claim("realm_access", """{"roles":["scientist","admin"]}"""),
            new Claim(ClaimTypes.Role, RoleConstants.Scientist),
            new Claim(ClaimTypes.Role, RoleConstants.Admin)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
