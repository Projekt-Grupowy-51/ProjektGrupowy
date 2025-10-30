using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Videos.Queries.GetVideo;

public record GetVideoQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Video>>(UserId, IsAdmin);
