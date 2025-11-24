using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VidMark.Application.Configuration;
using VidMark.Application.Events;

namespace VidMark.Application.Pipelines;

/// <summary>
/// Pipeline behavior that publishes a notification to trigger outbox processing after each command/query execution.
/// This behavior is only active when OutboxSettings.ProcessingMode is set to "Pipeline".
/// </summary>
public class OutboxProcessingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IPublisher _publisher;
    private readonly OutboxSettings _outboxSettings;
    private readonly ILogger<OutboxProcessingBehavior<TRequest, TResponse>> _logger;

    public OutboxProcessingBehavior(
        IPublisher publisher,
        IOptions<OutboxSettings> outboxSettings,
        ILogger<OutboxProcessingBehavior<TRequest, TResponse>> logger)
    {
        _publisher = publisher;
        _outboxSettings = outboxSettings.Value;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Execute the command/query
        var response = await next();

        // Publish notification to trigger outbox processing only if configured for Pipeline mode
        if (_outboxSettings.IsPipelineMode)
        {
            try
            {
                await _publisher.Publish(new ProcessOutboxNotification(), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing outbox processing notification for request {RequestType}", typeof(TRequest).Name);
                // Don't throw - we don't want to fail the request because of event publishing issues
            }
        }

        return response;
    }
}
