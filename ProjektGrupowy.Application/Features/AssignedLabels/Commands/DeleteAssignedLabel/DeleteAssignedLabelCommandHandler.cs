using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Commands.DeleteAssignedLabel;

public class DeleteAssignedLabelCommandHandler : IRequestHandler<DeleteAssignedLabelCommand, Result>
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAssignedLabelCommandHandler(
        IAssignedLabelRepository assignedLabelRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IUnitOfWork unitOfWork)
    {
        _assignedLabelRepository = assignedLabelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAssignedLabelCommand request, CancellationToken cancellationToken)
    {
        var assignedLabel = await _assignedLabelRepository.GetAssignedLabelAsync(request.Id, request.UserId, request.IsAdmin);
        if (assignedLabel is null)
        {
            return Result.Fail("No assigned label found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, assignedLabel, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        _assignedLabelRepository.DeleteAssignedLabel(assignedLabel);
        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
