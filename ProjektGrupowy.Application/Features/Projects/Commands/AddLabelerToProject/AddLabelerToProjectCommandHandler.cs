using FluentResults;
using MediatR;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Projects.Commands.AddLabelerToProject;

public class AddLabelerToProjectCommandHandler : IRequestHandler<AddLabelerToProjectCommand, Result<bool>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IKeycloakUserRepository _keycloakUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddLabelerToProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IKeycloakUserRepository keycloakUserRepository)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _keycloakUserRepository = keycloakUserRepository;
    }

    public async Task<Result<bool>> Handle(AddLabelerToProjectCommand request, CancellationToken cancellationToken)
    {
        var labeler = await _keycloakUserRepository.FindByIdAsync(request.UserId);

        if (labeler == null)
        {
            return Result.Fail("No labeler found!");
        }

        var project = await _projectRepository.GetProjectByAccessCodeAsync(request.AccessCode);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        project.AddLabeler(labeler, request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(true);
    }
}
