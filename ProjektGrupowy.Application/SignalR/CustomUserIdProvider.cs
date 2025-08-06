using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.Application.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var user = connection.User;
        return user.FindFirst("sub")?.Value
               ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}