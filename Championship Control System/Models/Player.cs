using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Nationality { get; set; }

    public string? Img { get; set; }

    public string? Position { get; set; }

    public int? ShirtNumber { get; set; }

    public int? TeamId { get; set; }

    public virtual Team? Team { get; set; }
}
