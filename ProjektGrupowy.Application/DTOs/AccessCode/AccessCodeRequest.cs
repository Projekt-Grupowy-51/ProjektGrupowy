using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Application.DTOs.AccessCode;

public class AccessCodeRequest
{
    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string Code { get; set; }
}