using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IProjectAccessCodeRepository
{
    Task<Optional<ProjectAccessCode>> GetAccessCodeByCodeAsync(string code);
    Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId);
    Task<Optional<ProjectAccessCode>> GetValidAccessCodeByProjectAsync(int projectId);
    Task<Optional<ProjectAccessCode>> AddAccessCodeAsync(ProjectAccessCode accessCode);
    Task<Optional<ProjectAccessCode>> UpdateAccessCodeAsync(ProjectAccessCode accessCode);

    Task<IDbContextTransaction> BeginTransactionAsync();
}