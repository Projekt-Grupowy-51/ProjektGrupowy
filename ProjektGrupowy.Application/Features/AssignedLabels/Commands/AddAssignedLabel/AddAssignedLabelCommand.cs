using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Commands.AddAssignedLabel;

public record AddAssignedLabelCommand(
    int LabelId,
    int VideoId,
    string Start,
    string End,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<AssignedLabel>>(UserId, IsAdmin);
