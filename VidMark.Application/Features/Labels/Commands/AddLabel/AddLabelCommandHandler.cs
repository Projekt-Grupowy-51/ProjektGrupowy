using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.Labels.Commands.AddLabel;

public class AddLabelCommandHandler : IRequestHandler<AddLabelCommand, Result<Label>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddLabelCommandHandler(ISubjectRepository subjectRepository, ILabelRepository labelRepository, ICurrentUserService currentUserService, IAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _labelRepository = labelRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Label>> Handle(AddLabelCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetSubjectAsync(request.SubjectId, request.UserId, request.IsAdmin);

        if (subject is null)
        {
            return Result.Fail("No subject found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, subject, new ResourceOperationRequirement(ResourceOperation.Modify));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You dont have permission to perform this action");
        }

        if (!char.IsLetterOrDigit(request.Shortcut))
        {
            return Result.Fail("Shortcut has to be a letter or a number");
        }

        var label = Label.Create(
            request.Name,
            request.ColorHex,
            request.Type,
            request.Shortcut,
            subject,
            request.UserId);

        await _labelRepository.AddLabelAsync(label);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(label);
    }
}
