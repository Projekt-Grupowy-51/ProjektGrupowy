using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IProjectAccessCodeRepository
{
    Task<Optional<ProjectAccessCode>> GetAccessCodeByCodeAsync(string code);
    Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId);
    Task<Optional<ProjectAccessCode>> GetValidAccessCodeByProjectAsync(int projectId);
    Task<Optional<ProjectAccessCode>> AddAccessCodeAsync(ProjectAccessCode accessCode);
    Task<Optional<ProjectAccessCode>> UpdateAccessCodeAsync(ProjectAccessCode accessCode);

    Task<IDbContextTransaction> BeginTransactionAsync();
}