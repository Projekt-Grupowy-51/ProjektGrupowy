using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.AddSubjectVideoGroupAssignment;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.SubjectVideoGroupAssignments.Commands;

public class AddSubjectVideoGroupAssignmentCommandHandlerTests
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly AddSubjectVideoGroupAssignmentCommandHandler _handler;

    public AddSubjectVideoGroupAssignmentCommandHandlerTests()
    {
        _repository = Substitute.For<ISubjectVideoGroupAssignmentRepository>();
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _videoGroupRepository = Substitute.For<IVideoGroupRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new AddSubjectVideoGroupAssignmentCommandHandler(
            _repository,
            _subjectRepository,
            _videoGroupRepository,
            _unitOfWork,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsAssignment()
    {
        // Arrange
        var subjectId = 1;
        var videoGroupId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId };
        var videoGroup = new VideoGroup { Id = videoGroupId };
        var command = new AddSubjectVideoGroupAssignmentCommand(subjectId, videoGroupId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(videoGroup));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _repository.Received(1).AddSubjectVideoGroupAssignmentAsync(Arg.Any<SubjectVideoGroupAssignment>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SubjectNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var videoGroupId = 1;
        var userId = "user123";
        var command = new AddSubjectVideoGroupAssignmentCommand(subjectId, videoGroupId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No subject found!");
    }

    [Fact]
    public async Task Handle_VideoGroupNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var videoGroupId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId };
        var command = new AddSubjectVideoGroupAssignmentCommand(subjectId, videoGroupId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));
        _videoGroupRepository.GetVideoGroupAsync(videoGroupId, userId, false)
            .Returns(Task.FromResult<VideoGroup?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No video group found!");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var subjectId = 1;
        var videoGroupId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId };
        var command = new AddSubjectVideoGroupAssignmentCommand(subjectId, videoGroupId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
