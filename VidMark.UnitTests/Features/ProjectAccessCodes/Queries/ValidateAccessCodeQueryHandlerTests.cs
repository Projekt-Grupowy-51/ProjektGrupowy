using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.ProjectAccessCodes.Queries.ValidateAccessCode;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.ProjectAccessCodes.Queries;

public class ValidateAccessCodeQueryHandlerTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectAccessCodeRepository _projectAccessCodeRepository;
    private readonly ValidateAccessCodeQueryHandler _handler;

    public ValidateAccessCodeQueryHandlerTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _projectAccessCodeRepository = Substitute.For<IProjectAccessCodeRepository>();
        _handler = new ValidateAccessCodeQueryHandler(
            _authorizationService,
            _currentUserService,
            _projectAccessCodeRepository);
    }

    [Fact]
    public async Task Handle_ValidAccessCode_ReturnsIsValid()
    {
        // Arrange
        var code = "ABC123";
        var userId = "user123";
        var accessCode = new ProjectAccessCode { Code = code };
        var query = new ValidateAccessCodeQuery(code, userId, false);

        _projectAccessCodeRepository.GetAccessCodeByCodeAsync(code, userId, false)
            .Returns(Task.FromResult<ProjectAccessCode?>(accessCode));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AccessCodeExists_ReturnsIsValid()
    {
        // Arrange
        var code = "ABC123";
        var userId = "user123";
        var accessCode = new ProjectAccessCode { Code = code };
        var query = new ValidateAccessCodeQuery(code, userId, false);

        _projectAccessCodeRepository.GetAccessCodeByCodeAsync(code, userId, false)
            .Returns(Task.FromResult<ProjectAccessCode?>(accessCode));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentAccessCode_ReturnsFalse()
    {
        // Arrange
        var code = "ABC123";
        var userId = "user123";
        var query = new ValidateAccessCodeQuery(code, userId, false);

        _projectAccessCodeRepository.GetAccessCodeByCodeAsync(code, userId, false)
            .Returns(Task.FromResult<ProjectAccessCode?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var code = "ABC123";
        var userId = "user123";
        var accessCode = new ProjectAccessCode { Code = code };
        var query = new ValidateAccessCodeQuery(code, userId, false);

        _projectAccessCodeRepository.GetAccessCodeByCodeAsync(code, userId, false)
            .Returns(Task.FromResult<ProjectAccessCode?>(accessCode));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
