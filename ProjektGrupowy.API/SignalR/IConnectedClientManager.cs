namespace ProjektGrupowy.API.SignalR;

public interface IConnectedClientManager
{
    void AddClient(string userId, string connectionId);
    void RemoveClient(string connectionId);
    IReadOnlyList<string> GetConnectionIds(string userId);
    IEnumerable<string> GetOnlineUsers();
}