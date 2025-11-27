using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Queries.GetProjects;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Queries;

public class GetProjectsQueryHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetProjectsQueryHandler _handler;

    public GetProjectsQueryHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetProjectsQueryHandler(_projectRepository, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_ProjectsExist_ReturnsProjects()
    {
        // Arrange
        var userId = "user123";
        var projects = new List<Project>
        {
            new() { Id = 1, Name = "Project 1" },
            new() { Id = 2, Name = "Project 2" }
        };
        var query = new GetProjectsQuery(userId, false);

        _projectRepository.GetProjectsAsync(userId, false)
            .Returns(Task.FromResult(projects));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(projects);
        await _projectRepository.Received(1).GetProjectsAsync(userId, false);
    }

    [Fact]
    public async Task Handle_NoProjects_ReturnsEmptyList()
    {
        // Arrange
        var userId = "user123";
        var projects = new List<Project>();
        var query = new GetProjectsQuery(userId, false);

        _projectRepository.GetProjectsAsync(userId, false)
            .Returns(Task.FromResult(projects));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var userId = "user123";
        var projects = new List<Project> { new() { Id = 1, Name = "Project 1" } };
        var query = new GetProjectsQuery(userId, false);

        _projectRepository.GetProjectsAsync(userId, false)
            .Returns(Task.FromResult(projects));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
