using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

// [Table("VideoGroups")]
public class VideoGroup : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();

    [Required]
    public virtual Project Project { get; set; } = default!;

    public virtual ICollection<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
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
        videoGroup.AddDomainEvent("Grupa wideo została dodana!", createdById);
        return videoGroup;
    }

    public void Update(string name, string description, Project project, string userId)
    {
        Name = name;
        Description = description;
        Project = project;
        AddDomainEvent("Grupa wideo została zaktualizowana!", userId);
    }
}