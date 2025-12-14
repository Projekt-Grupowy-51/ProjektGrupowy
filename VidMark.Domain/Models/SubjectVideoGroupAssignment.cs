using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace VidMark.Domain.Models;

public class SubjectVideoGroupAssignment : BaseEntity, IOwnedEntity
{
    
    public int Id { get; set; }
    
    public DateOnly CreationDate { get; set; }
    
    public DateOnly? ModificationDate { get; set; }

    
    public virtual Subject Subject { get; set; } = default!;

    
    public virtual VideoGroup VideoGroup { get; set; } = default!;

    public virtual ICollection<User> Labelers { get; set; } = new List<User>();
    public string CreatedById { get; set; } = string.Empty;

    
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
        AddDomainEvent(MessageContent.LabelersRemovedFromAssignment, userId);
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
        AddDomainEvent(MessageContent.LabelersAddedToAssignment, userId);
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
        assignment.AddDomainEvent(MessageContent.AssignmentCreated, createdById);
        return assignment;
    }

    public void Update(Subject subject, VideoGroup videoGroup, string userId)
    {
        Subject = subject;
        VideoGroup = videoGroup;
        ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        AddDomainEvent(MessageContent.AssignmentUpdated, userId);
    }

    public void AssignLabeler(User labeler, string userId)
    {
        if (!Labelers.Contains(labeler))
        {
            Labelers.Add(labeler);
            AddDomainEvent(MessageContent.LabelerAddedToAssignment, userId);
        }
    }

    public void UnassignLabeler(User labeler, string userId)
    {
        if (Labelers.Contains(labeler))
        {
            Labelers.Remove(labeler);
            AddDomainEvent(MessageContent.LabelerRemovedFromAssignment, userId);
        }
    }
}