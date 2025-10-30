using System.ComponentModel.DataAnnotations;
using ProjektGrupowy.Domain.Enums;

namespace ProjektGrupowy.Application.DTOs.AccessCode;

/// <summary>
/// DTO for creating an access code request
/// </summary>
public class CreateAccessCodeRequest
{
    /// <summary>
    /// The identifier of the project for which the access code is being created.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    /// <summary>
    /// The expiration type of the access code.
    /// </summary>
    [Required]
    public AccessCodeExpiration Expiration { get; set; }

    /// <summary>
    /// Custom expiration time in hours. Required if Expiration is set to Custom.
    /// </summary>
    [Required]
    public int CustomExpiration { get; set; }
}