using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.ProjectAccessCodes.Commands.AddValidCodeToProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;
using VidMark.Domain.Enums;

namespace VidMark.UnitTests.Features.ProjectAccessCodes.Commands;

public class AddValidCodeToProjectCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectAccessCodeRepository _projectAccessCodeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddValidCodeToProjectCommandHandler _handler;

    public AddValidCodeToProjectCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _projectAccessCodeRepository = Substitute.For<IProjectAccessCodeRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddValidCodeToProjectCommandHandler(
            _projectRepository,
            _authorizationService,
            _currentUserService,
            _projectAccessCodeRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsAccessCode()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId };
        var command = new AddValidCodeToProjectCommand(projectId, AccessCodeExpiration.In14Days, 0, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));
        _projectAccessCodeRepository.GetValidAccessCodeByProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<ProjectAccessCode?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _projectAccessCodeRepository.Received(1).AddAccessCodeAsync(Arg.Any<ProjectAccessCode>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new AddValidCodeToProjectCommand(projectId, AccessCodeExpiration.In14Days, 0, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Project does not exist!");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId };
        var command = new AddValidCodeToProjectCommand(projectId, AccessCodeExpiration.In14Days, 0, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
