using ProjektGrupowy.API.SignalR;

namespace ProjektGrupowy.API.Services.Background.Impl;

public class ReportGenerator(
    IMessageService messageService,
    IProjectService projectService) : IReportGenerator
{
    public async Task GenerateAsync(int projectId)
    {
    }
}