using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

public class GetAssignedLabelsByVideoIdAndSubjectIdPageQueryHandler(
    IAssignedLabelRepository assignedLabelRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetAssignedLabelsByVideoIdAndSubjectIdPageQuery, Result<List<AssignedLabel>>>
{
    public async Task<Result<List<AssignedLabel>>> Handle(GetAssignedLabelsByVideoIdAndSubjectIdPageQuery request, CancellationToken cancellationToken)
    {
        var assignedLabels = await assignedLabelRepository.GetAssignedLabelsByVideoIdAndSubjectIdPageAsync(
            request.VideoIds,
            request.SubjectId,
            request.Page,
            request.PageSize,
            request.UserId,
            request.IsAdmin);
        
        if (assignedLabels is null)
        {
            return Result.Fail("No assigned labels found");
        }

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded ? throw new ForbiddenException() : assignedLabels;
    }
}