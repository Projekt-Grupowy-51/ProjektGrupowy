using ProjektGrupowy.API.DTOs.AccessCode;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IProjectAccessCodeService
{
    Task<bool> ValidateAccessCode(AccessCodeRequest accessCodeRequest);
    Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId);
    Task<Optional<ProjectAccessCode>> AddValidCodeToProjectAsync(CreateAccessCodeRequest createCodeRequest);
    Task<Optional<ProjectAccessCode>> RetireAccessCodeAsync(string code);
}