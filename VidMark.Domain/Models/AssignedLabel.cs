using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Text.Json;

namespace VidMark.Domain.Models;

public class AssignedLabel : BaseEntity, IOwnedEntity
{
    
    public int Id { get; set; }

    
    public virtual Label Label { get; set; } = default!;

    public string CreatedById { get; set; } = string.Empty;

    
    public virtual User CreatedBy { get; set; } = default!;

    
    public virtual Video Video { get; set; } = default!;

    
    public string Start { get; set; } = string.Empty;

    
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
        assignedLabel.AddDomainEvent(MessageContent.AssignedLabelAdded, userId);
        return assignedLabel;
    }
}