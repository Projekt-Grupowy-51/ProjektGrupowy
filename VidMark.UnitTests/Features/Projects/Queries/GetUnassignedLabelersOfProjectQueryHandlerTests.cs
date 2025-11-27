using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Queries;

public class GetUnassignedLabelersOfProjectQueryHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetUnassignedLabelersOfProjectQueryHandler _handler;

    public GetUnassignedLabelersOfProjectQueryHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetUnassignedLabelersOfProjectQueryHandler(_projectRepository, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_AllLabelersUnassigned_ReturnsAllLabelers()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var labelers = new List<User>
        {
            new() { Id = "labeler1", UserName = "Labeler One" },
            new() { Id = "labeler2", UserName = "Labeler Two" }
        };
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            ProjectLabelers = labelers,
            Subjects = new List<Subject>()
        };
        var query = new GetUnassignedLabelersOfProjectQuery(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(labelers);
    }

    [Fact]
    public async Task Handle_SomeLabelersAssigned_ReturnsOnlyUnassigned()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var assignedLabeler = new User { Id = "labeler1", UserName = "Labeler One" };
        var unassignedLabeler = new User { Id = "labeler2", UserName = "Labeler Two" };

        var assignment = new SubjectVideoGroupAssignment
        {
            Labelers = new List<User> { assignedLabeler }
        };

        var subject = new Subject
        {
            SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment> { assignment }
        };

        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            ProjectLabelers = new List<User> { assignedLabeler, unassignedLabeler },
            Subjects = new List<Subject> { subject }
        };

        var query = new GetUnassignedLabelersOfProjectQuery(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value.Should().Contain(unassignedLabeler);
    }

    [Fact]
    public async Task Handle_AllLabelersAssigned_ReturnsEmptyList()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var labeler = new User { Id = "labeler1", UserName = "Labeler One" };

        var assignment = new SubjectVideoGroupAssignment
        {
            Labelers = new List<User> { labeler }
        };

        var subject = new Subject
        {
            SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment> { assignment }
        };

        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            ProjectLabelers = new List<User> { labeler },
            Subjects = new List<Subject> { subject }
        };

        var query = new GetUnassignedLabelersOfProjectQuery(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var query = new GetUnassignedLabelersOfProjectQuery(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var query = new GetUnassignedLabelersOfProjectQuery(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
