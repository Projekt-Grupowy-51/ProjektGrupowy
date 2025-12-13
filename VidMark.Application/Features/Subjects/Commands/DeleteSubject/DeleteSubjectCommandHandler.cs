using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Subjects.Commands.DeleteSubject;

public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, Result>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public DeleteSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetSubjectAsync(request.Id, request.UserId, request.IsAdmin);

        if (subject is null)
        {
            return Result.Fail("Failed to delete subject");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            subject,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // TODO: add domain event for subject deletion
        _subjectRepository.DeleteSubject(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
