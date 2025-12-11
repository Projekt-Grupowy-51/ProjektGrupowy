using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideosByVideoGroupId;

public record GetVideosByVideoGroupIdQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Video>>>(UserId, IsAdmin);
