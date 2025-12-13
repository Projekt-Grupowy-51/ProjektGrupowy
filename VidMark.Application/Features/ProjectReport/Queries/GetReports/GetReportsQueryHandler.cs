using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.ProjectReport.Queries.GetReports;

public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, Result<List<GeneratedReport>>>
{
    private readonly IProjectReportRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetReportsQueryHandler(
        IProjectReportRepository repository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<GeneratedReport>>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _repository.GetReportsAsync(request.ProjectId, request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            reports,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to access these reports.");
        }

        return Result.Ok(reports);
    }
}
