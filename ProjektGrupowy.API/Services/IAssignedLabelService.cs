using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IAssignedLabelService
{
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync();
    Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id);
    Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest);

    Task<Optional<AssignedLabel>> UpdateAssignedLabelAsync(int assignedLabelId,
        AssignedLabelRequest assignedLabelRequest);
    Task DeleteAssignedLabelAsync(int id);

    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByScientistIdAsync(int scientistId);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByLabelerIdAsync(int labelerId);

    Task<bool> IsAssignedToLabelerAsync(int subjectVideoGroupAssignmentId, int labelerId);
    Task<bool> IsAssignedToScientistAsync(int subjectVideoGroupAssignmentId, int scientistId);
}