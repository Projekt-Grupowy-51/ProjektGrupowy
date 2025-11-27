using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Labels.Commands.UpdateLabel;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Labels.Commands;

public class UpdateLabelCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateLabelCommandHandler _handler;

    public UpdateLabelCommandHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _labelRepository = Substitute.For<ILabelRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateLabelCommandHandler(
            _subjectRepository,
            _labelRepository,
            _currentUserService,
            _authorizationService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesLabel()
    {
        // Arrange
        var labelId = 1;
        var subjectId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var subject = new Subject { Id = subjectId };
        var command = new UpdateLabelCommand(labelId, "Updated Label", "Updated Description", userId, subjectId, 'B', "#00FF00", "Type2", false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(label);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LabelNotFound_ReturnsFailure()
    {
        // Arrange
        var labelId = 1;
        var userId = "user123";
        var command = new UpdateLabelCommand(labelId, "Updated Label", "Updated Description", userId, 1, 'B', "#00FF00", "Type2", false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Label does not exist");
    }

    [Fact]
    public async Task Handle_SubjectNotFound_ReturnsFailure()
    {
        // Arrange
        var labelId = 1;
        var subjectId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var command = new UpdateLabelCommand(labelId, "Updated Label", "Updated Description", userId, subjectId, 'B', "#00FF00", "Type2", false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));
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
        var labelId = 1;
        var subjectId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var subject = new Subject { Id = subjectId };
        var command = new UpdateLabelCommand(labelId, "Updated Label", "Updated Description", userId, subjectId, '!', "#00FF00", "Type2", false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));
        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

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
        var labelId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var command = new UpdateLabelCommand(labelId, "Updated Label", "Updated Description", userId, 1, 'B', "#00FF00", "Type2", false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
