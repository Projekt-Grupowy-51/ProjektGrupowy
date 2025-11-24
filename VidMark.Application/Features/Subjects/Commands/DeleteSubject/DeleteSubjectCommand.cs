using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Subjects.Commands.DeleteSubject;

public record DeleteSubjectCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
