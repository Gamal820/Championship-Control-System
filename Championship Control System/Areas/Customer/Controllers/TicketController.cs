using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TicketController : Controller
    {
        private readonly IRepository<Match> _matchRepo;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(
            IRepository<Match> matchRepo,
            IRepository<Ticket> ticketRepo,
            UserManager<ApplicationUser> userManager)
        {
            _matchRepo = matchRepo;
            _ticketRepo = ticketRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var matches = await _matchRepo.GetAsync(
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Include(m => m.Stadium),
                cancellationToken: cancellationToken
            );

            matches = matches.Where(m => m.Status == MatchStatus.Scheduled).ToList();
            return View(matches);
        }

        // Displays tickets purchased by the user
        public async Task<IActionResult> MyTickets(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var tickets = (await _ticketRepo.GetAsync(
                t => t.UserId == user.Id,
                include: q => q
                    .Include(t => t.Match).ThenInclude(m => m.HomeTeam)
                    .Include(t => t.Match).ThenInclude(m => m.AwayTeam)
                    .Include(t => t.Match).ThenInclude(m => m.Stadium),
                cancellationToken: cancellationToken
            )).ToList();

            var grouped = tickets
                .GroupBy(t => t.MatchId)
                .Select(g =>
                {
                    var first = g.First();
                    var match = first.Match;

                    return new MyTicketGroupVM
                    {
                        MatchId = g.Key,
                        MatchName = $"{match?.HomeTeam?.TeamName} vs {match?.AwayTeam?.TeamName}",
                        StadiumName = match?.Stadium?.StadiumName ?? "-",
                        UnitPrice = first.TicketPrice ?? 0m,        // حسب نوع TicketPrice عندك
                        Quantity = g.Count(),
                        LastBookingDate = g.Max(x => x.BookingDate)
                    };
                })
                .OrderByDescending(x => x.LastBookingDate)
                .ToList();

            return View(grouped);
        }

    }
}
