using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.Application.SignalR;

[Authorize]
public class AppHub(IConnectedClientManager clientManager) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? string.Empty;
        var connectionId = Context.ConnectionId;
        await clientManager.AddClientAsync(userId, connectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await clientManager.RemoveClientAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
