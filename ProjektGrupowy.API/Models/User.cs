using Microsoft.AspNetCore.Identity;

namespace ProjektGrupowy.API.Models;

public class User : IdentityUser
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public virtual Scientist? Scientist { get; set; }
    public virtual Labeler? Labeler { get; set; }
}
