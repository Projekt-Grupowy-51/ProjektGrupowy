using VidMark.Application.Interfaces.SignalR;
using VidMark.Domain.Models;

namespace VidMark.IntegrationTests.Infrastructure;

public class TestMessageService : IMessageService
{
    public Task SendErrorAsync(string userId, MessageContent content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendInfoAsync(string userId, MessageContent content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendSuccessAsync(string userId, MessageContent content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendWarningAsync(string userId, MessageContent content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendRegularMessageAsync(string userId, MessageContent content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendVideoProcessedAsync(string userId, MessageContent content, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendToUsersAsync(IEnumerable<string> userIds, string method, object data, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
