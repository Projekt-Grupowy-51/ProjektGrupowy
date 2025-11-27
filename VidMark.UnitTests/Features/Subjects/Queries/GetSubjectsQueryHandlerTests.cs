using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Subjects.Queries.GetSubjects;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Subjects.Queries;

public class GetSubjectsQueryHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetSubjectsQueryHandler _handler;

    public GetSubjectsQueryHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetSubjectsQueryHandler(_subjectRepository, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_SubjectsExist_ReturnsSubjects()
    {
        // Arrange
        var userId = "user123";
        var subjects = new List<Subject>
        {
            new() { Id = 1, Name = "Subject 1" },
            new() { Id = 2, Name = "Subject 2" }
        };
        var query = new GetSubjectsQuery(userId, false);

        _subjectRepository.GetSubjectsAsync(userId, false)
            .Returns(Task.FromResult(subjects));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(subjects);
    }

    [Fact]
    public async Task Handle_NoSubjects_ReturnsEmptyList()
    {
        // Arrange
        var userId = "user123";
        var subjects = new List<Subject>();
        var query = new GetSubjectsQuery(userId, false);

        _subjectRepository.GetSubjectsAsync(userId, false)
            .Returns(Task.FromResult(subjects));

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
        var subjects = new List<Subject> { new() { Id = 1, Name = "Subject 1" } };
        var query = new GetSubjectsQuery(userId, false);

        _subjectRepository.GetSubjectsAsync(userId, false)
            .Returns(Task.FromResult(subjects));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
