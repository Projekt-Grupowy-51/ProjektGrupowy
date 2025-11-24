using MediatR;

namespace VidMark.Application.Events;

public record ReportGeneratedEvent(int ProjectId, int ReportId, string UserId) : INotification;