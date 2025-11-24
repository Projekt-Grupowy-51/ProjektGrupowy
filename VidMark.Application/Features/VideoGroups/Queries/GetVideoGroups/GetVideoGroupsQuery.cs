using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideoGroups;

public record GetVideoGroupsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<VideoGroup>>>(UserId, IsAdmin);
