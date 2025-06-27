namespace ProjektGrupowy.Application.DTOs.AccessCode;

public class AccessCodeResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Code { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public bool IsValid { get; set; }
}