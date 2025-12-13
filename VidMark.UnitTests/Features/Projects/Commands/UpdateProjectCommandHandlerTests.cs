using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Commands.UpdateProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Commands;

public class UpdateProjectCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new UpdateProjectCommandHandler(_projectRepository, _unitOfWork, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesAndReturnsProject()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project
        {
            Id = projectId,
            Name = "Old Name",
            Description = "Old Description"
        };
        var command = new UpdateProjectCommand(
            ProjectId: projectId,
            Name: "New Name",
            Description: "New Description",
            Finished: true,
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        result.Value.Description.Should().Be("New Description");
        result.Value.ModificationDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));
        result.Value.EndDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UpdateNotFinished_DoesNotSetEndDate()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project
        {
            Id = projectId,
            Name = "Old Name",
            Description = "Old Description",
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        };
        var command = new UpdateProjectCommand(
            ProjectId: projectId,
            Name: "Updated Name",
            Description: "Updated Description",
            Finished: false,
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EndDate.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new UpdateProjectCommand(
            ProjectId: projectId,
            Name: "Name",
            Description: "Description",
            Finished: false,
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new UpdateProjectCommand(
            ProjectId: projectId,
            Name: "Name",
            Description: "Description",
            Finished: false,
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
