using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Domain.Models;

// [Table("Subjects")]
public class Subject : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "wymagane")]
    [StringLength(255, ErrorMessage = "max 255 znakow")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public virtual Project Project { get; set; } = default!;

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    public virtual ICollection<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = default!;
    public DateTime? DelDate { get; set; } = null;

    public static Subject Create(string name, string description, Project project, string createdById)
    {
        var subject = new Subject
        {
            Name = name,
            Description = description,
            Project = project,
            CreatedById = createdById
        };
        subject.AddDomainEvent("Temat został dodany!", createdById);
        return subject;
    }

    public void Update(string name, string description, Project project, string userId)
    {
        Name = name;
        Description = description;
        Project = project;
        AddDomainEvent("Temat został zaktualizowany!", userId);
    }
}
