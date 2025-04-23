using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProjektGrupowy.API.Models;

[Table("AssignedLabels")]
public class AssignedLabel : IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public virtual Label Label { get; set; }

    public string OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public virtual User Owner { get; set; }

    [Required]
    public virtual Video Video { get; set; }

    [Required]
    public string Start { get; set; }

    [Required]
    public string End { get; set; }

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
}