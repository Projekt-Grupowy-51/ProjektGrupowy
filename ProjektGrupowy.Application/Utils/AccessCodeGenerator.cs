using ProjektGrupowy.Domain.Models;
using System.Security.Cryptography;

namespace ProjektGrupowy.Domain.Utils;

public static class AccessCodeGenerator
{
    // Generate a 16-character cryptographically secure random code
    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        var data = RandomNumberGenerator.GetBytes(16); // 16 characters
        
        return new string(data.Select(b => chars[b % chars.Length]).ToArray());
    }

    // Create a new ProjectAccessCode object with all required properties. Uses cryptographically secure random code.
    public static ProjectAccessCode Create(Project project, DateTime? expiresAtUtc = null)
    {
        return new ProjectAccessCode
        {
            Project = project, 
            Code = GenerateCode(),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc
        };
    }
}