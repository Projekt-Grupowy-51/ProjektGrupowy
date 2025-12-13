using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.VideoGroups.Commands.DeleteVideoGroup;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.VideoGroups.Commands;

public class DeleteVideoGroupCommandHandlerTests
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly DeleteVideoGroupCommandHandler _handler;

    public DeleteVideoGroupCommandHandlerTests()
    {
        _videoGroupRepository = Substitute.For<IVideoGroupRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new DeleteVideoGroupCommandHandler(
            _videoGroupRepository,
            _unitOfWork,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesVideoGroup()
    {
        // Arrange
        var videoGroupId = 1;
        var userId = "user123";
        var videoGroup = new VideoGroup { Id = videoGroupId };
        var command = new DeleteVideoGroupCommand(videoGroupId, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(videoGroup));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _videoGroupRepository.Received(1).DeleteVideoGroup(videoGroup);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_VideoGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var videoGroupId = 1;
        var userId = "user123";
        var command = new DeleteVideoGroupCommand(videoGroupId, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No video group found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var videoGroupId = 1;
        var userId = "user123";
        var videoGroup = new VideoGroup { Id = videoGroupId };
        var command = new DeleteVideoGroupCommand(videoGroupId, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(videoGroup));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
