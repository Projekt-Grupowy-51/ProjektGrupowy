
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.Application.SignalR;

public class SignalRMessageService(IHubContext<AppHub> hubContext, IConnectedClientManager clientManager) : IMessageService
{
    public async Task SendErrorAsync(string userId, string message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Error, message, cancellationToken);

    public async Task SendInfoAsync(string userId, string message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Info, message, cancellationToken);

    public async Task SendSuccessAsync(string userId, string message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Success, message, cancellationToken);
        
    public async Task SendWarningAsync(string userId, string message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.Warning, message, cancellationToken);

    public async Task SendRegularMessageAsync(string userId, string message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.Message, message, cancellationToken);
    
    public async Task SendMessageAsync(string userId, string method, string message, CancellationToken cancellationToken = default)
    {
        var connections = clientManager.GetConnectionIds(userId);
        var tasks = connections.Select(connectionId =>
            hubContext.Clients.Client(connectionId).SendAsync(method, message, cancellationToken)); // Use SendAsync
        await Task.WhenAll(tasks);
    }
}