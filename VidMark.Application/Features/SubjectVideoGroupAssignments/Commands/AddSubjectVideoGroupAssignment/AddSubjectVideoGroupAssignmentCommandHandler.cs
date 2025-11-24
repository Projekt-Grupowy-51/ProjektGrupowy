using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.AddSubjectVideoGroupAssignment;

public class AddSubjectVideoGroupAssignmentCommandHandler : IRequestHandler<AddSubjectVideoGroupAssignmentCommand, Result<SubjectVideoGroupAssignment>>
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public AddSubjectVideoGroupAssignmentCommandHandler(
        ISubjectVideoGroupAssignmentRepository repository,
        ISubjectRepository subjectRepository,
        IVideoGroupRepository videoGroupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _subjectRepository = subjectRepository;
        _videoGroupRepository = videoGroupRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<SubjectVideoGroupAssignment>> Handle(AddSubjectVideoGroupAssignmentCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetSubjectAsync(request.SubjectId, request.UserId, request.IsAdmin);

        if (subject is null)
        {
            return Result.Fail("No subject found!");
        }

        var authResultSubject = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            subject,
            new ResourceOperationRequirement(ResourceOperation.Create));

        if (!authResultSubject.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroup = await _videoGroupRepository.GetVideoGroupAsync(request.VideoGroupId, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found!");
        }

        var authResultVG = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Create));

        if (!authResultVG.Succeeded)
        {
            throw new ForbiddenException();
        }

        var assignment = SubjectVideoGroupAssignment.Create(subject, videoGroup, request.UserId);

        await _repository.AddSubjectVideoGroupAssignmentAsync(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(assignment);
    }
}
