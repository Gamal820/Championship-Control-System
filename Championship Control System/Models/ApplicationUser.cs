using Microsoft.AspNetCore.Identity;

namespace Championship_Control_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<MatchEvent> Events { get; set; } = new List<MatchEvent>();
    }
}