using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.DTOs.AssignedLabel;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Application.Services.Impl;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoId;

public class GetAssignedLabelsByVideoIdQueryHandler(
    IAssignedLabelRepository assignedLabelRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetAssignedLabelsByVideoIdQuery, Result<List<AssignedLabel>>>
{
    public async Task<Result<List<AssignedLabel>>> Handle(GetAssignedLabelsByVideoIdQuery request, CancellationToken cancellationToken)
    {
        var assignedLabels = await assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(request.VideoId, request.UserId, request.IsAdmin);
        if (assignedLabels is null)
        {
            return Result.Fail("No assigned labels found");
        }

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : assignedLabels;
    }
}
