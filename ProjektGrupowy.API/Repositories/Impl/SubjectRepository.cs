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
            // Index lookup using "IX_Projects_ScientistId" btree ("ScientistId")
            var subjects = await context.Subjects
                .Where(s => s.Project.Id == projectId)
                .ToArrayAsync();

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

    public async Task<Optional<IEnumerable<Subject>>> GetSubjectsByScientistId(int scientistId)
    {
        try
        {
            var subjects = await context.Subjects
                .Where(s => s.Project.Scientist.Id == scientistId)
                .ToArrayAsync();

            return Optional<IEnumerable<Subject>>.Success(subjects);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subjects by scientist id");
            return Optional<IEnumerable<Subject>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId)
    {
        try
        {
            var labels = await context.Labels
                .Where(l => l.Subject.Id == subjectId)
                .ToListAsync();

            return Optional<IEnumerable<Label>>.Success(labels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting labels for subject");
            return Optional<IEnumerable<Label>>.Failure(e.Message);
        }
    }
}