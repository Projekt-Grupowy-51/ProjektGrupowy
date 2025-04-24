using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class ProjectRepository(AppDbContext context, ILogger<ProjectRepository> logger, ICurrentUserService currentUserService) : IProjectRepository
{
    public async Task<Optional<Project>> AddProjectAsync(Project project)
    {
        try
        {
            var p = await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            return Optional<Project>.Success(p.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding project");
            return Optional<Project>.Failure(e.Message);
        }
    }

    public async Task<Optional<Project>> GetProjectByAccessCodeAsync(string code)
    {
        try
        {
            // Nested loops and two index lookups "IX_ProjectAccessCode_Code" on "ProjectAccessCodes"
            // and "PK_Projects" on "Projects"
            var project = await context.Projects
                .Where(p => p.AccessCodes.Any(x => x.Code == code && (x.ExpiresAtUtc == null || x.ExpiresAtUtc > DateTime.UtcNow)))
                .SingleOrDefaultAsync();

            return project is null
                ? Optional<Project>.Failure("There is no valid access code for this project.")
                : Optional<Project>.Success(project);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting the project by access code.");
            return Optional<Project>.Failure(e.Message);
        }
    }

    public async Task<Optional<Dictionary<int, int>>> GetLabelerCountForAssignments(int projectId)
    {
        try
        {
            var result = await context.SubjectVideoGroupAssignments
                .FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
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

            return Optional<Dictionary<int, int>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting labeler count for assignments");
            return Optional<Dictionary<int, int>>.Failure(e.Message);
        }
    }

    public async Task DeleteProjectAsync(Project project)
    {
        try
        {
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting project");
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() => await context.Database.BeginTransactionAsync();

    public async Task<Optional<Project>> GetProjectAsync(int id)
    {
        try
        {
            var project = await context.Projects.FilteredProjects(currentUserService.UserId, currentUserService.IsAdmin)
                .FirstOrDefaultAsync(p => p.Id == id);
            return project is null
                ? Optional<Project>.Failure("Project not found")
                : Optional<Project>.Success(project);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting project");
            return Optional<Project>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Project>>> GetProjectsAsync()
    {
        try
        {
            var projects = await context.Projects.FilteredProjects(currentUserService.UserId, currentUserService.IsAdmin)
                .ToListAsync();
            return Optional<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting projects");
            return Optional<IEnumerable<Project>>.Failure(e.Message);
        }
    }

    public async Task<Optional<Project>> UpdateProjectAsync(Project project)
    {
        try
        {
            var p = context.Projects.Update(project);
            await context.SaveChangesAsync();

            return Optional<Project>.Success(p.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating project");
            return Optional<Project>.Failure(e.Message);
        }
    }
}