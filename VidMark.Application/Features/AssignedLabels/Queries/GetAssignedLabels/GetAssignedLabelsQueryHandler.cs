using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabels;

public class GetAssignedLabelsQueryHandler : IRequestHandler<GetAssignedLabelsQuery, Result<List<AssignedLabel>>>
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetAssignedLabelsQueryHandler(
        IAssignedLabelRepository assignedLabelRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _assignedLabelRepository = assignedLabelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }
    public async Task<Result<List<AssignedLabel>>> Handle(GetAssignedLabelsQuery request, CancellationToken cancellationToken)
    {
        var assignedLabels = await _assignedLabelRepository.GetAssignedLabelsAsync(request.UserId, request.IsAdmin);
        if (assignedLabels is null)
        {
            return Result.Fail("No assigned labels found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : assignedLabels;
    }
}
