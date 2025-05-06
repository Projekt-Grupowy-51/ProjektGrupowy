using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IAssignedLabelService
{
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync();
    Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id);
    Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest);
    Task DeleteAssignedLabelAsync(int id);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId);
}