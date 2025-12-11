using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideoGroups;

public record GetVideoGroupsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<VideoGroup>>>(UserId, IsAdmin);
