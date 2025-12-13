using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Labels.Queries.GetLabels;

public record GetLabelsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Label>>>(UserId, IsAdmin);
