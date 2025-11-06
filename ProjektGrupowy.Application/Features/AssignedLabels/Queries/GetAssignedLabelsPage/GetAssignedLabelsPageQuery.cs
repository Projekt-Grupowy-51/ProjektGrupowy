using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

public record GetAssignedLabelsPageQuery(int VideoId, int Page, int PageSize, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<AssignedLabel>>>(UserId, IsAdmin);