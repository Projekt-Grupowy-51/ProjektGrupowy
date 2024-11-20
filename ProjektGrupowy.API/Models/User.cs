using Microsoft.AspNetCore.Identity;

namespace ProjektGrupowy.API.Models;

public class User : IdentityUser
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
}
