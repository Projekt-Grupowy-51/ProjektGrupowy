using VidMark.Application.Features.Projects.Commands.AddLabelerToProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Commands;

public class AddLabelerToProjectCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly IKeycloakUserRepository _keycloakUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddLabelerToProjectCommandHandler _handler;

    public AddLabelerToProjectCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _keycloakUserRepository = Substitute.For<IKeycloakUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddLabelerToProjectCommandHandler(_projectRepository, _unitOfWork, _keycloakUserRepository);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsLabelerToProject()
    {
        // Arrange
        var userId = "labeler123";
        var accessCode = "ABC123";
        var labeler = new User { Id = userId, UserName = "Test Labeler" };
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            ProjectLabelers = new List<User>()
        };
        var command = new AddLabelerToProjectCommand(accessCode, userId, false);

        _keycloakUserRepository.FindByIdAsync(userId)
            .Returns(Task.FromResult<User?>(labeler));

        _projectRepository.GetProjectByAccessCodeAsync(accessCode)
            .Returns(Task.FromResult<Project?>(project));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        await _keycloakUserRepository.Received(1).FindByIdAsync(userId);
        await _projectRepository.Received(1).GetProjectByAccessCodeAsync(accessCode);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LabelerNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = "labeler123";
        var accessCode = "ABC123";
        var command = new AddLabelerToProjectCommand(accessCode, userId, false);

        _keycloakUserRepository.FindByIdAsync(userId)
            .Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No labeler found!");
        await _projectRepository.DidNotReceive().GetProjectByAccessCodeAsync(Arg.Any<string>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = "labeler123";
        var accessCode = "ABC123";
        var labeler = new User { Id = userId, UserName = "Test Labeler" };
        var command = new AddLabelerToProjectCommand(accessCode, userId, false);

        _keycloakUserRepository.FindByIdAsync(userId)
            .Returns(Task.FromResult<User?>(labeler));

        _projectRepository.GetProjectByAccessCodeAsync(accessCode)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LabelerAlreadyInProject_DoesNotAddDuplicate()
    {
        // Arrange
        var userId = "labeler123";
        var accessCode = "ABC123";
        var labeler = new User { Id = userId, UserName = "Test Labeler" };
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            ProjectLabelers = new List<User> { labeler }
        };
        var command = new AddLabelerToProjectCommand(accessCode, userId, false);

        _keycloakUserRepository.FindByIdAsync(userId)
            .Returns(Task.FromResult<User?>(labeler));

        _projectRepository.GetProjectByAccessCodeAsync(accessCode)
            .Returns(Task.FromResult<Project?>(project));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.ProjectLabelers.Should().ContainSingle();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
