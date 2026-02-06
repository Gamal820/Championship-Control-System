using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Coach
{
    public int CoachId { get; set; }

    public string? Name { get; set; }

    public DateOnly? BirthData { get; set; }

    public string? Img { get; set; }

    public int? TeamId { get; set; }

    public virtual Team? Team { get; set; }
}
