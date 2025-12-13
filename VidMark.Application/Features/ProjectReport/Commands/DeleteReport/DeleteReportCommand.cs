using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectReport.Commands.DeleteReport;

public record DeleteReportCommand(int ReportId, string UserId, bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
