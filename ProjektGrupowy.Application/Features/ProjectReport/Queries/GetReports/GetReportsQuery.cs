using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectReport.Queries.GetReports;

public record GetReportsQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<GeneratedReport>>>(UserId, IsAdmin);
