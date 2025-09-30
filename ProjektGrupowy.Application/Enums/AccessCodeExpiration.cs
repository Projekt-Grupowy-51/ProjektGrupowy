namespace ProjektGrupowy.Application.Enums;

/// <summary>
/// Enum for access code expiration options
/// </summary>
public enum AccessCodeExpiration
{
    /// <summary>
    /// The access code expires in 14 days.
    /// </summary>
    In14Days = 0,
    
    /// <summary>
    /// The access code expires in 30 days.
    /// </summary>
    In30Days = 1,
    
    /// <summary>
    /// The access code never expires.
    /// </summary>
    Never = 2,
    
    /// <summary>
    /// The access code expires after a custom amount of time.
    /// </summary>
    Custom = 3
}