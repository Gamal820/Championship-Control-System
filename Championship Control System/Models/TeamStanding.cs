using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class TeamStanding
{
    public int StandingId { get; set; }

    public int? Played { get; set; }

    public int? Won { get; set; }

    public int? Draw { get; set; }

    public int? Lost { get; set; }

    public int? GoalDifference { get; set; }

    public int? TeamId { get; set; }

    public int? ChampionshipId { get; set; }

    public virtual Championship? Championship { get; set; }

    public virtual Team? Team { get; set; }
}
