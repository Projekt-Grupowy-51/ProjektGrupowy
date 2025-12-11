using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface IAssignedLabelRepository
{
    Task<List<AssignedLabel>> GetAssignedLabelsAsync(string userId, bool isAdmin);
    Task<AssignedLabel> GetAssignedLabelAsync(int id, string userId, bool isAdmin);
    Task AddAssignedLabelAsync(AssignedLabel assignedLabel);
    void DeleteAssignedLabel(AssignedLabel assignedLabel);
    Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAsync(int videoId, string userId, bool isAdmin);
    Task<int> CountAssignedLabelsByVideoIdAsync(int videoId, string userId, bool isAdmin);

    Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId, string userId,
        bool isAdmin);

    Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAndSubjectIdPageAsync(int[] videoIds, int subjectId, int page,
        int pageSize, string userId, bool isAdmin);
    
    Task<int> GetAssignedLabelsByVideoIdAndSubjectIdCountAsync(int[] videoIds, int subjectId, string userId, bool isAdmin);

    Task<List<AssignedLabel>> GetAssignedLabelsByVideoPageAsync(int videoId, int page, int pageSize, string userId,
        bool isAdmin);
}