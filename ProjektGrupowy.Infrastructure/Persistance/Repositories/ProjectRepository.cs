using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Repositories;

public class ProjectRepository(IReadWriteContext context) : IProjectRepository
{
    public async Task AddProjectAsync(Project project)
    {
        _ = await context.Projects.AddAsync(project);
    }

    public Task<Project> GetProjectByAccessCodeAsync(string code)
    {
        return context.Projects
                .Where(p => p.AccessCodes.Any(x => x.Code == code && (x.ExpiresAtUtc == null || x.ExpiresAtUtc > DateTime.UtcNow)))
                .SingleOrDefaultAsync();
    }

    public Task<Dictionary<int, int>> GetLabelerCountForAssignments(int projectId, string userId, bool isAdmin)
    {
        return context.SubjectVideoGroupAssignments
                .FilteredSubjectVideoGroupAssignments(userId, isAdmin)
                .Where(svga => svga.VideoGroup.Project.Id == projectId)
                .GroupBy(svga => svga.Id)
                .Select(g => new
                {
                    SubjectVideoGroupAssignmentId = g.Key,
                    LabelerCount = g.SelectMany(svga => svga.Labelers)
                        .Distinct()
                        .Count()
                })
                .ToDictionaryAsync(x => x.SubjectVideoGroupAssignmentId, x => x.LabelerCount);
    }

    public void DeleteProject(Project project)
    {
        _ = context.Projects.Remove(project);
    }

    public Task<Project> GetProjectAsync(int id, string userId, bool isAdmin)
    {
        return context.Projects.FilteredProjects(userId, isAdmin)
                .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<List<Project>> GetProjectsAsync(string userId, bool isAdmin)
    {
        return context.Projects.FilteredProjects(userId, isAdmin)
                .ToListAsync();
    }
}