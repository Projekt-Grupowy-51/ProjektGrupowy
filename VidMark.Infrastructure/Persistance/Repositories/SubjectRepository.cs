using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class SubjectRepository(IReadWriteContext context) : ISubjectRepository
{
    public Task<List<Subject>> GetSubjectsAsync(string userId, bool isAdmin)
    {
        return context.Subjects.FilteredSubjects(userId, isAdmin)
                .ToListAsync();
    }

    public Task<Subject> GetSubjectAsync(int id, string userId, bool isAdmin)
    {
        return context.Subjects.FilteredSubjects(userId, isAdmin)
                .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddSubjectAsync(Subject subject)
    {
        _ = await context.Subjects.AddAsync(subject);
    }

    public Task<List<Subject>> GetSubjectsByProjectAsync(int projectId, string userId, bool isAdmin)
    {
        return context.Subjects.FilteredSubjects(userId, isAdmin)
                .Where(s => s.Project.Id == projectId)
                .ToListAsync();
    }

    public void DeleteSubject(Subject subject)
    {
        _ = context.Subjects.Remove(subject);
    }

    public Task<List<Label>> GetSubjectLabelsAsync(int subjectId, string userId, bool isAdmin)
    {
        return context.Labels.FilteredLabels(userId, isAdmin)
                .Where(l => l.Subject.Id == subjectId)
                .ToListAsync();
    }
}