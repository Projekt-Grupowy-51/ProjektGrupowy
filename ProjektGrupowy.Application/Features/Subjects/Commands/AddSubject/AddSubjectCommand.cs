using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Subjects.Commands.AddSubject;

public record AddSubjectCommand(
    string Name,
    string Description,
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Subject>>(UserId, IsAdmin);
