using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Championship
{
    public int ChampionshipId { get; set; }

    public string ChampionshipName { get; set; } = null!;

    public string? Season { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Country { get; set; }

    public string? Logo { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<TeamStanding> TeamStandings { get; set; } = new List<TeamStanding>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
