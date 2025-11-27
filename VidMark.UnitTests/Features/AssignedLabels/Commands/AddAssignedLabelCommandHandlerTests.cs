using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.AssignedLabels.Commands.AddAssignedLabel;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.AssignedLabels.Commands;

public class AddAssignedLabelCommandHandlerTests
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddAssignedLabelCommandHandler _handler;

    public AddAssignedLabelCommandHandlerTests()
    {
        _assignedLabelRepository = Substitute.For<IAssignedLabelRepository>();
        _labelRepository = Substitute.For<ILabelRepository>();
        _videoRepository = Substitute.For<IVideoRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddAssignedLabelCommandHandler(
            _assignedLabelRepository,
            _labelRepository,
            _videoRepository,
            _currentUserService,
            _authorizationService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsAssignedLabel()
    {
        // Arrange
        var labelId = 1;
        var videoId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var video = new Video { Id = videoId };
        var command = new AddAssignedLabelCommand(labelId, videoId, "00:00:10", "00:00:20", userId, false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _videoRepository.GetVideoAsync(videoId, userId, false)
            .Returns(Task.FromResult<Video?>(video));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _assignedLabelRepository.Received(1).AddAssignedLabelAsync(Arg.Any<AssignedLabel>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LabelNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new AddAssignedLabelCommand(1, 1, "00:00:10", "00:00:20", "user123", false);

        _labelRepository.GetLabelAsync(1, "user123", false)
            .Returns(Task.FromResult<Label?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Label does not exist");
    }

    [Fact]
    public async Task Handle_VideoNotFound_ReturnsFailure()
    {
        // Arrange
        var labelId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var command = new AddAssignedLabelCommand(labelId, 1, "00:00:10", "00:00:20", userId, false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));
        _videoRepository.GetVideoAsync(1, userId, false)
            .Returns(Task.FromResult<Video?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No subject video group assignment found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var labelId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var command = new AddAssignedLabelCommand(labelId, 1, "00:00:10", "00:00:20", userId, false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
