using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Subjects.Commands.AddSubject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Subjects.Commands;

public class AddSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly AddSubjectCommandHandler _handler;

    public AddSubjectCommandHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _projectRepository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new AddSubjectCommandHandler(_subjectRepository, _projectRepository, _unitOfWork, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsSubject()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new AddSubjectCommand(
            ProjectId: projectId,
            Name: "Test Subject",
            Description: "Test Description",
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Test Subject");
        result.Value.Description.Should().Be("Test Description");
        result.Value.CreatedById.Should().Be(userId);

        await _subjectRepository.Received(1).AddSubjectAsync(Arg.Any<Subject>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new AddSubjectCommand(
            ProjectId: projectId,
            Name: "Test Subject",
            Description: "Test Description",
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
        await _subjectRepository.DidNotReceive().AddSubjectAsync(Arg.Any<Subject>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new AddSubjectCommand(
            ProjectId: projectId,
            Name: "Test Subject",
            Description: "Test Description",
            UserId: userId,
            IsAdmin: false
        );

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        await _subjectRepository.DidNotReceive().AddSubjectAsync(Arg.Any<Subject>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
