using Microsoft.AspNetCore.SignalR;
using VidMark.Application.Interfaces.SignalR;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.SignalR;

public class SignalRMessageService(IHubContext<AppHub, IAppHubClient> hubContext) : IMessageService
{
    public async Task SendErrorAsync(string userId, MessageContent content, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.User(userId).NotifyFromBackendServerError(content);

    public async Task SendInfoAsync(string userId, MessageContent content, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.User(userId).NotifyFromBackendServerInfo(content);

    public async Task SendSuccessAsync(string userId, MessageContent content, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.User(userId).NotifyFromBackendServerSuccess(content);

    public async Task SendWarningAsync(string userId, MessageContent content, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.User(userId).NotifyFromBackendServerWarning(content);

    public async Task SendRegularMessageAsync(string userId, MessageContent content, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.User(userId).NotifyFromBackendServerMessage(content);

    public async Task SendVideoProcessedAsync(string userId, MessageContent content, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.User(userId).VideoProcessed(content);

    public async Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default)
    {
        // Map string method names to strongly typed calls
        switch (method)
        {
            case MessageTypes.LabelersCountChanged:
                await hubContext.Clients.User(userId).LabelersCountChanged(data);
                break;
            case MessageTypes.ReportGenerated:
                await hubContext.Clients.User(userId).ReportGenerated(data);
                break;
            default:
                throw new ArgumentException($"Unknown method: {method}", nameof(method));
        }
    }

    public async Task SendToUsersAsync(IEnumerable<string> userIds, string method, object data, CancellationToken cancellationToken = default)
    {
        // Map string method names to strongly typed calls
        switch (method)
        {
            case MessageTypes.LabelersCountChanged:
                await hubContext.Clients.Users(userIds).LabelersCountChanged(data);
                break;
            case MessageTypes.ReportGenerated:
                await hubContext.Clients.Users(userIds).ReportGenerated(data);
                break;
            default:
                throw new ArgumentException($"Unknown method: {method}", nameof(method));
        }
    }
}