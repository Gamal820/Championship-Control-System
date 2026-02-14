using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IRepository<Championship> _championshipRepo;
        private readonly IRepository<Team> _teamRepo;
        private readonly IRepository<Match> _matchRepo;
        private readonly IRepository<Ticket> _ticketRepo;

        public DashboardController(
            IRepository<Championship> championshipRepo,
            IRepository<Team> teamRepo,
            IRepository<Match> matchRepo,
            IRepository<Ticket> ticketRepo)
        {
            _championshipRepo = championshipRepo;
            _teamRepo = teamRepo;
            _matchRepo = matchRepo;
            _ticketRepo = ticketRepo;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalChampionships = (await _championshipRepo.GetAsync()).Count();
            ViewBag.TotalTeams = (await _teamRepo.GetAsync()).Count();
            ViewBag.TotalMatches = (await _matchRepo.GetAsync()).Count();

            var tickets = await _ticketRepo.GetAsync();
            ViewBag.TotalRevenue = tickets.Sum(t => t.TicketPrice ?? 0);
            ViewBag.TotalTickets = tickets.Count();

            var upcomingMatches = await _matchRepo.GetAsync(
                expression: m => m.MatchDate >= DateTime.Now,
                include: q => q.Include(m => m.HomeTeam).Include(m => m.AwayTeam).Include(m => m.Championship),
                orderBy: q => q.OrderBy(m => m.MatchDate)
            );

            var allMatches = await _matchRepo.GetAsync();
            ViewBag.MatchStatusData = new int[] {
                allMatches.Count(m => m.Status == Enums.MatchStatus.Completed),
                allMatches.Count(m => m.Status == Enums.MatchStatus.Scheduled),
                allMatches.Count(m => m.Status == Enums.MatchStatus.Live)
            };

            return View(upcomingMatches.Take(5).ToList());
        }
    }
}