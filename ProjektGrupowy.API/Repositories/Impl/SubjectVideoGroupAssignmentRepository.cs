using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class SubjectVideoGroupAssignmentRepository(
    AppDbContext context,
    ILogger<SubjectVideoGroupAssignmentRepository> logger) : ISubjectVideoGroupAssignmentRepository
{
    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        try
        {
            var subjectVideoGroupAssignments = await context.SubjectVideoGroupAssignments.ToListAsync();
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Success(subjectVideoGroupAssignments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments");
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Failure(e.Message);
        }
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        try
        {
            var subjectVideoGroupAssignment =
                await context.SubjectVideoGroupAssignments.FirstOrDefaultAsync(x => x.Id == id);
            return subjectVideoGroupAssignment is null
                ? Optional<SubjectVideoGroupAssignment>.Failure("Subject video group assignment not found")
                : Optional<SubjectVideoGroupAssignment>.Success(subjectVideoGroupAssignment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignment");
            return Optional<SubjectVideoGroupAssignment>.Failure(e.Message);
        }
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(
        SubjectVideoGroupAssignment subjectVideoGroupAssignment)
    {
        try
        {
            var s = await context.SubjectVideoGroupAssignments.AddAsync(subjectVideoGroupAssignment);
            await context.SaveChangesAsync();

            return Optional<SubjectVideoGroupAssignment>.Success(s.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding subject video group assignment");
            return Optional<SubjectVideoGroupAssignment>.Failure(e.Message);
        }
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(
        SubjectVideoGroupAssignment subjectVideoGroupAssignment)
    {
        try
        {
            context.SubjectVideoGroupAssignments.Update(subjectVideoGroupAssignment);
            await context.SaveChangesAsync();

            return Optional<SubjectVideoGroupAssignment>.Success(subjectVideoGroupAssignment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating subject video group assignment");
            return Optional<SubjectVideoGroupAssignment>.Failure(e.Message);
        }
    }

    public async Task DeleteSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment)
    {
        try
        {
            context.SubjectVideoGroupAssignments.Remove(subjectVideoGroupAssignment);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting subject video group assignment");
        }
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        try
        {
            var assignments = await context.SubjectVideoGroupAssignments
                .Where(x => x.Subject.Project.Id == projectId)
                .ToListAsync();

            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Success(assignments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments by project");
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Failure(e.Message);
        }
    }
    public async Task<Optional<IEnumerable<Labeler>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id)
    {
        try
        {
            var labelers = await context.SubjectVideoGroupAssignments
                .SelectMany(x => x.Labelers)
                .ToListAsync();

            return Optional<IEnumerable<Labeler>>.Success(labelers);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments labelers");
            return Optional<IEnumerable<Labeler>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetSubjectVideoGroupAssignmentAsignedLabelsAsync(int id)
    {
        try
        {
            var labels = await context.SubjectVideoGroupAssignments
                .SelectMany(x => x.AssignedLabels)
                .ToListAsync();

            return Optional<IEnumerable<AssignedLabel>>.Success(labels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments labelers");
            return Optional<IEnumerable<AssignedLabel>>.Failure(e.Message);
        }
    }
}