using StackExchange.Redis;

namespace ProjektGrupowy.Application.SignalR;

public class RedisClientManager(IConnectionMultiplexer redis) : IConnectedClientManager
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string OnlineUsersKey = "online_users";

    public async Task AddClientAsync(string userId, string connectionId)
    {
        var userKey = $"user:{userId}";
        var connectionKey = $"connection:{connectionId}";

        var batch = _db.CreateBatch();
        await batch.SetAddAsync(userKey, connectionId);
        await batch.StringSetAsync(connectionKey, userId);
        await batch.SetAddAsync(OnlineUsersKey, userId);
        batch.Execute();
        await Task.CompletedTask;
    }

    public async Task RemoveClientAsync(string connectionId)
    {
        var connectionKey = $"connection:{connectionId}";
        var userId = await _db.StringGetAsync(connectionKey);

        if (userId.IsNullOrEmpty)
            return;

        string userKey = $"user:{userId}";

        await _db.SetRemoveAsync(userKey, connectionId);
        await _db.KeyDeleteAsync(connectionKey);

        if (await _db.SetLengthAsync(userKey) == 0)
        {
            await _db.KeyDeleteAsync(userKey);
            await _db.SetRemoveAsync(OnlineUsersKey, userId);
        }
    }

    public async Task<IReadOnlyList<string>> GetConnectionIdsAsync(string userId)
    {
        string userKey = $"user:{userId}";
        var members = await _db.SetMembersAsync(userKey);
        return members.Select(x => x.ToString()).ToList();
    }

    public async Task<IEnumerable<string>> GetOnlineUsersAsync()
    {
        var members = await _db.SetMembersAsync(OnlineUsersKey);
        return members.Select(x => x.ToString());
    }
}
