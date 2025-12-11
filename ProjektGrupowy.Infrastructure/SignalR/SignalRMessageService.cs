using Microsoft.AspNetCore.SignalR;
using ProjektGrupowy.Application.Interfaces.SignalR;

namespace ProjektGrupowy.Infrastructure.SignalR;

public class SignalRMessageService(IHubContext<AppHub> hubContext) : IMessageService
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
    
    public async Task SendVideoProcessedAsync(string userId, string message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.VideoProcessed, message, cancellationToken);

    public async Task SendMessageAsync(string userId, string method, string message, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.User(userId).SendAsync(method, message, cancellationToken);
    }

    public async Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.User(userId).SendAsync(method, data, cancellationToken);
    }

    public async Task SendToUsersAsync(IEnumerable<string> userIds, string method, object data, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Users(userIds).SendAsync(method, data, cancellationToken);
    }
}