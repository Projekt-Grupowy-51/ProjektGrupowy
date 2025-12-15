using System.ComponentModel.DataAnnotations;

namespace VidMark.Domain.Models;

public class VideoGroup : BaseEntity, IOwnedEntity
{
    
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();

    
    public virtual Project Project { get; set; } = default!;

    public virtual ICollection<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public string CreatedById { get; set; } = string.Empty;

    
    public virtual User CreatedBy { get; set; } = default!;
    public DateTime? DelDate { get; set; } = null;

    public static VideoGroup Create(string name, string description, Project project, string createdById)
    {
        var videoGroup = new VideoGroup
        {
            Name = name,
            Description = description,
            Project = project,
            CreatedById = createdById
        };
        videoGroup.AddDomainEvent(MessageContent.VideoGroupCreated, createdById);
        return videoGroup;
    }

    public void Update(string name, string description, Project project, string userId)
    {
        Name = name;
        Description = description;
        Project = project;
        AddDomainEvent(MessageContent.VideoGroupUpdated, userId);
    }
}