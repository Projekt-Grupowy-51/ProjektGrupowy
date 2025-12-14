using VidMark.Domain.Enums;
using System.Security.Cryptography;

namespace VidMark.Domain.Models;

public class ProjectAccessCode : BaseEntity, IOwnedEntity
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;

    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; } = null;

    public bool IsExpired => CreatedAtUtc switch
    {
        _ when ExpiresAtUtc is null => false,
        _ => DateTime.UtcNow > ExpiresAtUtc
    };

    public bool IsValid => !IsExpired;

    public void Retire(string userId)
    {
        ExpiresAtUtc = DateTime.UtcNow;
        AddDomainEvent(MessageContent.AccessCodeRetired, userId);
    }
    public string CreatedById { get; set; } = string.Empty;

    
    public virtual User CreatedBy { get; set; } = default!;

    public DateTime? DelDate { get; set; } = null;

    private static DateTime? GetExpirationDate(AccessCodeExpiration expiration, int days = -1)
    {
        return expiration switch
        {
            AccessCodeExpiration.In14Days => DateTime.Today.AddDays(14).ToUniversalTime(),
            AccessCodeExpiration.In30Days => DateTime.Today.AddDays(30).ToUniversalTime(),
            AccessCodeExpiration.Never => null,
            AccessCodeExpiration.Custom when days > 0 => DateTime.Today.AddDays(days).ToUniversalTime(),
            _ => throw new ArgumentException("Invalid expiration or days value")
        };
    }

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        var data = RandomNumberGenerator.GetBytes(16); // 16 characters

        return new string(data.Select(b => chars[b % chars.Length]).ToArray());
    }

    public static ProjectAccessCode Create(Project project, AccessCodeExpiration expiration, string createdById, int customDays = -1)
    {
        var accessCode = new ProjectAccessCode
        {
            Project = project,
            Code = GenerateCode(),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = GetExpirationDate(expiration, customDays),
            CreatedById = createdById
        };
        accessCode.AddDomainEvent(MessageContent.AccessCodeCreated, createdById);
        return accessCode;
    }
}