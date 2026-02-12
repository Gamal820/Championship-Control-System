using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public DateTime? MatchDate { get; set; }

    public int? HomeGoals { get; set; }

    public int? AwayGoals { get; set; }

    public int? AvailableTicket { get; set; }

    public decimal? TicketPrice { get; set; }

    public string? Status { get; set; }

    public decimal? TicketPrice { get; set; }

    public int? StadiumId { get; set; }

    public int? ChampionshipId { get; set; }

    public int? HomeTeamId { get; set; }

    public int? AwayTeamId { get; set; }

    public virtual Team? AwayTeam { get; set; }

    public virtual Championship? Championship { get; set; }

    public virtual Team? HomeTeam { get; set; }

    public virtual ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();

    public virtual Stadium? Stadium { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
