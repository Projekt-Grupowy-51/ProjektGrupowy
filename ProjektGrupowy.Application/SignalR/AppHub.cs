using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.Application.SignalR;

[Authorize]
public class AppHub(IConnectedClientManager clientManager) : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Console.WriteLine($"\n\nNew connection: {Context.ConnectionId}, User: {Context.UserIdentifier}\n\n");
        
        var userId = Context.UserIdentifier ?? string.Empty;
        var connectionId = Context.ConnectionId; 
        clientManager.AddClient(userId, connectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    { 
        clientManager.RemoveClient(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
