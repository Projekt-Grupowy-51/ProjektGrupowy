using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabel;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.AssignedLabels.Queries;

public class GetAssignedLabelQueryHandlerTests
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetAssignedLabelQueryHandler _handler;

    public GetAssignedLabelQueryHandlerTests()
    {
        _assignedLabelRepository = Substitute.For<IAssignedLabelRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetAssignedLabelQueryHandler(
            _assignedLabelRepository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_AssignedLabelExists_ReturnsAssignedLabel()
    {
        // Arrange
        var assignedLabelId = 1;
        var userId = "user123";
        var assignedLabel = new AssignedLabel { Id = assignedLabelId };
        var query = new GetAssignedLabelQuery(assignedLabelId, userId, false);

        _assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId, userId, false)
            .Returns(Task.FromResult<AssignedLabel?>(assignedLabel));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(assignedLabel);
        await _assignedLabelRepository.Received(1).GetAssignedLabelAsync(assignedLabelId, userId, false);
    }

    [Fact]
    public async Task Handle_AssignedLabelNotFound_ReturnsFailure()
    {
        // Arrange
        var assignedLabelId = 1;
        var userId = "user123";
        var query = new GetAssignedLabelQuery(assignedLabelId, userId, false);

        _assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId, userId, false)
            .Returns(Task.FromResult<AssignedLabel?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

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
        var query = new GetAssignedLabelQuery(assignedLabelId, userId, false);

        _assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId, userId, false)
            .Returns(Task.FromResult<AssignedLabel?>(assignedLabel));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
