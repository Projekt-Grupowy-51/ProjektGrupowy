using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideosByVideoGroupId;

public record GetVideosByVideoGroupIdQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Video>>>(UserId, IsAdmin);
