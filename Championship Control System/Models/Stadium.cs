using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Stadium
{
    public int StadiumId { get; set; }

    public string StadiumName { get; set; } = null!;

    public string? City { get; set; }

    public int? Capacity { get; set; }

    public virtual Team? Team { get; set; }
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

}
