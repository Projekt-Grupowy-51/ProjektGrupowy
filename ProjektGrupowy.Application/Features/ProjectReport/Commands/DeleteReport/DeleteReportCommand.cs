using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectReport.Commands.DeleteReport;

public record DeleteReportCommand(int ReportId, string UserId, bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
