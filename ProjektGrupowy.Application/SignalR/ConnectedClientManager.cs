using StackExchange.Redis;

namespace ProjektGrupowy.Application.SignalR;

public class ConnectedClientManager(IConnectionMultiplexer redis) : IConnectedClientManager
{
    private readonly IDatabase _database = redis.GetDatabase();
    private const string OnlineUsersKey = "online_users";
    private static readonly TimeSpan SlidingTtl = TimeSpan.FromSeconds(60);

    public async Task AddClientAsync(string userId, string connectionId)
    {
        var userKey = $"user:{userId}";
        var connectionKey = $"connection:{connectionId}";

        var transaction = _database.CreateTransaction();

        var addToUserSetTask = transaction.SetAddAsync(userKey, connectionId);
        var setConnectionMappingTask = transaction.StringSetAsync(connectionKey, userId);
        var addToOnlineUsersTask = transaction.SetAddAsync(OnlineUsersKey, userId);

        var expireUserSetTask = transaction.KeyExpireAsync(userKey, SlidingTtl);
        var expireConnectionTask = transaction.KeyExpireAsync(connectionKey, SlidingTtl);
        var expireOnlineUsersTask = transaction.KeyExpireAsync(OnlineUsersKey, SlidingTtl);

        bool committed = await transaction.ExecuteAsync();

        if (!committed)
        {
            throw new Exception("Failed to add client to Redis transactionally.");
        }

        await Task.WhenAll(addToUserSetTask, setConnectionMappingTask, addToOnlineUsersTask,
            expireUserSetTask, expireConnectionTask, expireOnlineUsersTask);
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
        var userKey = $"user:{userId}";
        var members = await _database.SetMembersAsync(userKey);
        return members.Select(x => x.ToString()).ToList();
    }

    public async Task<IEnumerable<string>> GetOnlineUsersAsync()
    {
        var members = await _database.SetMembersAsync(OnlineUsersKey);
        return members.Select(x => x.ToString());
    }

    public async Task RefreshTtlAsync(string userId, string connectionId)
    {
        var userKey = $"user:{userId}";
        var connectionKey = $"connection:{connectionId}";

        var transaction = _database.CreateTransaction();

        var refreshUserKeyTtl = transaction.KeyExpireAsync(userKey, SlidingTtl);
        var refreshConnectionKeyTtl = transaction.KeyExpireAsync(connectionKey, SlidingTtl);
        var refreshOnlineUsersTtl = transaction.KeyExpireAsync(OnlineUsersKey, SlidingTtl);

        var success = await transaction.ExecuteAsync();

        if (!success)
        {
            throw new Exception("Redis transaction to refresh TTL failed.");
        }

        await Task.WhenAll(refreshUserKeyTtl, refreshConnectionKeyTtl, refreshOnlineUsersTtl);
    }
}