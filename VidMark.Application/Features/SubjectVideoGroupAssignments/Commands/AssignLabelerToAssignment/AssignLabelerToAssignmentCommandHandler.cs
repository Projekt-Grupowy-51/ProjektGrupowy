using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.AssignLabelerToAssignment;

public class AssignLabelerToAssignmentCommandHandler : IRequestHandler<AssignLabelerToAssignmentCommand, Result<SubjectVideoGroupAssignment>>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakUserRepository _keycloakUserRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public AssignLabelerToAssignmentCommandHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IKeycloakUserRepository keycloakUserRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _keycloakUserRepository = keycloakUserRepository;
    }

    public async Task<Result<SubjectVideoGroupAssignment>> Handle(AssignLabelerToAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetSubjectVideoGroupAssignmentAsync(request.AssignmentId, request.UserId, request.IsAdmin);

        if (assignment is null)
        {
            return Result.Fail("No assignment found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var labeler = await _keycloakUserRepository.FindByIdAsync(request.LabelerId);

        if (labeler == null)
        {
            return Result.Fail("No labeler found!");
        }

        assignment.AssignLabeler(labeler, request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(assignment);
    }
}
