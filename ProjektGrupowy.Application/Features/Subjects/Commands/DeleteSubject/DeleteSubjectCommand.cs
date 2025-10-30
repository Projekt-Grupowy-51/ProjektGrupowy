using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Subjects.Commands.DeleteSubject;

public record DeleteSubjectCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
