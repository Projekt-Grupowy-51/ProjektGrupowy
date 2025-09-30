using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Application.DTOs.AccessCode;

/// <summary>
/// DTO for access code request
/// </summary>
public class AccessCodeRequest
{
    /// <summary>
    /// The access code string (exactly 16 characters).
    /// </summary>
    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string Code { get; set; }
}