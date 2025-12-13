using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class ProjectAccessCodeRepository(IReadWriteContext dbContext) : IProjectAccessCodeRepository
{
    public Task<ProjectAccessCode> GetAccessCodeByCodeAsync(string code, string userId, bool isAdmin)
    {
        return dbContext
            .ProjectAccessCodes.FilteredProjectAccessCodes(userId, isAdmin)
            .FirstOrDefaultAsync(p => p.Code == code);
    }

    public Task<List<ProjectAccessCode>> GetAccessCodesByProjectAsync(int projectId, string userId, bool isAdmin)
    {
        return dbContext
            .ProjectAccessCodes.FilteredProjectAccessCodes(userId, isAdmin)
            .Where(p => p.Project.Id == projectId)
            .ToListAsync();
    }

    public Task<ProjectAccessCode> GetValidAccessCodeByProjectAsync(int projectId, string userId, bool isAdmin)
    {
        return dbContext
            .ProjectAccessCodes.FilteredProjectAccessCodes(userId, isAdmin)
            .Where(p => p.Project.Id == projectId)
            .Where(p => p.ExpiresAtUtc == null || p.ExpiresAtUtc > DateTime.UtcNow)
            .SingleOrDefaultAsync();
    }

    public async Task AddAccessCodeAsync(ProjectAccessCode accessCode)
    {
        _ = await dbContext.ProjectAccessCodes.AddAsync(accessCode);
    }
}