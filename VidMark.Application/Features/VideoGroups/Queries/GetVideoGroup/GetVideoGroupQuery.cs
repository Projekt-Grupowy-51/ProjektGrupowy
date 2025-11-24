using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideoGroup;

public record GetVideoGroupQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<VideoGroup>>(UserId, IsAdmin);
