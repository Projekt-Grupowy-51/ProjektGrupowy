namespace ProjektGrupowy.Application.Interfaces.SignalR;

public interface IMessageService
{
    Task SendMessageAsync(string userId, string method, string message, CancellationToken cancellationToken = default);

    Task SendSuccessAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendErrorAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendWarningAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendInfoAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendRegularMessageAsync(string userId, string message, CancellationToken cancellationToken = default);

    Task SendToUserAsync(string userId, string method, object data, CancellationToken cancellationToken = default);
    Task SendToUsersAsync(IEnumerable<string> userIds, string method, object data, CancellationToken cancellationToken = default);
}
