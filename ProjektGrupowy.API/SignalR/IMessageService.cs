namespace ProjektGrupowy.API.SignalR;
public interface IMessageService
{
    Task SendMessageAsync<T>(string userId, string method, T message, CancellationToken cancellationToken = default);
    Task SendMessageToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default);
}
