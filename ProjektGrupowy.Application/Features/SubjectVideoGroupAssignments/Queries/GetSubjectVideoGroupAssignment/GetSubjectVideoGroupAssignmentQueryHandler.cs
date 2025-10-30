using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignment;

public class GetSubjectVideoGroupAssignmentQueryHandler : IRequestHandler<GetSubjectVideoGroupAssignmentQuery, Result<SubjectVideoGroupAssignment>>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSubjectVideoGroupAssignmentQueryHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<SubjectVideoGroupAssignment>> Handle(GetSubjectVideoGroupAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetSubjectVideoGroupAssignmentAsync(request.Id, request.UserId, request.IsAdmin);

        if (assignment is null)
        {
            return Result.Fail("No subject video group assignment found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(assignment);
    }
}
