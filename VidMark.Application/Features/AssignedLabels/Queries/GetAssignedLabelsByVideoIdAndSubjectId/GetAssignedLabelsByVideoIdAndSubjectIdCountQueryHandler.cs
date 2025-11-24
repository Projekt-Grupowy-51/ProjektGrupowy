using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

public class GetAssignedLabelsByVideoIdAndSubjectIdCountQueryHandler(
    IAssignedLabelRepository assignedLabelRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetAssignedLabelsByVideoIdAndSubjectIdCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetAssignedLabelsByVideoIdAndSubjectIdCountQuery request, CancellationToken cancellationToken)
    {
        var assignedLabelCount = await assignedLabelRepository.GetAssignedLabelsByVideoIdAndSubjectIdCountAsync(
            request.VideoIds,
            request.SubjectId,
            request.UserId,
            request.IsAdmin);

        return assignedLabelCount;
    }
}