using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

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