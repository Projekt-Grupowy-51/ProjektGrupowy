using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IAssignedLabelService
{
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync();
    Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id);
    Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabel assignedLabel);
    Task<Optional<AssignedLabel>> UpdateAssignedLabelAsync(AssignedLabel assignedLabel);
    Task DeleteAssignedLabelAsync(int id);
}