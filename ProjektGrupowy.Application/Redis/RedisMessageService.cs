using Newtonsoft.Json;
using ProjektGrupowy.Application.SignalR;
using StackExchange.Redis;

namespace ProjektGrupowy.Application.Redis;

public class RedisMessageService(IConnectionMultiplexer redis) : IMessageService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private const string RedisStreamKey = "notifications";

    private async Task PublishAsync<T>(string userId, string type, string method, T message,
        CancellationToken cancellationToken)
    {
        var payload = JsonConvert.SerializeObject(new
        {
            UserId = userId,
            Type = type,
            Method = method,
            Payload = message
        });

        await _database.StreamAddAsync(RedisStreamKey, [new NameValueEntry("message", payload)]);
    }

    public Task SendMessageAsync<T>(string userId, string method, T message,
        CancellationToken cancellationToken = default) =>
        PublishAsync(userId, "custom", method, message, cancellationToken);

    public Task SendMessageToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default) =>
        PublishAsync("*", "broadcast", method, message, cancellationToken);

    public Task SendSuccessAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        PublishAsync(userId, "success", "ReceiveMessage", message, cancellationToken);

    public Task SendErrorAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        PublishAsync(userId, "error", "ReceiveMessage", message, cancellationToken);

    public Task SendWarningAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        PublishAsync(userId, "warning", "ReceiveMessage", message, cancellationToken);

    public Task SendInfoAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        PublishAsync(userId, "info", "ReceiveMessage", message, cancellationToken);

    public Task SendRegularMessageAsync<T>(string userId, T message, CancellationToken cancellationToken = default) =>
        PublishAsync(userId, "message", "ReceiveMessage", message, cancellationToken);
}