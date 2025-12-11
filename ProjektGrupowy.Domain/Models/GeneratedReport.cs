using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

public class GeneratedReport : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string Path { get; set; } = string.Empty;
    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = default!;

    public virtual Project Project { get; set; } = default!;
    public DateTime? DelDate { get; set; }

    public Stream ToStream()
    {
        try
        {
            return new FileStream(Path, FileMode.Open);
        }
        catch (Exception)
        {
            return new MemoryStream();
        }
    }

    public static GeneratedReport Create(string name, string path, Project project, string createdById)
    {
        var report = new GeneratedReport
        {
            Name = name,
            Path = path,
            Project = project,
            CreatedById = createdById,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Add user notification
        report.AddDomainEvent("Raport został wygenerowany!", createdById);

        return report;
    }

    public void AddReportGeneratedEvent()
    {
        // Add typed event for SignalR refresh after report is saved and has an ID
        this.AddTypedDomainEvent(
            "Report generated",
            CreatedById,
            "ReportGeneratedEvent",
            new { ProjectId = Project.Id, ReportId = Id }
        );
    }
}