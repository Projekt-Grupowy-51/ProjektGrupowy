using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class SubjectVideoGroupAssignmentRepository(
    AppDbContext context,
    ILogger<SubjectVideoGroupAssignmentRepository> logger,
    UserManager<User> userManager,
    ICurrentUserService currentUserService) : ISubjectVideoGroupAssignmentRepository
{
    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        try
        {
            var subjectVideoGroupAssignments = await context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
                .ToListAsync();
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
                await context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
                .FirstOrDefaultAsync(x => x.Id == id);
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
            var assignments = await context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
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
    public async Task<Optional<IEnumerable<User>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id)
    {
        try
        {
            var labelers = await context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
                .Where(x => x.Id == id)
                .SelectMany(x => x.Labelers)
                .ToListAsync();

            return Optional<IEnumerable<User>>.Success(labelers);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting subject video group assignments labelers");
            return Optional<IEnumerable<User>>.Failure(e.Message);
        }
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, string labelerId)
    {
        var assignment = await context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("SubjectVideoGroupAssignment not found");
        }

        var labeler = await userManager.FindByIdAsync(labelerId);
        if (labeler == null)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("Labeler not found");
        }

        if (assignment.Labelers == null)
        {
            assignment.Labelers = new List<User>();
        }

        if (!assignment.Labelers.Contains(labeler))
        {
            assignment.Labelers.Add(labeler);
            await context.SaveChangesAsync();
            return Optional<SubjectVideoGroupAssignment>.Success(assignment);
        }

        return Optional<SubjectVideoGroupAssignment>.Failure("Labeler is already assigned to this SubjectVideoGroupAssignment");
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, string labelerId)
    {
        try
        {
            var assignment = await context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(currentUserService.UserId, currentUserService.IsAdmin)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment == null)
            {
                return Optional<SubjectVideoGroupAssignment>.Failure("SubjectVideoGroupAssignment not found");
            }

            var labeler = await userManager.FindByIdAsync(labelerId);
            if (labeler == null)
            {
                return Optional<SubjectVideoGroupAssignment>.Failure("Labeler not found");
            }

            if (assignment.Labelers == null || !assignment.Labelers.Contains(labeler))
            {
                return Optional<SubjectVideoGroupAssignment>.Failure("Labeler is not assigned to this SubjectVideoGroupAssignment");
            }

            assignment.Labelers.Remove(labeler);
            await context.SaveChangesAsync();
            return Optional<SubjectVideoGroupAssignment>.Success(assignment);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while unassigning labeler from assignment");
            return Optional<SubjectVideoGroupAssignment>.Failure(e.Message);
        }
    }
}