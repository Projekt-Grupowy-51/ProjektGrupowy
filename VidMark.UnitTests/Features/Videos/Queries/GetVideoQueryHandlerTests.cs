using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Videos.Queries.GetVideo;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Videos.Queries;

public class GetVideoQueryHandlerTests
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetVideoQueryHandler _handler;

    public GetVideoQueryHandlerTests()
    {
        _videoRepository = Substitute.For<IVideoRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetVideoQueryHandler(
            _videoRepository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_VideoExists_ReturnsVideo()
    {
        // Arrange
        var videoId = 1;
        var userId = "user123";
        var video = new Video { Id = videoId };
        var query = new GetVideoQuery(videoId, userId, false);

        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(video));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(video);
        await _videoRepository.Received(1).GetVideoAsync(videoId, userId, false);
    }

    [Fact]
    public async Task Handle_VideoNotFound_ReturnsFailure()
    {
        // Arrange
        var videoId = 1;
        var userId = "user123";
        var query = new GetVideoQuery(videoId, userId, false);

        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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
        var query = new GetVideoQuery(videoId, userId, false);

        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(video));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
