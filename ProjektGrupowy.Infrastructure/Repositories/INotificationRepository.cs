using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface INotificationRepository
{
    Task<Optional<Notification>> AddNotificationAsync(Notification notification);
    Task<Optional<Notification>> GetNotificationByIdAsync(int id);
    Task<Optional<IEnumerable<Notification>>> GetNotificationsByUserAsync(string userId);
    Task<Optional<IEnumerable<Notification>>> GetNotificationsBetweenDatesAsync(DateTime startDate, DateTime endDate);
}