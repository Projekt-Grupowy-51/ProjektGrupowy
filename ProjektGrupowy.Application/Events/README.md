# Event-Based Domain Event Publishing

This document describes the real-time event publishing architecture that replaced the polling-based approach.

## Architecture Overview

### Previous Approach (Polling)
- Domain events were stored in the database
- A Hangfire job polled the `DomainEvents` table every 10 seconds
- Events were published to clients with 5-10 second delays

### New Approach (Event-Driven)
- Domain events are still stored in the database (for reliability)
- Events are published **immediately** after `SaveChangesAsync` completes
- MediatR pipeline processes events in real-time
- Zero polling delay - messages are delivered instantly

## Flow Diagram

```
Entity.AddDomainEvent()
    ↓
AppDbContext.SaveChangesAsync()
    ↓
DispatchDomainEventsAsync() → Collects domain events from entities
    ↓
base.SaveChangesAsync() → Saves events to database
    ↓
IMediator.Publish(DomainEventSavedNotification) → Publishes immediately
    ↓
DomainEventSavedNotificationHandler.Handle()
    ↓
    ├→ DomainEventNotification (for legacy events)
    └→ Typed events (ReportGeneratedEvent, etc.)
        ↓
    SignalR Hub → Sends to clients in real-time
```

## Key Components

### 1. DomainEventSavedNotification
**File:** `ProjektGrupowy.Application/Events/DomainEventSavedNotification.cs`

A MediatR notification that carries a list of domain events that were just saved to the database.

### 2. DomainEventSavedNotificationHandler
**File:** `ProjektGrupowy.Application/Events/DomainEventSavedNotificationHandler.cs`

Handles the notification by:
- Processing each domain event
- Publishing typed events (like `ReportGeneratedEvent`)
- Publishing legacy notification events
- Marking events as published
- Saving the published status

### 3. AppDbContext
**File:** `ProjektGrupowy.Infrastructure/Persistance/AppDbContext.cs`

Modified to:
- Inject `IMediator` via constructor
- Collect domain events before save
- Save domain events to database
- Publish `DomainEventSavedNotification` immediately after successful save

## Benefits

1. **Real-time delivery**: Messages are delivered instantly (< 100ms instead of 5-10 seconds)
2. **Better user experience**: Users see notifications immediately
3. **Reduced database load**: No constant polling
4. **Reliability**: Events are still persisted to database
5. **Fallback mechanism**: Hangfire job runs every 30 seconds to catch any missed events

## Fallback Mechanism

The Hangfire job `publish-domain-events-fallback` runs every 30 seconds to:
- Find any unpublished events (where `IsPublished = false`)
- Publish them through the normal pipeline
- Mark them as published

This ensures reliability in case:
- The MediatR publish fails
- A transaction is rolled back
- Any unexpected error occurs

## Configuration

### Hangfire Job Configuration
**File:** `ProjektGrupowy.Application/IoC/ServiceCollectionExtensions.cs`

```csharp
RecurringJob.AddOrUpdate<IDomainEventPublisher>(
    "publish-domain-events-fallback",
    publisher => publisher.PublishPendingEventsAsync(),
    "*/30 * * * * *" // Every 30 seconds (fallback only)
);
```

To adjust the fallback interval, modify the cron expression:
- `"*/30 * * * * *"` = Every 30 seconds
- `"*/60 * * * * *"` = Every 60 seconds
- `"0 * * * * *"` = Every minute

### Disabling Fallback

If you want to completely remove the polling mechanism:

```csharp
public static void RegisterHangfireJobs()
{
    // Domain events are now published immediately via MediatR
    // No fallback needed
}
```

## Adding New Event Types

To add a new typed event:

1. Create the event class (implements `INotification`)
2. Create a handler (implements `INotificationHandler<YourEvent>`)
3. Add the event type to `DomainEventSavedNotificationHandler.PublishTypedEventAsync()`

Example:

```csharp
case nameof(VideoUploadedEvent):
    var videoEvent = JsonSerializer.Deserialize<VideoUploadedEventData>(domainEvent.EventData!);
    if (videoEvent != null)
    {
        await _mediator.Publish(new VideoUploadedEvent(
            videoEvent.VideoId,
            domainEvent.UserId
        ), cancellationToken);
    }
    break;
```

## Testing

To verify the real-time publishing works:

1. Trigger an action that creates a domain event (e.g., delete a project)
2. Check browser console/network tab for SignalR messages
3. Messages should appear within milliseconds
4. Check logs for: "Processing {Count} domain events in real-time"

## Monitoring

Key log messages to monitor:

- `Processing {Count} domain events in real-time` - Events being published immediately
- `Successfully processed {Count} domain events` - Successful processing
- `Error occurred while processing domain events in real-time` - Processing failures (events will be picked up by fallback)

## Troubleshooting

### Events not being delivered immediately

1. Check that `IMediator` is injected into `AppDbContext`
2. Verify `DomainEventSavedNotificationHandler` is registered (it should auto-register via MediatR)
3. Check logs for errors in the handler

### Events being delivered twice

1. Check that the fallback job is running at the correct interval (30 seconds)
2. Verify events are being marked as published (`IsPublished = true`)

### Events not being persisted

1. Check that `DispatchDomainEventsAsync()` is being called
2. Verify `base.SaveChangesAsync()` is called after collecting events
3. Check database for entries in `DomainEvents` table

