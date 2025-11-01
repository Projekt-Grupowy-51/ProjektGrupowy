using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using System.Text.Json;

namespace ProjektGrupowy.Domain.Models;

// [Table("AssignedLabels")]
public class AssignedLabel : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public virtual Label Label { get; set; } = default!;

    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = default!;

    [Required]
    public virtual Video Video { get; set; } = default!;

    [Required]
    public string Start { get; set; } = string.Empty;

    [Required]
    public string End { get; set; } = string.Empty;

    public DateTime InsDate { get; set; } = DateTime.UtcNow;
    public DateTime? DelDate { get; set; } = null;

    public string ToJson()
    {
        var jsonObject = new
        {
            start = Start,
            end = End,
            label = JsonSerializer.Deserialize<Label>(Label.ToJson())
        };

        return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public static AssignedLabel Create(Label label, string userId, Video video, string start, string end)
    {
        var assignedLabel = new AssignedLabel
        {
            Label = label,
            CreatedById = userId,
            Video = video,
            Start = start,
            End = end
        };
        assignedLabel.AddDomainEvent("Przypisana etykieta została dodana!", userId);
        return assignedLabel;
    }
}