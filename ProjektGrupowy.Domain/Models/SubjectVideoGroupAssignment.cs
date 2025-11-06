using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProjektGrupowy.Domain.Models;

// [Table("SubjectVideoGroupAssignments")]
public class SubjectVideoGroupAssignment : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }
    
    public DateOnly CreationDate { get; set; }
    
    public DateOnly? ModificationDate { get; set; }

    [Required]
    public virtual Subject Subject { get; set; } = default!;

    [Required]
    public virtual VideoGroup VideoGroup { get; set; } = default!;

    public virtual ICollection<User> Labelers { get; set; } = new List<User>();
    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = default!;
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

    public void ClearLabelers(string userId)
    {
        Labelers.Clear();
        AddDomainEvent("Labelowacze zostali odpisani z przypisania!", userId);
    }

    public void AddLabelers(IEnumerable<User> labelers, string userId)
    {
        foreach (var labeler in labelers)
        {
            if (!Labelers.Contains(labeler))
            {
                Labelers.Add(labeler);
            }
        }
        AddDomainEvent("Labelowacze zostali przypisani do przypisania!", userId);
    }

    public static SubjectVideoGroupAssignment Create(Subject subject, VideoGroup videoGroup, string createdById)
    {
        var assignment = new SubjectVideoGroupAssignment
        {
            Subject = subject,
            VideoGroup = videoGroup,
            CreationDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedById = createdById
        };
        assignment.AddDomainEvent("Przypisanie zostało dodane!", createdById);
        return assignment;
    }

    public void Update(Subject subject, VideoGroup videoGroup, string userId)
    {
        Subject = subject;
        VideoGroup = videoGroup;
        ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        AddDomainEvent("Przypisanie zostało zaktualizowane!", userId);
    }

    public void AssignLabeler(User labeler, string userId)
    {
        if (!Labelers.Contains(labeler))
        {
            Labelers.Add(labeler);
            AddDomainEvent("Labelowacz został przypisany!", userId);
        }
    }

    public void UnassignLabeler(User labeler, string userId)
    {
        if (Labelers.Contains(labeler))
        {
            Labelers.Remove(labeler);
            AddDomainEvent("Labelowacz został odpisany!", userId);
        }
    }
}