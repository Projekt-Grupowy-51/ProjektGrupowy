using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

public class GetAssignedLabelsCountQueryHandler(
    IAssignedLabelRepository assignedLabelRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetAssignedLabelsCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetAssignedLabelsCountQuery request, CancellationToken cancellationToken)
    {
        var count = await assignedLabelRepository.CountAssignedLabelsByVideoIdAsync(
            request.VideoId,
            request.UserId,
            request.IsAdmin);

        return count;
    }
}