using MediatR;

namespace ProjektGrupowy.Application.Events;

/// <summary>
/// Notification to trigger processing of unpublished domain events from the outbox.
/// Published by OutboxProcessingBehavior after each command/query execution when configured for Pipeline mode.
/// </summary>
public class ProcessOutboxNotification : INotification
{
}
