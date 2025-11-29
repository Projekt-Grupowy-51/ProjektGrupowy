using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.Labels.Commands.UpdateLabel;

public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, Result<Label>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLabelCommandHandler(
        ISubjectRepository subjectRepository,
        ILabelRepository labelRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _labelRepository = labelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Label>> Handle(UpdateLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _labelRepository.GetLabelAsync(request.LabelId, request.UserId, request.IsAdmin);

        if (label is null)
        {
            return Result.Fail("Label does not exist");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Modify));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var subject = await _subjectRepository.GetSubjectAsync(request.SubjectId, request.UserId, request.IsAdmin);
        if (subject is null)
        {
            return Result.Fail("No subject found");
        }

        var authResultSubject = await _authorizationService.AuthorizeAsync(_currentUserService.User, subject, new ResourceOperationRequirement(ResourceOperation.Modify));
        if (!authResultSubject.Succeeded)
        {
            throw new ForbiddenException();
        }

        if (!char.IsLetterOrDigit(request.Shortcut))
        {
            return Result.Fail("Shortcut has to be a letter or a number");
        }

        label.Update(
            request.Name,
            request.ColorHex,
            request.Type,
            request.Shortcut,
            subject,
            request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(label);
    }
}
