using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

[Table("Projects")]
public class Project : IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Podanie nazwy projektu jest wymagane.")]
    [StringLength(255, ErrorMessage = "Maksymalna długość nazwy projektu wynosi 255 znaków.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Podanie opisu projektu jest wymagane.")]
    [StringLength(1000, ErrorMessage = "Maksymalna długość opisu projektu wynosi 1000 znaków.")]
    public string Description { get; set; }
    
    public DateOnly CreationDate { get; set; }
    
    public DateOnly? ModificationDate { get; set; }
        
    public DateOnly? EndDate { get; set; }

    //public virtual ICollection<Video>? Videos { get; set; } = new List<Video>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public virtual ICollection<ProjectAccessCode> AccessCodes { get; set; } = new List<ProjectAccessCode>();

    // Many-to-Many Relation
    public virtual ICollection<User> ProjectLabelers { get; set; } = new List<User>();
    
    public virtual ICollection<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();

    public string CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; }
    public DateTime? DelDate { get; set; } = null;
}
