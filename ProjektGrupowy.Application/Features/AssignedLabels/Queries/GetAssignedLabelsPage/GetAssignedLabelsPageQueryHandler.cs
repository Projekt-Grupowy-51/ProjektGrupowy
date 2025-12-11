using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

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