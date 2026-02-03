using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class MatchEvent
{
    public int EventId { get; set; }

    public int? Minute { get; set; }

    public string? EventType { get; set; }

    public int? MatchId { get; set; }

    public virtual Match? Match { get; set; }

    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
