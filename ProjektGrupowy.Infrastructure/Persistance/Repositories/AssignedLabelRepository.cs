using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Repositories;

public class AssignedLabelRepository(IReadWriteContext context) : IAssignedLabelRepository
{
    public Task<List<AssignedLabel>> GetAssignedLabelsAsync(string userId, bool isAdmin)
    {
        return context.AssignedLabels.FilteredAssignedLabels(userId, isAdmin)
                .OrderByDescending(a => a.InsDate)
                .ToListAsync();
    }

    public Task<AssignedLabel> GetAssignedLabelAsync(int id, string userId, bool isAdmin)
    {
        return context.AssignedLabels.FilteredAssignedLabels(userId, isAdmin).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAssignedLabelAsync(AssignedLabel assignedLabel)
    {
        _ = await context.AssignedLabels.AddAsync(assignedLabel);
    }

    public void DeleteAssignedLabel(AssignedLabel assignedLabel)
    {
        _ = context.AssignedLabels.Remove(assignedLabel);
    }

    public Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAsync(int videoId, string userId, bool isAdmin)
    {
        return context.AssignedLabels.FilteredAssignedLabels(userId, isAdmin)
                .Where(a => a.Video.Id == videoId)
                .OrderByDescending(a => a.InsDate)
                .ToListAsync();
    }

    public Task<int> CountAssignedLabelsByVideoIdAsync(int videoId, string userId, bool isAdmin)
    {
        return context.AssignedLabels.FilteredAssignedLabels(userId, isAdmin)
            .Where(a => a.Video.Id == videoId)
            .CountAsync();
    }

    public Task<List<AssignedLabel>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId, string userId, bool isAdmin)
    {
        return context.AssignedLabels.FilteredAssignedLabels(userId, isAdmin)
                .Where(a => a.Video.Id == videoId && a.Label.Subject.Id == subjectId)
                .OrderByDescending(a => a.InsDate)
                .ToListAsync();
    }

    public Task<List<AssignedLabel> > GetAssignedLabelsByVideoPageAsync(int videoId, int page, int pageSize, string userId, bool isAdmin)
    {
        return context.AssignedLabels.FilteredAssignedLabels(userId, isAdmin)
            .Where(a => a.Video.Id == videoId)
            .OrderByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}