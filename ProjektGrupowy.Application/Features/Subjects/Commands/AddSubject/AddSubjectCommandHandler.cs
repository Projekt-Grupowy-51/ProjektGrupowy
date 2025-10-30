using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Subjects.Commands.AddSubject;

public class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, Result<Subject>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public AddSubjectCommandHandler(
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

    public async Task<Result<Subject>> Handle(AddSubjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Create));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var subject = Subject.Create(request.Name, request.Description, project, request.UserId);

        await _subjectRepository.AddSubjectAsync(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(subject);
    }
}
