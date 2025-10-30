using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentLabelers;

public class GetSubjectVideoGroupAssignmentLabelersQueryHandler : IRequestHandler<GetSubjectVideoGroupAssignmentLabelersQuery, Result<List<User>>>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSubjectVideoGroupAssignmentLabelersQueryHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<User>>> Handle(GetSubjectVideoGroupAssignmentLabelersQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetSubjectVideoGroupAssignmentAsync(request.Id, request.UserId, request.IsAdmin);

        if (assignment is null)
        {
            return Result.Fail("No subject video group assignment found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var labelers = await _repository.GetSubjectVideoGroupAssignmentsLabelersAsync(request.Id, request.UserId, request.IsAdmin);

        return Result.Ok(labelers);
    }
}
