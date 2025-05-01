using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProjektGrupowy.API.Models;

[Table("SubjectVideoGroupAssignments")]
public class SubjectVideoGroupAssignment : IOwnedEntity
{
    [Key]
    public int Id { get; set; }
    
    public DateOnly CreationDate { get; set; }
    
    public DateOnly? ModificationDate { get; set; }

    [Required]
    public virtual Subject Subject { get; set; }

    [Required]
    public virtual VideoGroup VideoGroup { get; set; }

    public virtual ICollection<User>? Labelers { get; set; }
    public string CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; }
    public DateTime? DelDate { get; set; } = null;

    //public string ToJson()
    //{
    //    var jsonObject = Labelers?.Select(labeler => new
    //    {
    //        videoSetName = VideoGroup.Name,
    //        assigneeName = labeler.Name,
    //        eventList = AssignedLabels?
    //            .Where(assignedLabel => assignedLabel.Labeler == labeler)
    //            .Select(assignedLabel => JsonSerializer.Deserialize<AssignedLabel>(assignedLabel.ToJson()))
    //            .ToList()
    //    }).ToList();
    //
    //    return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
    //    {
    //        WriteIndented = true
    //    });
    //}
}