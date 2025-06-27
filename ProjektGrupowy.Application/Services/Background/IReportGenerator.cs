namespace ProjektGrupowy.Application.Services.Background;

public interface IReportGenerator
{
    Task GenerateAsync(int projectId, string userId, bool isAdmin);
}