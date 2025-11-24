namespace ProjektGrupowy.API.DTOs.User;

/// <summary>
/// A simple DTO representing a user with basic information.
/// </summary>
/// <param name="userId">The unique identifier of the user - a GUID string.</param>
/// <param name="userName">The name of the user, which can be null.</param>
/// <param name="roles">A collection of roles assigned to the user.</param>
/// <param name="isAdmin">Indicates whether the user has administrative privileges.</param>
public class SimpleUserDto(string userId, string? userName, IEnumerable<string> roles, bool isAdmin)
{
    /// <summary>
    /// The unique identifier of the user - a GUID string.
    /// </summary>
    public string UserId { get; init; } = userId;
    
    /// <summary>
    /// The name of the user, which can be null.
    /// </summary>
    public string? UserName { get; init; } = userName;
    
    /// <summary>
    /// A collection of roles assigned to the user.
    /// </summary>
    public IEnumerable<string> Roles { get; init; } = roles;
    
    /// <summary>
    /// Indicates whether the user has administrative privileges.
    /// </summary>
    public bool IsAdmin { get; init; } = isAdmin;
}