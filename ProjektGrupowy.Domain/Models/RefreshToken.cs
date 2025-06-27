using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = default!;

        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = default!;

        [Required]
        public DateTime Expires { get; set; }
        public bool IsUsed { get; set; } = false;

        public bool IsRevoked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
