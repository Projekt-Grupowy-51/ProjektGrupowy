using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

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