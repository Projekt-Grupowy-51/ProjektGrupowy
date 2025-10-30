using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

[Table("Projects")]
public class Project : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Podanie nazwy projektu jest wymagane.")]
    [StringLength(255, ErrorMessage = "Maksymalna długość nazwy projektu wynosi 255 znaków.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podanie opisu projektu jest wymagane.")]
    [StringLength(1000, ErrorMessage = "Maksymalna długość opisu projektu wynosi 1000 znaków.")]
    public string Description { get; set; } = string.Empty;
    
    public DateOnly CreationDate { get; set; }
    
    public DateOnly? ModificationDate { get; set; }
        
    public DateOnly? EndDate { get; set; }

    //public virtual ICollection<Video>? Videos { get; set; } = new List<Video>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public virtual ICollection<VideoGroup> VideoGroups { get; set; } = new List<VideoGroup>();
    public virtual ICollection<ProjectAccessCode> AccessCodes { get; set; } = new List<ProjectAccessCode>();

    // Many-to-Many Relation
    public virtual ICollection<User> ProjectLabelers { get; set; } = new List<User>();
    
    public virtual ICollection<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();

    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = default!;
    public DateTime? DelDate { get; set; } = null;

    public static Project Create(string name, string description, string createdById)
    {
        var project = new Project
        {
            Name = name,
            Description = description,
            CreatedById = createdById,
            CreationDate = DateOnly.FromDateTime(DateTime.Today)
        };
        project.AddDomainEvent("Projekt został dodany!", createdById);
        return project;
    }

    public void Update(string name, string description, bool finished, string userId)
    {
        Name = name;
        Description = description;
        ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        EndDate = finished ? DateOnly.FromDateTime(DateTime.Today) : null;
        AddDomainEvent("Projekt został zaktualizowany!", userId);
    }

    public void AddLabeler(User labeler, string userId)
    {
        if (!ProjectLabelers.Contains(labeler))
        {
            ProjectLabelers.Add(labeler);
            AddDomainEvent($"Użytkownik dołączył do projektu!", userId);
        }
    }
}
