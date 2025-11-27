using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Labels.Commands.AddLabel;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Labels.Commands;

public class AddLabelCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddLabelCommandHandler _handler;

    public AddLabelCommandHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _labelRepository = Substitute.For<ILabelRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddLabelCommandHandler(
            _subjectRepository,
            _labelRepository,
            _currentUserService,
            _authorizationService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsLabel()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId };
        var command = new AddLabelCommand("Test Label", "Test Description", userId, subjectId, "#FF0000", 'A', "Type1", false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _labelRepository.Received(1).AddLabelAsync(Arg.Any<Label>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SubjectNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var command = new AddLabelCommand("Test Label", "Test Description", userId, subjectId, "#FF0000", 'A', "Type1", false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No subject found");
    }

    [Fact]
    public async Task Handle_InvalidShortcut_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId };
        var command = new AddLabelCommand("Test Label", "Test Description", userId, subjectId, "#FF0000", '!', "Type1", false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Shortcut has to be a letter or a number");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId };
        var command = new AddLabelCommand("Test Label", "Test Description", userId, subjectId, "#FF0000", 'A', "Type1", false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
