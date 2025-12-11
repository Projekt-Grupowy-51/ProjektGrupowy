using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Videos.Queries.GetVideos;

public record GetVideosQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Video>>>(UserId, IsAdmin);
