using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.ProjectReport.Commands.DeleteReport;

public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, Result>
{
    private readonly IProjectReportRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public DeleteReportCommandHandler(
        IProjectReportRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _repository.GetReportAsync(request.ReportId, request.UserId, request.IsAdmin);

        if (report is null)
        {
            return Result.Fail("Error while deleting report.");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            report,
            new ResourceOperationRequirement(ResourceOperation.Delete));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to delete this report.");
        }

        // TODO: add domain event for report deletion
        _repository.DeleteReport(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
