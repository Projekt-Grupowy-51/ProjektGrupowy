using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.ProjectReport.Queries.GetReport;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.ProjectReport.Queries;

public class GetReportQueryHandlerTests
{
    private readonly IProjectReportRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly GetReportQueryHandler _handler;

    public GetReportQueryHandlerTests()
    {
        _repository = Substitute.For<IProjectReportRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _handler = new GetReportQueryHandler(
            _repository,
            _currentUserService,
            _authorizationService);
    }

    [Fact]
    public async Task Handle_ReportExists_ReturnsReport()
    {
        // Arrange
        var reportId = 1;
        var userId = "user123";
        var report = new GeneratedReport { Id = reportId };
        var query = new GetReportQuery(reportId, userId, false);

        _repository.GetReportAsync(reportId, userId, false)
            .Returns(Task.FromResult<GeneratedReport?>(report));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(report);
        await _repository.Received(1).GetReportAsync(reportId, userId, false);
    }

    [Fact]
    public async Task Handle_ReportNotFound_ReturnsFailure()
    {
        // Arrange
        var reportId = 1;
        var userId = "user123";
        var query = new GetReportQuery(reportId, userId, false);

        _repository.GetReportAsync(reportId, userId, false)
            .Returns(Task.FromResult<GeneratedReport?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Report not found");
    }

    [Fact]
    public async Task Handle_UnauthorizedAccess_ThrowsForbiddenException()
    {
        // Arrange
        var reportId = 1;
        var userId = "user123";
        var report = new GeneratedReport { Id = reportId };
        var query = new GetReportQuery(reportId, userId, false);

        _repository.GetReportAsync(reportId, userId, false)
            .Returns(Task.FromResult<GeneratedReport?>(report));
        _authorizationService.AuthorizeAsync(default, default, default(IEnumerable<IAuthorizationRequirement>))
            .ReturnsForAnyArgs(Task.FromResult(AuthorizationResult.Failed()));

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
