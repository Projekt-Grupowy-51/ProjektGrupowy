namespace ProjektGrupowy.API.Services.Background;

public interface IReportGenerator
{
    Task GenerateAsync(int projectId, string userId, bool isAdmin);
}