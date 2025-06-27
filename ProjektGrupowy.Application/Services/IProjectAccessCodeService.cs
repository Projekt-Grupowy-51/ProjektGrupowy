using ProjektGrupowy.Application.DTOs.AccessCode;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface IProjectAccessCodeService
{
    Task<bool> ValidateAccessCode(AccessCodeRequest accessCodeRequest);
    Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId);
    Task<Optional<ProjectAccessCode>> AddValidCodeToProjectAsync(CreateAccessCodeRequest createCodeRequest);
    Task<Optional<ProjectAccessCode>> RetireAccessCodeAsync(string code);
}