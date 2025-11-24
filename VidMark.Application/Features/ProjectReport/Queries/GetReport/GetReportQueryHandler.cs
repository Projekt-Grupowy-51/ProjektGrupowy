using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.ProjectReport.Queries.GetReport;

public class GetReportQueryHandler : IRequestHandler<GetReportQuery, Result<GeneratedReport>>
{
    private readonly IProjectReportRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetReportQueryHandler(
        IProjectReportRepository repository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<GeneratedReport>> Handle(GetReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _repository.GetReportAsync(request.ReportId, request.UserId, request.IsAdmin);

        if (report is null)
        {
            return Result.Fail("Report not found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            report,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to access this report.");
        }

        return Result.Ok(report);
    }
}
