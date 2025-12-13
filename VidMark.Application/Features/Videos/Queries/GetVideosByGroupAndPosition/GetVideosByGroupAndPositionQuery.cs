using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Videos.Queries.GetVideosByGroupAndPosition;

public record GetVideosByGroupAndPositionQuery(int VideoGroupId, int PositionInQueue, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Video>>>(UserId, IsAdmin);
