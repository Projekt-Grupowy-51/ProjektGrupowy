using System.ComponentModel.DataAnnotations;
using ProjektGrupowy.API.Enums;

namespace ProjektGrupowy.API.DTOs.AccessCode;

public class CreateAccessCodeRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    [Required]
    public AccessCodeExpiration Expiration { get; set; }

    [Required]
    public int CustomExpiration { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}