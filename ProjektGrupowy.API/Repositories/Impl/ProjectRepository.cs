using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class ProjectRepository(AppDbContext context, ILogger<ProjectRepository> logger) : IProjectRepository
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

    public async Task<Optional<IEnumerable<Project>>> GetProjectsOfScientist(int scientistId)
    {
        try
        {
            // Index lookup using "IX_Projects_ScientistId" btree ("ScientistId")
            var projects = await context.Projects
                .Where(p => p.Scientist.Id == scientistId)
                .ToArrayAsync();

            return Optional<IEnumerable<Project>>.Success(projects);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting projects of scientist");
            return Optional<IEnumerable<Project>>.Failure(e.Message);
        }
    }

    public Task<Optional<IEnumerable<Project>>> GetProjectsOfScientist(Scientist scientist) =>
        GetProjectsOfScientist(scientist.Id);

    public async Task DeleteScientistAsync(Scientist scientist)
    {
        try
        {
            context.Scientists.Remove(scientist);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete scientist");
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
            var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == id);
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
            var projects = await context.Projects.Include(p => p.Videos).ToListAsync();
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