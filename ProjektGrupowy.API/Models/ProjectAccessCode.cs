using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("ProjectAccessCodes")]
public class ProjectAccessCode
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
}