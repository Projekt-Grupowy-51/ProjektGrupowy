using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Labels.Queries.GetLabels;

public record GetLabelsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Label>>>(UserId, IsAdmin);
