using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IProjectAccessCodeRepository
{
    Task<Optional<ProjectAccessCode>> GetAccessCodeByCodeAsync(string code, string userId, bool isAdmin);
    Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId, string userId, bool isAdmin);
    Task<Optional<ProjectAccessCode>> GetValidAccessCodeByProjectAsync(int projectId, string userId, bool isAdmin);
    Task<Optional<ProjectAccessCode>> AddAccessCodeAsync(ProjectAccessCode accessCode);
    Task<Optional<ProjectAccessCode>> UpdateAccessCodeAsync(ProjectAccessCode accessCode);

    Task<IDbContextTransaction> BeginTransactionAsync();
}