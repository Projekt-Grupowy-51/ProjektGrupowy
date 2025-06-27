using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

[Table("ProjectAccessCodes")]
public class ProjectAccessCode : IOwnedEntity
{
    [Key] public int Id { get; set; }
    public virtual Project Project { get; set; }
    [StringLength(16, MinimumLength = 16)] public string Code { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; } = null;

    [NotMapped]
    public bool IsExpired => CreatedAtUtc switch
    {
        _ when ExpiresAtUtc is null => false,
        _ => DateTime.UtcNow > ExpiresAtUtc
    };

    [NotMapped]
    public bool IsValid => !IsExpired;

    public void Retire() => ExpiresAtUtc = DateTime.UtcNow;
    public string CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; }

    public DateTime? DelDate { get; set; } = null;
}