using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface IAssignedLabelRepository
{
    Task<List<AssignedLabel>> GetAssignedLabelsAsync(string userId, bool isAdmin);
    Task<AssignedLabel> GetAssignedLabelAsync(int id, string userId, bool isAdmin);
    Task AddAssignedLabelAsync(AssignedLabel assignedLabel);
    void DeleteAssignedLabel(AssignedLabel assignedLabel);
    Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAsync(int videoId, string userId, bool isAdmin);
    Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId, string userId, bool isAdmin);
}