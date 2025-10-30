using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectReport.Queries.GetReport;

public record GetReportQuery(int ReportId, string UserId, bool IsAdmin)
    : BaseQuery<Result<GeneratedReport>>(UserId, IsAdmin);
