using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoId;

public record GetAssignedLabelsByVideoIdQuery(int VideoId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<AssignedLabel>>>(UserId, IsAdmin);