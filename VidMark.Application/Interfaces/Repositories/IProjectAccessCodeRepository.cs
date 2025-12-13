using VidMark.Domain.Models;

namespace VidMark.Application.Interfaces.Repositories;

public interface IProjectAccessCodeRepository
{
    Task<ProjectAccessCode> GetAccessCodeByCodeAsync(string code, string userId, bool isAdmin);
    Task<List<ProjectAccessCode>> GetAccessCodesByProjectAsync(int projectId, string userId, bool isAdmin);
    Task<ProjectAccessCode> GetValidAccessCodeByProjectAsync(int projectId, string userId, bool isAdmin);
    Task AddAccessCodeAsync(ProjectAccessCode accessCode);
}