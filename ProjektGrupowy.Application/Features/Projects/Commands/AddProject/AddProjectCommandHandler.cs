using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;

namespace ProjektGrupowy.Application.Features.Projects.Commands.AddProject;

public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand, Result<Project>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Project>> Handle(AddProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(request.Name, request.Description, request.UserId);

        await _projectRepository.AddProjectAsync(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(project);
    }
}
