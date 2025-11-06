namespace ProjektGrupowy.Application.Configuration;

public class OutboxSettings
{
    public const string SectionName = "Outbox";

    /// <summary>
    /// Określa sposób przetwarzania outbox:
    /// - "Cron" - przetwarzanie przez Hangfire job
    /// - "Pipeline" - przetwarzanie przez MediatR pipeline behavior
    /// </summary>
    public string ProcessingMode { get; set; } = "Pipeline";

    public bool IsCronMode => ProcessingMode.Equals("Cron", StringComparison.OrdinalIgnoreCase);
    public bool IsPipelineMode => ProcessingMode.Equals("Pipeline", StringComparison.OrdinalIgnoreCase);
}
