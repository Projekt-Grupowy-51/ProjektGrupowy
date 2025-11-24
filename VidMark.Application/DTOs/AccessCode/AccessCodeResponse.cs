namespace VidMark.Application.DTOs.AccessCode;

/// <summary>
/// DTO representing an access code response - simple representation of an access code
/// </summary>
public class AccessCodeResponse
{
    /// <summary>
    /// Unique identifier for the access code.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Identifier of the project associated with the access code.
    /// </summary>
    public int ProjectId { get; set; }
    
    /// <summary>
    /// The actual access code string.
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// The UTC timestamp when the access code was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
    
    /// <summary>
    /// The UTC timestamp when the access code expires. Null if it does not expire.
    /// </summary>
    public DateTime? ExpiresAtUtc { get; set; }
    
    /// <summary>
    /// Indicates whether the access code is currently active.
    /// </summary>
    public bool IsValid { get; set; }
}