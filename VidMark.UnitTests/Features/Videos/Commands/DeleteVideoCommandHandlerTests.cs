using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Videos.Commands.DeleteVideo;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Videos.Commands;

public class DeleteVideoCommandHandlerTests
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly DeleteVideoCommandHandler _handler;

    public DeleteVideoCommandHandlerTests()
    {
        _videoRepository = Substitute.For<IVideoRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new DeleteVideoCommandHandler(
            _videoRepository,
            _unitOfWork,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesVideo()
    {
        // Arrange
        var videoId = 1;
        var userId = "user123";
        var video = new Video { Id = videoId };
        var command = new DeleteVideoCommand(videoId, userId, false);

        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(video));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _videoRepository.Received(1).DeleteVideo(video);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_VideoNotFound_ReturnsFailure()
    {
        // Arrange
        var videoId = 1;
        var userId = "user123";
        var command = new DeleteVideoCommand(videoId, userId, false);

        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No video found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var videoId = 1;
        var userId = "user123";
        var video = new Video { Id = videoId };
        var command = new DeleteVideoCommand(videoId, userId, false);

        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(video));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
