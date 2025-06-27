using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Application.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
