using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Championship_Control_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        [StringLength(20)]
        public string? Gender { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<MatchEvent> Events { get; set; } = new List<MatchEvent>();
    }
}