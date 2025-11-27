using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabels;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.AssignedLabels.Queries;

public class GetAssignedLabelsQueryHandlerTests
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetAssignedLabelsQueryHandler _handler;

    public GetAssignedLabelsQueryHandlerTests()
    {
        _assignedLabelRepository = Substitute.For<IAssignedLabelRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetAssignedLabelsQueryHandler(
            _assignedLabelRepository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_AssignedLabelsExist_ReturnsAssignedLabels()
    {
        // Arrange
        var userId = "user123";
        var assignedLabels = new List<AssignedLabel>
        {
            new AssignedLabel { Id = 1 },
            new AssignedLabel { Id = 2 }
        };
        var query = new GetAssignedLabelsQuery(userId, false);

        _assignedLabelRepository.GetAssignedLabelsAsync(userId, false)
            .Returns(Task.FromResult<List<AssignedLabel>?>(assignedLabels));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(assignedLabels);
        await _assignedLabelRepository.Received(1).GetAssignedLabelsAsync(userId, false);
    }

    [Fact]
    public async Task Handle_NoAssignedLabelsFound_ReturnsFailure()
    {
        // Arrange
        var userId = "user123";
        var query = new GetAssignedLabelsQuery(userId, false);

        _assignedLabelRepository.GetAssignedLabelsAsync(userId, false)
            .Returns(Task.FromResult<List<AssignedLabel>?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No assigned labels found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var userId = "user123";
        var assignedLabels = new List<AssignedLabel>
        {
            new AssignedLabel { Id = 1 }
        };
        var query = new GetAssignedLabelsQuery(userId, false);

        _assignedLabelRepository.GetAssignedLabelsAsync(userId, false)
            .Returns(Task.FromResult<List<AssignedLabel>?>(assignedLabels));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
