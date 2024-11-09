using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.Models;

public class User : IdentityUser
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
}
