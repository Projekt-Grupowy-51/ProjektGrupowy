using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Events;

namespace ProjektGrupowy.Application.Features.ProjectReport.Commands.GenerateReport;

public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, Result<GeneratedReport>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<GenerateReportCommandHandler> _logger;

    public GenerateReportCommandHandler(
        IProjectRepository projectRepository,
        IProjectReportRepository reportRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        ILogger<GenerateReportCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<Result<GeneratedReport>> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating report for project {ProjectId}", request.ProjectId);

        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var reportResult = await WriteReportToFileAsync(project);
        if (reportResult.IsFailed)
        {
            _logger.LogError("Failed to generate report for project {ProjectId}: {Error}", request.ProjectId, reportResult.Errors);
            return reportResult;
        }

        var report = reportResult.Value;

        await _reportRepository.AddReportAsync(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Add SignalR event after save so we have the report ID
        report.AddReportGeneratedEvent();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Report generated successfully for project {ProjectId}", request.ProjectId);

        return Result.Ok(report);
    }

    private async Task<Result<GeneratedReport>> WriteReportToFileAsync(Project project)
    {
        try
        {
            var reportsDirectory = _configuration["Reports:RootDirectory"] ?? "reports";
            var directoryPath = Path.Combine(AppContext.BaseDirectory, reportsDirectory);

            Directory.CreateDirectory(directoryPath);

            var filename = $"{Guid.NewGuid():N}.report.json";
            var filePath = Path.Combine(directoryPath, filename);

            var json = ConvertProjectToJson(project);
            await File.WriteAllTextAsync(filePath, json);

            var report = GeneratedReport.Create(
                $"Summary report {DateTime.UtcNow:dd.MM.yyyy HH:mm}",
                filePath,
                project,
                project.CreatedById);

            return Result.Ok(report);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private string ConvertProjectToJson(Project project)
    {
        var json = new
        {
            ProjectId = project.Id,
            ProjectName = project.Name,
            ProjectDescription = project.Description,
            ProjectOwner = project.CreatedBy.UserName,
            Subjects = project.Subjects
            .Select(subject => new
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                VideoGroups = subject.Labels
                    .SelectMany(label => label.AssignedLabels)
                    .Where(label => label.Video != null)
                    .Select(label => new
                    {
                        LabelId = label.Label.Id,
                        LabelerId = label.CreatedBy.Id,
                        LabelerName = label.CreatedBy.UserName,
                        LabelName = label.Label.Name,
                        label.Start,
                        label.End,
                        label.Label.Type,
                        label.Label.ColorHex,
                        VideoPath = label.Video.Path,
                        VideoTitle = label.Video.Title,
                        VideoId = label.Video.Id,
                        VideoGroupName = label.Video.VideoGroup.Name
                    })
                    .GroupBy(l => l.VideoGroupName)
                    .Select(group => new
                    {
                        VideoGroupName = group.Key,
                        Videos = group
                            .GroupBy(l => l.VideoId)
                            .Select(videoGroup =>
                            {
                                var first = videoGroup.First();
                                return new
                                {
                                    first.VideoId,
                                    first.VideoPath,
                                    first.VideoTitle,
                                    Labels = videoGroup.Select(label => new
                                    {
                                        label.LabelId,
                                        label.LabelerId,
                                        label.LabelName,
                                        label.Start,
                                        label.End,
                                        label.Type,
                                        label.ColorHex
                                    }).ToList()
                                };
                            })
                            .ToList()
                    })
                    .ToList()
            })
        };

        return JsonConvert.SerializeObject(json, Formatting.Indented);
    }
}
