using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Videos.Queries.GetVideosByGroupAndPosition;

public record GetVideosByGroupAndPositionQuery(int VideoGroupId, int PositionInQueue, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Video>>>(UserId, IsAdmin);
