using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabel;

public class GetAssignedLabelQueryHandler : IRequestHandler<GetAssignedLabelQuery, Result<AssignedLabel>>
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetAssignedLabelQueryHandler(IAssignedLabelRepository assignedLabelRepository, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
    {
        _assignedLabelRepository = assignedLabelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<AssignedLabel>> Handle(GetAssignedLabelQuery request, CancellationToken cancellationToken)
    {
        var assignedLabel = await _assignedLabelRepository.GetAssignedLabelAsync(request.Id, request.UserId, request.IsAdmin);
        if (assignedLabel is null)
        {
            return Result.Fail("No assigned label found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, assignedLabel, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : assignedLabel;
    }
}
