using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideoGroupsByProject;

public record GetVideoGroupsByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<VideoGroup>>>(UserId, IsAdmin);
