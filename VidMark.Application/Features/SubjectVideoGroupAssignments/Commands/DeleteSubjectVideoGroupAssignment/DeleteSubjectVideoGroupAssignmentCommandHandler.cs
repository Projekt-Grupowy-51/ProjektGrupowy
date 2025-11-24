using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.DeleteSubjectVideoGroupAssignment;

public class DeleteSubjectVideoGroupAssignmentCommandHandler : IRequestHandler<DeleteSubjectVideoGroupAssignmentCommand, Result>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public DeleteSubjectVideoGroupAssignmentCommandHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteSubjectVideoGroupAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetSubjectVideoGroupAssignmentAsync(request.Id, request.UserId, request.IsAdmin);

        if (assignment is null)
        {
            return Result.Fail("No subject video group assignment found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Delete));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // TODO: add domain event for assignment deletion
        _repository.DeleteSubjectVideoGroupAssignment(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
