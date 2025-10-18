using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Data;

namespace ProjektGrupowy.Infrastructure.Repositories.Impl;

public class NotificationRepository(AppDbContext dbContext) : INotificationRepository
{
    public async Task<Optional<Notification>> AddNotificationAsync(Notification notification)
    {
        try
        {
            var result = await dbContext.Notifications.AddAsync(notification);
            await dbContext.SaveChangesAsync();
            return Optional<Notification>.Success(result.Entity);
        }
        catch (Exception e)
        {
            return Optional<Notification>.Failure(e.Message);
        }
    }

    public async Task<Optional<Notification>> GetNotificationByIdAsync(int id)
    {
        try
        {
            var notification = await dbContext.Notifications.FindAsync(id);
            return notification is null
                ? Optional<Notification>.Failure("Notification not found")
                : Optional<Notification>.Success(notification);
        }
        catch (Exception e)
        {
            return Optional<Notification>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Notification>>> GetNotificationsByUserAsync(string userId)
    {
        try
        {
            var notifications = await dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.RecipientId == userId)
                .ToArrayAsync();
            
            return Optional<IEnumerable<Notification>>.Success(notifications);
        }
        catch (Exception e)
        {
            return Optional<IEnumerable<Notification>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Notification>>> GetNotificationsBetweenDatesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var notifications = await dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.CreatedAtUtc >= startDate && n.CreatedAtUtc <= endDate)
                .ToArrayAsync();
            
            return Optional<IEnumerable<Notification>>.Success(notifications);
        }
        catch (Exception e)
        {
            return Optional<IEnumerable<Notification>>.Failure(e.Message);
        }
    }
}