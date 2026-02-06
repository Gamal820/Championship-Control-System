using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public DateOnly? FoundationDate { get; set; }

    public string? Logo { get; set; }

    public string? Country { get; set; }

    public int? StadiumId { get; set; }
    public int? CoachId { get; set; }

    public virtual Coach? Coach { get; set; }

    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual Stadium? Stadium { get; set; }

    public virtual ICollection<TeamStanding> TeamStandings { get; set; } = new List<TeamStanding>();

    public virtual ICollection<Championship> Championships { get; set; } = new List<Championship>();
}
