using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Commands.DistributeLabelersEqually;
using VidMark.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Projects.Commands;

public class DistributeLabelersEquallyCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISubjectVideoGroupAssignmentRepository _subjectVideoGroupAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMediator _mediator;
    private readonly DistributeLabelersEquallyCommandHandler _handler;

    public DistributeLabelersEquallyCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _subjectVideoGroupAssignmentRepository = Substitute.For<ISubjectVideoGroupAssignmentRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DistributeLabelersEquallyCommandHandler(
            _projectRepository,
            _subjectVideoGroupAssignmentRepository,
            _unitOfWork,
            _currentUserService,
            _authorizationService,
            _mediator
        );
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new DistributeLabelersEquallyCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

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
        var command = new DistributeLabelersEquallyCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NoAssignments_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Subjects = new List<Subject>()
        };
        var command = new DistributeLabelersEquallyCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "There are no assignments.");
    }

    [Fact]
    public async Task Handle_GetUnassignedLabelersFails_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var subject = new Subject
        {
            SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment>
            {
                new SubjectVideoGroupAssignment()
            }
        };
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Subjects = new List<Subject> { subject }
        };
        var command = new DistributeLabelersEquallyCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        _mediator.Send(Arg.Any<GetUnassignedLabelersOfProjectQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Fail<List<User>>("Failed to get unassigned labelers"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No unassigned labelers found!");
    }

    [Fact]
    public async Task Handle_NoUnassignedLabelers_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var subject = new Subject
        {
            SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment>
            {
                new SubjectVideoGroupAssignment()
            }
        };
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Subjects = new List<Subject> { subject }
        };
        var command = new DistributeLabelersEquallyCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        _mediator.Send(Arg.Any<GetUnassignedLabelersOfProjectQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(new List<User>()));

        _projectRepository.GetLabelerCountForAssignments(projectId, userId, false)
            .Returns(Task.FromResult(new Dictionary<int, int>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        // No SaveChangesAsync call expected when there are no labelers to assign
    }

    [Fact]
    public async Task Handle_WithUnassignedLabelers_DistributesLabelers()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var assignmentId = 1;

        var assignment = new SubjectVideoGroupAssignment
        {
            Id = assignmentId,
            Labelers = new List<User>()
        };

        var subject = new Subject
        {
            SubjectVideoGroupAssignments = new List<SubjectVideoGroupAssignment> { assignment }
        };

        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Subjects = new List<Subject> { subject }
        };

        var unassignedLabelers = new List<User>
        {
            new() { Id = "labeler1", UserName = "Labeler 1" }
        };

        var command = new DistributeLabelersEquallyCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(project));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        _mediator.Send(Arg.Any<GetUnassignedLabelersOfProjectQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok(unassignedLabelers));

        _projectRepository.GetLabelerCountForAssignments(projectId, userId, false)
            .Returns(Task.FromResult(new Dictionary<int, int> { { assignmentId, 0 } }));

        _subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, false)
            .Returns(Task.FromResult<SubjectVideoGroupAssignment?>(assignment));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
