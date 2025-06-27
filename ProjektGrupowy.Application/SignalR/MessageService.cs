
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.Application.SignalR;

public class MessageService(IHubContext<AppHub> hubContext, IConnectedClientManager clientManager) : IMessageService
{
    public async Task SendErrorAsync<T>(string userId, T message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Error, message, cancellationToken);

    public async Task SendInfoAsync<T>(string userId, T message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Info, message, cancellationToken);

    public async Task SendSuccessAsync<T>(string userId, T message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Success, message, cancellationToken);
        
    public async Task SendWarningAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.Warning, message, cancellationToken);

    public async Task SendRegularMessageAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.Message, message, cancellationToken);
    
    public async Task SendMessageAsync<T>(string userId, string method, T message, CancellationToken cancellationToken = default)
    {
        var connections = clientManager.GetConnectionIds(userId);
        var tasks = connections.Select(connectionId =>
            hubContext.Clients.Client(connectionId).SendAsync(method, message, cancellationToken)); // Use SendAsync
        await Task.WhenAll(tasks);
    }

    public async Task SendMessageToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.All.SendAsync(method, message, cancellationToken);
    }
}