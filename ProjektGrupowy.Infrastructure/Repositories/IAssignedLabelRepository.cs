using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IAssignedLabelRepository
{
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync(string userId, bool isAdmin);
    Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id, string userId, bool isAdmin);
    Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabel assignedLabel);
    Task DeleteAssignedLabelAsync(AssignedLabel assignedLabel);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId, string userId, bool isAdmin);
    Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId, string userId, bool isAdmin);
}