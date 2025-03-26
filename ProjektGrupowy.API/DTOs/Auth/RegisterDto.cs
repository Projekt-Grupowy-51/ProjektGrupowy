using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana!")]
    [StringLength(255, ErrorMessage = "Maksymalna długość nazwy projektu wynosi 255 znaków.")]
    public string UserName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}
