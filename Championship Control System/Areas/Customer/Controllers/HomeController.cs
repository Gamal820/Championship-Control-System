using Championship_Control_System.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<Match> _matchRepository;
        private readonly IRepository<TeamStanding> _standingRepository;
        private readonly IHubContext<MatchHub> _hubContext;

        public HomeController(
            IRepository<Match> matchRepository,
            IRepository<TeamStanding> standingRepository,
            IHubContext<MatchHub> hubContext)
        {
            _matchRepository = matchRepository;
            _standingRepository = standingRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var today = now.Date;

            var todayMatches = await _matchRepository.GetAsync(
                m => m.MatchDate.HasValue && m.MatchDate.Value.Date == today,
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Include(m => m.Stadium)
                    .Include(m => m.Championship),
                cancellationToken: cancellationToken
            );

            bool isUpdated = false;

            foreach (var m in todayMatches)
            {
                var matchTime = m.MatchDate.Value;
                var oldStatus = m.Status; 

                if (m.Status == MatchStatus.Completed) continue;

                if (now >= matchTime && now <= matchTime.AddMinutes(105))
                {
                    m.Status = MatchStatus.Live;
                }
                else if (now < matchTime)
                {
                    m.Status = MatchStatus.Scheduled;
                }
                else
                {
                    m.Status = MatchStatus.Completed;
                }

                if (m.Status != oldStatus)
                {

                    isUpdated = true;

                    await _hubContext.Clients.All.SendAsync("ReceiveMatchUpdate", new
                    {
                        matchId = m.MatchId,
                        homeGoals = m.HomeGoals ?? 0,
                        awayGoals = m.AwayGoals ?? 0,
                        newStatus = m.Status.ToString()
                    });
                }
            }
            if (isUpdated)
            {
                await _matchRepository.CommitAsync(cancellationToken);
            }

            var processedMatches = todayMatches
                .OrderBy(m => m.Status == MatchStatus.Live ? 0 : (m.Status == MatchStatus.Scheduled ? 1 : 2))
                .ThenBy(m => m.MatchDate)
                .ToList();

            var standings = await _standingRepository.GetAsync(
                include: q => q.Include(s => s.Team).Include(s => s.Championship),
                tracked: false,
                cancellationToken: cancellationToken
            );

            var orderedStandings = standings
                .Select(s => new {
                    TeamName = s.Team.TeamName,
                    Played = s.Played ?? 0,
                    Points = ((s.Won ?? 0) * 3) + (s.Draw ?? 0)
                })
                .OrderByDescending(s => s.Points)
                .ToList();

            ViewBag.TodayMatches = processedMatches;
            ViewBag.Standings = orderedStandings;

            return View();
        }
    }
}