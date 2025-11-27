using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Subjects.Commands.UpdateSubject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Subjects.Commands;

public class UpdateSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly UpdateSubjectCommandHandler _handler;

    public UpdateSubjectCommandHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _projectRepository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new UpdateSubjectCommandHandler(_subjectRepository, _projectRepository, _unitOfWork, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesAndReturnsSubject()
    {
        // Arrange
        var subjectId = 1;
        var projectId = 1;
        var userId = "user123";
        var subject = new Subject
        {
            Id = subjectId,
            Name = "Old Name",
            Description = "Old Description"
        };
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new UpdateSubjectCommand(
            SubjectId: subjectId,
            ProjectId: projectId,
            Name: "New Name",
            Description: "New Description",
            UserId: userId,
            IsAdmin: false
        );

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        result.Value.Description.Should().Be("New Description");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SubjectNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var projectId = 1;
        var userId = "user123";
        var command = new UpdateSubjectCommand(
            SubjectId: subjectId,
            ProjectId: projectId,
            Name: "Name",
            Description: "Description",
            UserId: userId,
            IsAdmin: false
        );

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No subject found");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var projectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var command = new UpdateSubjectCommand(
            SubjectId: subjectId,
            ProjectId: projectId,
            Name: "Name",
            Description: "Description",
            UserId: userId,
            IsAdmin: false
        );

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnauthorizedSubjectAccess_ThrowsForbiddenException()
    {
        // Arrange
        var subjectId = 1;
        var projectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var command = new UpdateSubjectCommand(
            SubjectId: subjectId,
            ProjectId: projectId,
            Name: "Name",
            Description: "Description",
            UserId: userId,
            IsAdmin: false
        );

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnauthorizedProjectAccess_ThrowsForbiddenException()
    {
        // Arrange
        var subjectId = 1;
        var projectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var project = new Project { Id = projectId, Name = "Test Project" };
        var command = new UpdateSubjectCommand(
            SubjectId: subjectId,
            ProjectId: projectId,
            Name: "Name",
            Description: "Description",
            UserId: userId,
            IsAdmin: false
        );

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        // First call succeeds for subject, second call fails for project
        var callCount = 0;
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(_ =>
            {
                callCount++;
                return Task.FromResult(callCount == 1 ? AuthorizationResult.Success() : AuthorizationResult.Failed());
            });

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
