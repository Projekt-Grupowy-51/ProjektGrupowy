using FluentResults;
using VidMark.Application.CQRS;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Commands.AddAssignedLabel;

public record AddAssignedLabelCommand(
    int LabelId,
    int VideoId,
    string Start,
    string End,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<AssignedLabel>>(UserId, IsAdmin);
