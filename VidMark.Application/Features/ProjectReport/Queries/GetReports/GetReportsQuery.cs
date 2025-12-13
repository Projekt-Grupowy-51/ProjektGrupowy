using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectReport.Queries.GetReports;

public record GetReportsQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<GeneratedReport>>>(UserId, IsAdmin);
