using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class SubjectRepository(AppDbContext context, ILogger<SubjectRepository> logger) : ISubjectRepository
{
    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync()
    {
        try
        {
            var subjects = await context.Subjects.ToListAsync();
            return Optional<IEnumerable<Subject>>.Success(subjects);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting subjects");
            return Optional<IEnumerable<Subject>>.Failure(ex.Message);
        }
    }

    public async Task<Optional<Subject>> GetSubjectAsync(int id)
    {
        try
        {
            var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            return subject is null
                ? Optional<Subject>.Failure("Subject not found")
                : Optional<Subject>.Success(subject);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject");
            return Optional<Subject>.Failure(e.Message);
        }
    }

    public async Task<Optional<Subject>> AddSubjectAsync(Subject subject)
    {
        try
        {
            var s = await context.Subjects.AddAsync(subject);
            await context.SaveChangesAsync();

            return Optional<Subject>.Success(s.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding subject");
            return Optional<Subject>.Failure(e.Message);
        }
    }

    public async Task<Optional<Subject>> UpdateSubjectAsync(Subject subject)
    {
        try
        {
            context.Subjects.Update(subject);
            await context.SaveChangesAsync();
            return Optional<Subject>.Success(subject);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating subject");
            return Optional<Subject>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId)
    {
        try
        {
            // 1. Include subjects in project
            var projectWithSubjects = await context.Projects
                .Include(p => p.Subjects)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            // 2. Check if project exists and return failure if not
            if (projectWithSubjects is null)
            {
                return Optional<IEnumerable<Subject>>.Failure($"Project of id {projectId} was not found.");
            }
            
            // If the included collection is null, return empty array
            var subjects = projectWithSubjects.Subjects?.ToArray() ?? [];

            return Optional<IEnumerable<Subject>>.Success(subjects);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subjects by project");
            return Optional<IEnumerable<Subject>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(Project project)
        => await GetSubjectsByProjectAsync(project.Id);

    public async Task DeleteSubjectAsync(Subject subject)
    {
        try
        {
            context.Subjects.Remove(subject);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting subject");
        }
    }
}