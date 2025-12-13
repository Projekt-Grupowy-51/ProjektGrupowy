using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Videos.Queries.GetVideos;

public record GetVideosQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Video>>>(UserId, IsAdmin);
