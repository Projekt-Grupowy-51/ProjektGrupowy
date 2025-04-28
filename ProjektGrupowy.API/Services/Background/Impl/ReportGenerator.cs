using Newtonsoft.Json;
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
    public async Task GenerateAsync(int projectId)
    {
        logger.LogInformation("Generating report for project {ProjectId}", projectId);
        
        var projectOpt = await projectService.GetProjectAsync(projectId, false);
        
        var project = projectOpt.GetValueOrThrow();
        var projectOwnerId = project.OwnerId;
        
        var reportOpt = await WriteReportToFileAsync(project);
        if (reportOpt.IsFailure)
        {
            logger.LogError("Failed to generate report for project {ProjectId}: {Error}", projectId, reportOpt.GetErrorOrThrow());
            await messageService.SendErrorAsync(
                project.OwnerId, 
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
                project.OwnerId, 
                $"Failed to update project after report generation: {updateResult.GetErrorOrThrow()}");
            return;
        }

        await messageService.SendSuccessAsync(
            projectOwnerId, 
            "Report generation completed successfully.");
        
        await messageService.SendMessageAsync(
            projectOwnerId,
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
                Owner = project.Owner
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
        return "Test";
    }
}