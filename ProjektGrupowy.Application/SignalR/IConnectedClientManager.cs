namespace ProjektGrupowy.Application.SignalR;

public interface IConnectedClientManager
{
    Task AddClientAsync(string userId, string connectionId);
    Task RemoveClientAsync(string connectionId);
    Task<IReadOnlyList<string>> GetConnectionIdsAsync(string userId);
    Task<IEnumerable<string>> GetOnlineUsersAsync();
}