using VidMark.Domain.Models;

namespace VidMark.Infrastructure.SignalR;

public interface IAppHubClient
{
    Task NotifyFromBackendServerSuccess(MessageContent content);
    Task NotifyFromBackendServerError(MessageContent content);
    Task NotifyFromBackendServerWarning(MessageContent content);
    Task NotifyFromBackendServerInfo(MessageContent content);
    Task NotifyFromBackendServerMessage(MessageContent content);
    Task LabelersCountChanged(object data);
    Task ReportGenerated(object data);
    Task VideoProcessed(MessageContent content);
}
