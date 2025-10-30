using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Labels.Queries.GetLabel;

public record GetLabelQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Label>>(UserId, IsAdmin);
