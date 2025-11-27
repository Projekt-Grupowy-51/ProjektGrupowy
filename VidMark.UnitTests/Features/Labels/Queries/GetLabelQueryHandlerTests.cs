using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Labels.Queries.GetLabel;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.Labels.Queries;

public class GetLabelQueryHandlerTests
{
    private readonly ILabelRepository _labelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetLabelQueryHandler _handler;

    public GetLabelQueryHandlerTests()
    {
        _labelRepository = Substitute.For<ILabelRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetLabelQueryHandler(
            _labelRepository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_LabelExists_ReturnsLabel()
    {
        // Arrange
        var labelId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var query = new GetLabelQuery(labelId, userId, false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(label);
        await _labelRepository.Received(1).GetLabelAsync(labelId, userId, false);
    }

    [Fact]
    public async Task Handle_LabelNotFound_ReturnsFailure()
    {
        // Arrange
        var labelId = 1;
        var userId = "user123";
        var query = new GetLabelQuery(labelId, userId, false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Label does not exist");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var labelId = 1;
        var userId = "user123";
        var label = new Label { Id = labelId };
        var query = new GetLabelQuery(labelId, userId, false);

        _labelRepository.GetLabelAsync(labelId, userId, false)
            .Returns(Task.FromResult<Label?>(label));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
