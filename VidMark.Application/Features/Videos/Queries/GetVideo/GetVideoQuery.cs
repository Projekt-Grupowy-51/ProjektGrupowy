using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Videos.Queries.GetVideo;

public record GetVideoQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Video>>(UserId, IsAdmin);
