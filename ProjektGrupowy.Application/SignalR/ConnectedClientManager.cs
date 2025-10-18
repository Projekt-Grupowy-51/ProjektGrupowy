using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ProjektGrupowy.Application.SignalR;

public class ConnectedClientManager(ILogger<ConnectedClientManager> logger) : IConnectedClientManager
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _clients = [];
    public void AddClient(string userId, string connectionId)
    {
        _clients.AddOrUpdate(userId,
            _ => [connectionId],
            (_, set) =>
            {
                lock (set) set.Add(connectionId);
                return set;
            });

        // Console.WriteLine($"\n\nUser {userId} connected with connection ID {connectionId}. Total connections for user: {_clients[userId].Count}\n\n");
    }

    public void RemoveClient(string connectionId)
    {
        foreach (var kvp in _clients)
        {
            lock (kvp.Value)
            {
                if (kvp.Value.Remove(connectionId) && kvp.Value.Count == 0)
                {
                    _clients.TryRemove(kvp.Key, out _);
                }
            }
        }
    }

    public IReadOnlyList<string> GetConnectionIds(string userId)
    {
        return _clients.TryGetValue(userId, out var set)
            ? [.. set]
            : [];
    }

    public IEnumerable<string> GetOnlineUsers() => _clients.Keys;
}