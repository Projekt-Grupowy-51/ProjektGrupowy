using ProjektGrupowy.Application.Interfaces.SignalR;

namespace ProjektGrupowy.IntegrationTests.Infrastructure;

public class TestMessageService : IMessageService
{
    public Task SendErrorAsync(string userId, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendInfoAsync(string userId, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendSuccessAsync(string userId, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendWarningAsync(string userId, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendRegularMessageAsync(string userId, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendVideoProcessedAsync(string userId, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendMessageAsync(string userId, string method, string message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendToUsersAsync(IEnumerable<string> userIds, string method, object data, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
