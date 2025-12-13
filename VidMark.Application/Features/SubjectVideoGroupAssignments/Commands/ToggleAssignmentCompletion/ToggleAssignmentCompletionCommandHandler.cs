using MediatR;
using FluentResults;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.ToggleAssignmentCompletion;

public class ToggleAssignmentCompletionCommandHandler : IRequestHandler<ToggleAssignmentCompletionCommand, Result>
{
    private readonly ISubjectVideoGroupAssignmentRepository _assignmentRepository;
    private readonly IAssignmentCompletionRepository _completionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleAssignmentCompletionCommandHandler(
        ISubjectVideoGroupAssignmentRepository assignmentRepository,
        IAssignmentCompletionRepository completionRepository,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _completionRepository = completionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ToggleAssignmentCompletionCommand request, CancellationToken cancellationToken)
    {
        // Verify assignment exists and user has access
        var assignment = await _assignmentRepository.GetSubjectVideoGroupAssignmentAsync(
            request.AssignmentId,
            request.UserId,
            request.IsAdmin);

        if (assignment == null)
        {
            return Result.Fail("Assignment not found or access denied");
        }

        // Check if user is assigned to this assignment
        var isAssigned = assignment.Labelers.Any(l => l.Id == request.UserId);
        if (!isAssigned && !request.IsAdmin)
        {
            return Result.Fail("You are not assigned to this assignment");
        }

        // Find or create completion record
        var completion = await _completionRepository.GetByAssignmentAndLabelerAsync(
            request.AssignmentId,
            request.UserId);

        if (completion == null)
        {
            completion = SubjectVideoGroupAssignmentCompletion.Create(
                request.AssignmentId,
                request.UserId,
                request.UserId);
            await _completionRepository.AddAsync(completion);
        }

        // Toggle completion status
        if (request.IsCompleted)
        {
            completion.MarkAsCompleted(request.UserId);
        }
        else
        {
            completion.MarkAsIncomplete(request.UserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
