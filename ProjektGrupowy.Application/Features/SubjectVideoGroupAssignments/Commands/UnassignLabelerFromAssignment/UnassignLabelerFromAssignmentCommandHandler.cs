using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.UnassignLabelerFromAssignment;

public class UnassignLabelerFromAssignmentCommandHandler : IRequestHandler<UnassignLabelerFromAssignmentCommand, Result<SubjectVideoGroupAssignment>>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly IKeycloakUserRepository _keycloakUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public UnassignLabelerFromAssignmentCommandHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        IKeycloakUserRepository keycloakUserRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _keycloakUserRepository = keycloakUserRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<SubjectVideoGroupAssignment>> Handle(UnassignLabelerFromAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetSubjectVideoGroupAssignmentAsync(request.AssignmentId, request.UserId, request.IsAdmin);

        if (assignment is null)
        {
            return Result.Fail("No assignment found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Update));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var labeler = await _keycloakUserRepository.FindByIdAsync(request.LabelerId);

        if (labeler == null)
        {
            return Result.Fail("No labeler found!");
        }

        assignment.UnassignLabeler(labeler, request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(assignment);
    }
}
