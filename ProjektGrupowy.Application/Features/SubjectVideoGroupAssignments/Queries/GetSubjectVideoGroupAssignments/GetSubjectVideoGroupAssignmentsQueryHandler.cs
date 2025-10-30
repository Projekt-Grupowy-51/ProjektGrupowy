using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignments;

public class GetSubjectVideoGroupAssignmentsQueryHandler : IRequestHandler<GetSubjectVideoGroupAssignmentsQuery, Result<List<SubjectVideoGroupAssignment>>>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSubjectVideoGroupAssignmentsQueryHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<SubjectVideoGroupAssignment>>> Handle(GetSubjectVideoGroupAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _repository.GetSubjectVideoGroupAssignmentsAsync(request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignments,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(assignments);
    }
}
