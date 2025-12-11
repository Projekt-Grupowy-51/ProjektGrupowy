using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideoGroupsByProject;

public record GetVideoGroupsByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<VideoGroup>>>(UserId, IsAdmin);
