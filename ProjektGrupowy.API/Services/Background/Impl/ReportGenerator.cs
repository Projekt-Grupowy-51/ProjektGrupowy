using ProjektGrupowy.API.SignalR;

namespace ProjektGrupowy.API.Services.Background;

public class ReportGenerator(IMessageService messageService) : IReportGenerator
{
    public async Task GenerateAsync(int projectId)
    {
    }
}