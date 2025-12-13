using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Labels.Queries.GetLabel;

public record GetLabelQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Label>>(UserId, IsAdmin);
