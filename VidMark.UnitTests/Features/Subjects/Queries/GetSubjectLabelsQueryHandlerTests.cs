using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Subjects.Queries.GetSubjectLabels;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Subjects.Queries;

public class GetSubjectLabelsQueryHandlerTests
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetSubjectLabelsQueryHandler _handler;

    public GetSubjectLabelsQueryHandlerTests()
    {
        _subjectRepository = Substitute.For<ISubjectRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetSubjectLabelsQueryHandler(_subjectRepository, _currentUserService, _authorizationService);
    }

    [Fact]
    public async Task Handle_LabelsExist_ReturnsLabels()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var labels = new List<Label>
        {
            new() { Id = 1, Name = "Label 1" },
            new() { Id = 2, Name = "Label 2" }
        };
        var query = new GetSubjectLabelsQuery(subjectId, userId, false);

        _subjectRepository.GetSubjectLabelsAsync(subjectId, userId, false)
            .Returns(Task.FromResult(labels));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(labels);
    }

    [Fact]
    public async Task Handle_NoLabels_ReturnsEmptyList()
    {
        // Arrange
        var subjectId = 1;
        var userId = "user123";
        var labels = new List<Label>();
        var query = new GetSubjectLabelsQuery(subjectId, userId, false);

        _subjectRepository.GetSubjectLabelsAsync(subjectId, userId, false)
            .Returns(Task.FromResult(labels));

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
        var subjectId = 1;
        var userId = "user123";
        var labels = new List<Label> { new() { Id = 1, Name = "Label 1" } };
        var query = new GetSubjectLabelsQuery(subjectId, userId, false);

        _subjectRepository.GetSubjectLabelsAsync(subjectId, userId, false)
            .Returns(Task.FromResult(labels));

        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
