using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string Username { get; set; } = null!;

    public string? Role { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<MatchEvent> Events { get; set; } = new List<MatchEvent>();
}
