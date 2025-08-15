namespace ProjektGrupowy.Application.SignalR;
public interface IMessageService
{
    Task SendMessageAsync(string userId, string method, string message, CancellationToken cancellationToken = default);

    Task SendSuccessAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendErrorAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendWarningAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendInfoAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task SendRegularMessageAsync(string userId, string message, CancellationToken cancellationToken = default);
}
