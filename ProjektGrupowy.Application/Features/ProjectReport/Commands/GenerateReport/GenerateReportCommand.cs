using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectReport.Commands.GenerateReport;

public record GenerateReportCommand(int ProjectId, string UserId, bool IsAdmin)
    : BaseCommand<Result<GeneratedReport>>(UserId, IsAdmin);
