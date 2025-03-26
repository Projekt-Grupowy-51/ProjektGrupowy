using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.AccessCode;

public class AccessCodeRequest
{
    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string Code { get; set; }
}