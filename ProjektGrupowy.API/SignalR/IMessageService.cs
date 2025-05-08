namespace ProjektGrupowy.API.SignalR;
public interface IMessageService
{
    Task SendMessageAsync<T>(string userId, string method, T message, CancellationToken cancellationToken = default);
    Task SendMessageToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default);

    Task SendSuccessAsync<T>(string userId, T message, CancellationToken cancellationToken = default);
    Task SendErrorAsync<T>(string userId, T message, CancellationToken cancellationToken = default);
    Task SendWarningAsync<T>(string userId, T message, CancellationToken cancellationToken = default);
    Task SendInfoAsync<T>(string userId, T message, CancellationToken cancellationToken = default);
    Task SendRegularMessageAsync<T>(string userId, T message, CancellationToken cancellationToken = default);
}
