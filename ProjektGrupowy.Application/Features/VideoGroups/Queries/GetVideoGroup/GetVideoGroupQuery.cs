using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideoGroup;

public record GetVideoGroupQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<VideoGroup>>(UserId, IsAdmin);
