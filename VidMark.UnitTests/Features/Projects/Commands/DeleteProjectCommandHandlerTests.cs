using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Commands.DeleteProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Commands;

public class DeleteProjectCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new DeleteProjectCommandHandler(_projectRepository, _unitOfWork, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_ProjectExists_DeletesProject()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new DeleteProjectCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _projectRepository.Received(1).DeleteProject(project);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new DeleteProjectCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
        _projectRepository.DidNotReceive().DeleteProject(Arg.Any<Project>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new DeleteProjectCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        _projectRepository.DidNotReceive().DeleteProject(Arg.Any<Project>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidDelete_CallsMethodsInCorrectOrder()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new DeleteProjectCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Received.InOrder(() =>
        {
            _projectRepository.DeleteProject(project);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>());
        });
    }
}
