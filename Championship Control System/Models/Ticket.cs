using System;
using System.Collections.Generic;

namespace Championship_Control_System.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string? SeatNumber { get; set; }

    public DateTime? BookingDate { get; set; }
    public decimal? TicketPrice { get; set; }

    public int? MatchId { get; set; }

    public string? UserId { get; set; }

    public virtual Match? Match { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
