using ProjektGrupowy.API.SignalR;

namespace ProjektGrupowy.API.Services.Background;

public interface IReportGenerator
{
    Task GenerateAsync(int projectId);
}

public class ReportGenerator(IMessageService messageService) : IReportGenerator
{
    public async Task GenerateAsync(int projectId)
    {
    }
}