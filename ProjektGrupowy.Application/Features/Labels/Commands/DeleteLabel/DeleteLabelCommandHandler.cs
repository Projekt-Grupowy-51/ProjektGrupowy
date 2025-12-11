using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Labels.Commands.DeleteLabel;

public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, Result>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteLabelCommandHandler(ISubjectRepository subjectRepository, ILabelRepository labelRepository, ICurrentUserService currentUserService, IAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _labelRepository = labelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _labelRepository.GetLabelAsync(request.Id, request.UserId, request.IsAdmin);

        if (label is null)
        {
            return Result.Fail("Label does not exist");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You dont have permission to perform this action");
        }

        _labelRepository.DeleteLabel(label);
        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
