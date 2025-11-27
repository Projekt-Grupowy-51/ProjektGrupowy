using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VidMark.Application.Authorization;
using VidMark.Application.Features.Videos.Commands.AddVideo;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Videos.Commands;

public class AddVideoCommandHandlerTests
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly AddVideoCommandHandler _handler;

    public AddVideoCommandHandlerTests()
    {
        _videoRepository = Substitute.For<IVideoRepository>();
        _videoGroupRepository = Substitute.For<IVideoGroupRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _configuration = Substitute.For<IConfiguration>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new AddVideoCommandHandler(
            _videoRepository,
            _videoGroupRepository,
            _unitOfWork,
            _configuration,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_VideoGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var videoGroupId = 1;
        var userId = "user123";
        var file = Substitute.For<IFormFile>();
        var command = new AddVideoCommand("Test Video", file, videoGroupId, 1, userId, false);

        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No video group found!");
    }
}
