using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IAssignedLabelRepository
{
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync();
    Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id);
    Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabel assignedLabel);
    Task<Optional<AssignedLabel>> UpdateAssignedLabelAsync(AssignedLabel assignedLabel);
    Task DeleteAssignedLabelAsync(AssignedLabel assignedLabel);

    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByScientistIdAsync(int scientistId);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByLabelerIdAsync(int labelerId);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId);
}