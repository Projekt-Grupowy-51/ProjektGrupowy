using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignment;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.SubjectVideoGroupAssignments.Queries;

public class GetSubjectVideoGroupAssignmentQueryHandlerTests
{
    private readonly ISubjectVideoGroupAssignmentRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetSubjectVideoGroupAssignmentQueryHandler _handler;

    public GetSubjectVideoGroupAssignmentQueryHandlerTests()
    {
        _repository = Substitute.For<ISubjectVideoGroupAssignmentRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetSubjectVideoGroupAssignmentQueryHandler(
            _repository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_AssignmentExists_ReturnsAssignment()
    {
        // Arrange
        var assignmentId = 1;
        var userId = "user123";
        var assignment = new SubjectVideoGroupAssignment { Id = assignmentId };
        var query = new GetSubjectVideoGroupAssignmentQuery(assignmentId, userId, false);

        _repository.GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, false)
            .Returns(Task.FromResult<SubjectVideoGroupAssignment?>(assignment));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(assignment);
        await _repository.Received(1).GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, false);
    }

    [Fact]
    public async Task Handle_AssignmentNotFound_ReturnsFailure()
    {
        // Arrange
        var assignmentId = 1;
        var userId = "user123";
        var query = new GetSubjectVideoGroupAssignmentQuery(assignmentId, userId, false);

        _repository.GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, false)
            .Returns(Task.FromResult<SubjectVideoGroupAssignment?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No subject video group assignment found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var assignmentId = 1;
        var userId = "user123";
        var assignment = new SubjectVideoGroupAssignment { Id = assignmentId };
        var query = new GetSubjectVideoGroupAssignmentQuery(assignmentId, userId, false);

        _repository.GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, false)
            .Returns(Task.FromResult<SubjectVideoGroupAssignment?>(assignment));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
