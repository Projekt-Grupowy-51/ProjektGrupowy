using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Subjects.Commands.UpdateSubject;

public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, Result<Subject>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public UpdateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _subjectRepository = subjectRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Subject>> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetSubjectAsync(request.SubjectId, request.UserId, request.IsAdmin);

        if (subject is null)
        {
            return Result.Fail("No subject found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            subject,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResultProject = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResultProject.Succeeded)
        {
            throw new ForbiddenException();
        }

        subject.Update(request.Name, request.Description, project, request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(subject);
    }
}
