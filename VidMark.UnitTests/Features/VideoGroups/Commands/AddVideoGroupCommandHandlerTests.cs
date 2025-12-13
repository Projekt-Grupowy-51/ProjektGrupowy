using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.VideoGroups.Commands.AddVideoGroup;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.VideoGroups.Commands;

public class AddVideoGroupCommandHandlerTests
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly AddVideoGroupCommandHandler _handler;

    public AddVideoGroupCommandHandlerTests()
    {
        _videoGroupRepository = Substitute.For<IVideoGroupRepository>();
        _projectRepository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new AddVideoGroupCommandHandler(
            _videoGroupRepository,
            _projectRepository,
            _unitOfWork,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsVideoGroup()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId };
        var command = new AddVideoGroupCommand("Video Group 1", "Description", projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _videoGroupRepository.Received(1).AddVideoGroupAsync(Arg.Any<VideoGroup>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new AddVideoGroupCommand("Video Group 1", "Description", projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId };
        var command = new AddVideoGroupCommand("Video Group 1", "Description", projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
