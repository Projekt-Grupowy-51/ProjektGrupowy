using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Subjects.Commands.DeleteSubject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Subjects.Commands;

public class DeleteSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly DeleteSubjectCommandHandler _handler;

    public DeleteSubjectCommandHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new DeleteSubjectCommandHandler(_subjectRepository, _unitOfWork, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_SubjectExists_DeletesSubject()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var command = new DeleteSubjectCommand(subjectId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _subjectRepository.Received(1).DeleteSubject(subject);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SubjectNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var command = new DeleteSubjectCommand(subjectId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Failed to delete subject");
        _subjectRepository.DidNotReceive().DeleteSubject(Arg.Any<Subject>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var command = new DeleteSubjectCommand(subjectId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        _subjectRepository.DidNotReceive().DeleteSubject(Arg.Any<Subject>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
