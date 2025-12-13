using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectReport.Commands.GenerateReport;

public record GenerateReportCommand(int ProjectId, string UserId, bool IsAdmin)
    : BaseCommand<Result<GeneratedReport>>(UserId, IsAdmin);
