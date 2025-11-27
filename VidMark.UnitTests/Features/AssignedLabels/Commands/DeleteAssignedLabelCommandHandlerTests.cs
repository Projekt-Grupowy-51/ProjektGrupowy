using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.AssignedLabels.Commands.DeleteAssignedLabel;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.AssignedLabels.Commands;

public class DeleteAssignedLabelCommandHandlerTests
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteAssignedLabelCommandHandler _handler;

    public DeleteAssignedLabelCommandHandlerTests()
    {
        _assignedLabelRepository = Substitute.For<IAssignedLabelRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new DeleteAssignedLabelCommandHandler(
            _assignedLabelRepository,
            _currentUserService,
            _authorizationService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesAssignedLabel()
    {
        // Arrange
        var assignedLabelId = 1;
        var userId = "user123";
        var assignedLabel = new AssignedLabel { Id = assignedLabelId };
        var command = new DeleteAssignedLabelCommand(assignedLabelId, userId, false);

        _assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId, userId, false)
            .Returns(Task.FromResult<AssignedLabel?>(assignedLabel));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _assignedLabelRepository.Received(1).DeleteAssignedLabel(assignedLabel);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AssignedLabelNotFound_ReturnsFailure()
    {
        // Arrange
        var assignedLabelId = 1;
        var userId = "user123";
        var command = new DeleteAssignedLabelCommand(assignedLabelId, userId, false);

        _assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId, userId, false)
            .Returns(Task.FromResult<AssignedLabel?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No assigned label found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var assignedLabelId = 1;
        var userId = "user123";
        var assignedLabel = new AssignedLabel { Id = assignedLabelId };
        var command = new DeleteAssignedLabelCommand(assignedLabelId, userId, false);

        _assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId, userId, false)
            .Returns(Task.FromResult<AssignedLabel?>(assignedLabel));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
