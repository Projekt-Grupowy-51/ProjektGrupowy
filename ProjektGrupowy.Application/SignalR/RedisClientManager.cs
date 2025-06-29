using StackExchange.Redis;

namespace ProjektGrupowy.Application.SignalR;

public class RedisClientManager(IConnectionMultiplexer redis) : IConnectedClientManager
{
    private readonly IDatabase _database = redis.GetDatabase();
    private const string OnlineUsersKey = "online_users";

    public async Task AddClientAsync(string userId, string connectionId)
    {
        var userKey = $"user:{userId}";
        var connectionKey = $"connection:{connectionId}";

        var batch = _database.CreateBatch();

        var addToUserSetTask = batch.SetAddAsync(userKey, connectionId);
        var setConnectionMappingTask = batch.StringSetAsync(connectionKey, userId);
        var addToOnlineUsersTask = batch.SetAddAsync(OnlineUsersKey, userId);

        batch.Execute();

        await Task.WhenAll(addToUserSetTask, setConnectionMappingTask, addToOnlineUsersTask);
    }

    public async Task RemoveClientAsync(string connectionId)
    {
        var connectionKey = $"connection:{connectionId}";
        var userId = await _database.StringGetAsync(connectionKey);

        if (userId.IsNullOrEmpty)
            return;

        var userKey = $"user:{userId}";

        await _database.SetRemoveAsync(userKey, connectionId);
        await _database.KeyDeleteAsync(connectionKey);

        if (await _database.SetLengthAsync(userKey) == 0)
        {
            await _database.KeyDeleteAsync(userKey);
            await _database.SetRemoveAsync(OnlineUsersKey, userId);
        }
    }

    public async Task<IReadOnlyList<string>> GetConnectionIdsAsync(string userId)
    {
        string userKey = $"user:{userId}";
        var members = await _database.SetMembersAsync(userKey);
        return members.Select(x => x.ToString()).ToList();
    }

    public async Task<IEnumerable<string>> GetOnlineUsersAsync()
    {
        var members = await _database.SetMembersAsync(OnlineUsersKey);
        return members.Select(x => x.ToString());
    }
}