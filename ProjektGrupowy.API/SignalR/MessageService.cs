
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.API.SignalR;

public class MessageService(IHubContext<AppHub> hubContext, IConnectedClientManager clientManager) : IMessageService
{
    public async Task SendMessageAsync<T>(string userId, string method, T message, CancellationToken cancellationToken = default)
    {
        var connections = clientManager.GetConnectionIds(userId);
        var tasks = connections.Select(connectionId =>
            hubContext.Clients.Client(connectionId).SendAsync(method, message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task SendMessageToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.All.SendAsync(method, message, cancellationToken);
    }
}