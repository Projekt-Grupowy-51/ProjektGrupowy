using VidMark.Domain.Models;

namespace VidMark.Application.Interfaces.SignalR;

public interface IMessageService
{
    Task SendSuccessAsync(string userId, MessageContent content, CancellationToken cancellationToken = default);
    Task SendErrorAsync(string userId, MessageContent content, CancellationToken cancellationToken = default);
    Task SendWarningAsync(string userId, MessageContent content, CancellationToken cancellationToken = default);
    Task SendInfoAsync(string userId, MessageContent content, CancellationToken cancellationToken = default);
    Task SendRegularMessageAsync(string userId, MessageContent content, CancellationToken cancellationToken = default);
    Task SendVideoProcessedAsync(string userId, MessageContent content, CancellationToken cancellationToken = default);

    Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default);
    Task SendToUsersAsync(IEnumerable<string> userIds, string method, object data, CancellationToken cancellationToken = default);
}
