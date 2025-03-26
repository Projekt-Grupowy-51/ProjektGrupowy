using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProjektGrupowy.API.Models;

[Table("AssignedLabels")]
public class AssignedLabel
{
    [Key]
    public int Id { get; set; }
    
    public TimeSpan Start { get; set; }
    
    public TimeSpan End { get; set; }

    [Required]
    public virtual Label Label { get; set; }

    [Required]
    public virtual SubjectVideoGroupAssignment SubjectVideoGroupAssignment { get; set; }

    [Required]
    public virtual Labeler Labeler { get; set; }
    
    public string ToJson()
    {
        var jsonObject = new
        {
            start = Start.ToString(@"hh\:mm\:ss\.fff"),
            end = End.ToString(@"hh\:mm\:ss\.fff"),
            label = JsonSerializer.Deserialize<Label>(Label.ToJson())
        };

        return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}