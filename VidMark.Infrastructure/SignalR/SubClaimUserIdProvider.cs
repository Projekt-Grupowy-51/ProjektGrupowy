using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace VidMark.Infrastructure.SignalR;

public class SubClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Use the 'sub' claim from the JWT token as the user identifier
        // This is the standard claim for user ID in Keycloak/OIDC tokens
        var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? connection.User?.FindFirst("sub")?.Value;
        
        return userId;
    }
}
