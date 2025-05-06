using System.Text;
using Newtonsoft.Json;
using NuGet.Protocol;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Background.Impl;

public class ReportGenerator(
    IMessageService messageService,
    IProjectService projectService,
    IConfiguration configuration,
    ILogger<ReportGenerator> logger) : IReportGenerator
{
    public async Task GenerateAsync(int projectId, string userId, bool isAdmin)
    {
        logger.LogInformation("Generating report for project {ProjectId}", projectId);
        
        var projectOpt = await projectService.GetProjectAsync(projectId, userId, isAdmin);
        
        var project = projectOpt.GetValueOrThrow();

        
        var reportOpt = await WriteReportToFileAsync(project);
        if (reportOpt.IsFailure)
        {
            logger.LogError("Failed to generate report for project {ProjectId}: {Error}", projectId, reportOpt.GetErrorOrThrow());
            await messageService.SendErrorAsync(
                userId, 
                $"Failed to generate report: {reportOpt.GetErrorOrThrow()}");
            return;
        }
        
        var report = reportOpt.GetValueOrThrow();

        project.GeneratedReports.Add(report);
        var updateResult = await projectService.UpdateProjectAsync(project);

        if (updateResult.IsFailure)
        {
            logger.LogError("Failed to update project {ProjectId} after report generation: {Error}", projectId, updateResult.GetErrorOrThrow());
            await messageService.SendErrorAsync(
                userId, 
                $"Failed to update project after report generation: {updateResult.GetErrorOrThrow()}");
            return;
        }

        await messageService.SendSuccessAsync(
            userId, 
            "Report generation completed successfully.");
        
        await messageService.SendMessageAsync(
            userId,
            MessageTypes.ReportGenerated,
            "");
    }

    private async Task<Optional<GeneratedReport>> WriteReportToFileAsync(Project project)
    {
        try
        {
            var reportsDirectory = configuration["Reports:RootDirectory"] ?? "reports";
            var directoryPath = Path.Combine(AppContext.BaseDirectory, reportsDirectory);

            Directory.CreateDirectory(directoryPath);

            var filename = $"{Guid.NewGuid():N}.report.json";
            var filePath = Path.Combine(directoryPath, filename);

            var json = ConvertProjectToJson(project);
            await File.WriteAllTextAsync(filePath, json);

            var report = new GeneratedReport
            {
                Path = filePath,
                CreatedBy = project.CreatedBy,
                Name = $"Summary report {DateTime.UtcNow:dd.MM.yyyy HH:mm}",
            };

            return Optional<GeneratedReport>.Success(report);
        }
        catch (Exception e)
        {
            return Optional<GeneratedReport>.Failure(e.Message);
        }
    }

    private string ConvertProjectToJson(Project project)
    {
        var json = new {
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