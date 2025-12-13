using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Subjects.Commands.UpdateSubject;

public record UpdateSubjectCommand(
    int SubjectId,
    string Name,
    string Description,
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Subject>>(UserId, IsAdmin);
