using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.AssignedLabel;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Services.Impl;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoId;

public class GetAssignedLabelsByVideoIdQueryHandler : IRequestHandler<GetAssignedLabelsByVideoIdQuery, Result<List<AssignedLabel>>>
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetAssignedLabelsByVideoIdQueryHandler(
        IAssignedLabelRepository assignedLabelRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _assignedLabelRepository = assignedLabelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<AssignedLabel>>> Handle(GetAssignedLabelsByVideoIdQuery request, CancellationToken cancellationToken)
    {
        var assignedLabels = await _assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(request.VideoId, request.UserId, request.IsAdmin);
        if (assignedLabels is null)
        {
            return Result.Fail("No assigned labels found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : assignedLabels;
    }
}
