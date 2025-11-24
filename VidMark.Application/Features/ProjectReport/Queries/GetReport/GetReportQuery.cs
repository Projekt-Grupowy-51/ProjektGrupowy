using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectReport.Queries.GetReport;

public record GetReportQuery(int ReportId, string UserId, bool IsAdmin)
    : BaseQuery<Result<GeneratedReport>>(UserId, IsAdmin);
