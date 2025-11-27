using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.VideoGroups.Queries.GetVideoGroup;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.VideoGroups.Queries;

public class GetVideoGroupQueryHandlerTests
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetVideoGroupQueryHandler _handler;

    public GetVideoGroupQueryHandlerTests()
    {
        _videoGroupRepository = Substitute.For<IVideoGroupRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetVideoGroupQueryHandler(
            _videoGroupRepository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_VideoGroupExists_ReturnsVideoGroup()
    {
        // Arrange
        var videoGroupId = 1;
        var userId = "user123";
        var videoGroup = new VideoGroup { Id = videoGroupId };
        var query = new GetVideoGroupQuery(videoGroupId, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(videoGroup));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(videoGroup);
        await _videoGroupRepository.Received(1).GetVideoGroupAsync(videoGroupId, userId, false);
    }

    [Fact]
    public async Task Handle_VideoGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var videoGroupId = 1;
        var userId = "user123";
        var query = new GetVideoGroupQuery(videoGroupId, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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
        var query = new GetVideoGroupQuery(videoGroupId, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(videoGroup));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
