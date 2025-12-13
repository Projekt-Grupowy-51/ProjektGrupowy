using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Queries.GetProjectStats;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Queries;

public class GetProjectStatsQueryHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetProjectStatsQueryHandler _handler;

    public GetProjectStatsQueryHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetProjectStatsQueryHandler(_projectRepository, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_ProjectExists_ReturnsProjectStats()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var query = new GetProjectStatsQuery(projectId, userId, false);

        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Subjects = new List<Subject>
            {
                new() { Id = 1, SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment> { new() } },
                new() { Id = 2, SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment>() }
            },
            VideoGroups = new List<VideoGroup> { new(), new() },
            ProjectLabelers = new List<User> { new(), new(), new() },
            AccessCodes = new List<ProjectAccessCode> { new() }
        };

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Subjects.Should().HaveCount(2);
        result.Value.VideoGroups.Should().HaveCount(2);
        result.Value.Subjects.First().SubjectVideoGroupAssignments.Should().HaveCount(1);
        result.Value.ProjectLabelers.Should().HaveCount(3);
        result.Value.AccessCodes.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var query = new GetProjectStatsQuery(projectId, userId, false);

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
        var query = new GetProjectStatsQuery(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
