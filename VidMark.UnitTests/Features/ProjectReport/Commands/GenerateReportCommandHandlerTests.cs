using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VidMark.Application.Authorization;
using VidMark.Application.Features.ProjectReport.Commands.GenerateReport;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.UnitTests.Features.ProjectReport.Commands;

public class GenerateReportCommandHandlerTests
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<GenerateReportCommandHandler> _logger;
    private readonly GenerateReportCommandHandler _handler;

    public GenerateReportCommandHandlerTests()
    {
        _projectRepository = Substitute.For<IProjectRepository>();
        _reportRepository = Substitute.For<IProjectReportRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _configuration = Substitute.For<IConfiguration>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _logger = Substitute.For<ILogger<GenerateReportCommandHandler>>();
        _handler = new GenerateReportCommandHandler(
            _projectRepository,
            _reportRepository,
            _unitOfWork,
            _configuration,
            _currentUserService,
            _authorizationService,
            _logger);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFailure()
    {
        // Arrange
        var projectId = 1;
        var userId = "user123";
        var command = new GenerateReportCommand(projectId, userId, false);

        _projectRepository.GetProjectAsync(projectId, userId, false)
            .Returns(Task.FromResult<Project?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No project found!");
    }
}
