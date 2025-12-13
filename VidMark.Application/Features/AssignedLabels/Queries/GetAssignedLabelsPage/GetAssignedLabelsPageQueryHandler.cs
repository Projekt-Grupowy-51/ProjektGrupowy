using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

public class GetAssignedLabelsPageQueryHandler(
    IAssignedLabelRepository assignedLabelRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetAssignedLabelsPageQuery, Result<List<AssignedLabel>>>
{
    public async Task<Result<List<AssignedLabel>>> Handle(GetAssignedLabelsPageQuery request,
        CancellationToken cancellationToken)
    {
        var assignedLabels = await assignedLabelRepository.GetAssignedLabelsByVideoPageAsync(
            request.VideoId,
            request.Page,
            request.PageSize,
            request.UserId,
            request.IsAdmin);

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            assignedLabels,
            new ResourceOperationRequirement(ResourceOperation.Read));

        return !authResult.Succeeded
            ? throw new ForbiddenException()
            : assignedLabels;
    }
}