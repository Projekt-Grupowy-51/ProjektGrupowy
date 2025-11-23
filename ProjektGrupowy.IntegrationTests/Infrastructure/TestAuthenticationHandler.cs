using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ProjektGrupowy.IntegrationTests.Infrastructure;

public class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string[]? Roles { get; set; }
}

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public const string SchemeName = "TestScheme";
    public const string UserIdHeaderName = "X-Test-UserId";
    public const string UserNameHeaderName = "X-Test-UserName";
    public const string RolesHeaderName = "X-Test-Roles";

    public TestAuthenticationHandler(
        IOptionsMonitor<TestAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if authentication headers are present
        if (!Context.Request.Headers.ContainsKey(UserIdHeaderName))
        {
            // No authentication headers - return no result (allows anonymous access)
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = Context.Request.Headers[UserIdHeaderName].ToString();
        var userName = Context.Request.Headers.ContainsKey(UserNameHeaderName)
            ? Context.Request.Headers[UserNameHeaderName].ToString()
            : "test-user";

        var rolesHeader = Context.Request.Headers.ContainsKey(RolesHeaderName)
            ? Context.Request.Headers[RolesHeaderName].ToString()
            : string.Empty;

        var roles = string.IsNullOrEmpty(rolesHeader)
            ? Array.Empty<string>()
            : rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
            new("sub", userId),
            new("preferred_username", userName)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

        // Add realm_access claim for Keycloak compatibility
        if (roles.Length > 0)
        {
            var realmAccess = new
            {
                roles = roles.Select(r => r.Trim()).ToArray()
            };
            claims.Add(new Claim("realm_access", JsonSerializer.Serialize(realmAccess)));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
