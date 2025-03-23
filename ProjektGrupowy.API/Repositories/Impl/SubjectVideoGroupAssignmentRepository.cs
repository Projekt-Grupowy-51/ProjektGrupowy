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

    public async Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, int labelerId)
    {
        var assignment = await context.SubjectVideoGroupAssignments
            .Include(a => a.Labelers)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("SubjectVideoGroupAssignment not found");
        }

        var labeler = await context.Labelers.FindAsync(labelerId);
        if (labeler == null)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("Labeler not found");
        }

        if (assignment.Labelers == null)
        {
            assignment.Labelers = new List<Labeler>();
        }

        if (!assignment.Labelers.Contains(labeler))
        {
            assignment.Labelers.Add(labeler);
            await context.SaveChangesAsync();
            return Optional<SubjectVideoGroupAssignment>.Success(assignment);
        }

        return Optional<SubjectVideoGroupAssignment>.Failure("Labeler is already assigned to this SubjectVideoGroupAssignment");
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(int videoGroupId)
    {
        try
        {
            var assignments = await context.SubjectVideoGroupAssignments
                .Where(x => x.VideoGroup.Id == videoGroupId)
                .ToListAsync();

            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Success(assignments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments by video group");
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByScientistIdAsync(int scientistId)
    {
        try
        {
            var assignments = await context.SubjectVideoGroupAssignments
                .Include(x => x.Subject)
                    .ThenInclude(s => s.Project)
                .Include(x => x.VideoGroup)
                .Where(x => x.Subject.Project.Scientist.Id == scientistId)
                .ToListAsync();

            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Success(assignments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments by scientist ID");
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetAssignmentsForLabelerAsync(int labelerId)
    {
        try
        {
            var assignments = await context.SubjectVideoGroupAssignments
                .Include(x => x.Subject)
                .Include(x => x.VideoGroup)
                .Include(x => x.Labelers)
                .Where(x => x.Labelers!.Any(l => l.Id == labelerId))
                .ToListAsync();

            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Success(assignments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting assignments for labeler");
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetLabelerAssignedLabelsAsync(int assignmentId, int labelerId)
    {
        try
        {
            var labels = await context.AssignedLabels
                .Where(x => x.SubjectVideoGroupAssignment.Id == assignmentId && x.Labeler.Id == labelerId)
                .ToListAsync();

            return Optional<IEnumerable<AssignedLabel>>.Success(labels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting assigned labels for labeler in assignment");
            return Optional<IEnumerable<AssignedLabel>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsBySubjectIdAsync(int subjectId)
    {
        try
        {
            var assignments = await context.SubjectVideoGroupAssignments
                .Include(x => x.Subject)
                .Include(x => x.VideoGroup)
                .Include(x => x.Labelers)
                .Where(x => x.Subject.Id == subjectId)
                .ToListAsync();

            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Success(assignments);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments by subject ID {SubjectId}", subjectId);
            return Optional<IEnumerable<SubjectVideoGroupAssignment>>.Failure(e.Message);
        }
    }
}