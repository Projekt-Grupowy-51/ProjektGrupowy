using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Subjects.Queries.GetSubject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Subjects.Queries;

public class GetSubjectQueryHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetSubjectQueryHandler _handler;

    public GetSubjectQueryHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetSubjectQueryHandler(_subjectRepository, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_SubjectExists_ReturnsSubject()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var query = new GetSubjectQuery(subjectId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(subject);
        await _subjectRepository.Received(1).GetSubjectAsync(subjectId, userId, false);
    }

    [Fact]
    public async Task Handle_SubjectNotFound_ReturnsFailure()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var query = new GetSubjectQuery(subjectId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No subject found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var subject = new Subject { Id = subjectId, Name = "Test Subject" };
        var query = new GetSubjectQuery(subjectId, userId, false);

        _subjectRepository.GetSubjectAsync(subjectId, userId, false)
            .Returns(Task.FromResult<Subject?>(subject));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
