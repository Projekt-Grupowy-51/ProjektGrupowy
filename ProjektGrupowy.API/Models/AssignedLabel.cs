using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProjektGrupowy.API.Models;

[Table("AssignedLabels")]
public class AssignedLabel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public virtual Label Label { get; set; }

    [Required]
    public virtual Labeler Labeler { get; set; }

    [Required]
    public virtual Video Video { get; set; }

    [Required]
    public string Start { get; set; }

    [Required]
    public string End { get; set; }

    public DateTime InsDate { get; set; } = DateTime.UtcNow;

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