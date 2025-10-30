using MediatR;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.Interfaces.SignalR;

namespace ProjektGrupowy.Application.Events;

public class ReportGeneratedEventHandler : INotificationHandler<ReportGeneratedEvent>
{
    private readonly IMessageService _messageService;
    private readonly ILogger<ReportGeneratedEventHandler> _logger;

    public ReportGeneratedEventHandler(
        IMessageService messageService,
        ILogger<ReportGeneratedEventHandler> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    public async Task Handle(ReportGeneratedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Report generated event received for project {ProjectId}, report {ReportId}",
            notification.ProjectId, notification.ReportId);

        try
        {
            // Send SignalR notification to the user who generated the report
            await _messageService.SendToUserAsync(notification.UserId, "ReportGenerated", new
            {
                projectId = notification.ProjectId,
                reportId = notification.ReportId,
                timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("SignalR notification sent to user {UserId} for report {ReportId}",
                notification.UserId, notification.ReportId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR notification for report {ReportId}", notification.ReportId);
        }
    }
}