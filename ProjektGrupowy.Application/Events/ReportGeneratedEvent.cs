using MediatR;

namespace ProjektGrupowy.Application.Events;

public record ReportGeneratedEvent(int ProjectId, int ReportId, string UserId) : INotification;