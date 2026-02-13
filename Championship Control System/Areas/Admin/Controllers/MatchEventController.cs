using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.TournamentManagerRole}")]
    public class MatchEventController : Controller
    {
        private readonly IRepository<MatchEvent> _matchEventRepository;
        private readonly IRepository<Match> _matchRepository;
        private readonly IRepository<Player> _playerRepository;
        public MatchEventController(
            IRepository<MatchEvent> matchEventRepository,
            IRepository<Match> matchRepository,
            IRepository<Player> playerRepository)
        {
            _matchEventRepository = matchEventRepository;
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
        }

        public async Task<IActionResult> Index(int? matchId, CancellationToken cancellationToken)
        {
            ViewBag.SelectedMatchId = matchId;
            IEnumerable<MatchEvent> events;

            if (matchId.HasValue)
            {
                events = await _matchEventRepository.GetAsync(
                    expression: e => e.MatchId == matchId,
                    include: q => q.Include(e => e.Match).ThenInclude(m => m.HomeTeam)
                                   .Include(e => e.Match).ThenInclude(m => m.AwayTeam)
                                   .Include(e => e.Player), 
                    orderBy: q => q.OrderByDescending(e => e.Match.MatchDate).ThenBy(e => e.Minute),
                    tracked: false,
                    cancellationToken: cancellationToken);
            }
            else
            {
                
                events = await _matchEventRepository.GetAsync(
                    include: q => q.Include(e => e.Match).ThenInclude(m => m.HomeTeam)
                                   .Include(e => e.Match).ThenInclude(m => m.AwayTeam)
                                   .Include(e => e.Player),
                    orderBy: q => q.OrderByDescending(e => e.Match.MatchDate).ThenBy(e => e.Minute),
                    tracked: false,
                    cancellationToken: cancellationToken);
            }

            return View(events);
        }


        public async Task<IActionResult> Create(int? matchId, CancellationToken cancellationToken)
        {
            await LoadMatchesList(cancellationToken, matchId);

            await LoadPlayersForMatch(matchId, cancellationToken);

            return View(new MatchEvent { MatchId = matchId, Minute = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> Create(MatchEvent matchEvent, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                await _matchEventRepository.AddAsync(matchEvent, cancellationToken);
                await _matchEventRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match Event created successfully.";
                return RedirectToAction(nameof(Index), new { matchId = matchEvent.MatchId });
            }

            await LoadMatchesList(cancellationToken, matchEvent.MatchId);
            await LoadPlayersForMatch(matchEvent.MatchId, cancellationToken, matchEvent.PlayerId);
            return View(matchEvent);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var matchEvent = await _matchEventRepository.GetOneAsync(e => e.EventId == id, cancellationToken: cancellationToken);
            if (matchEvent == null) return NotFound();

            await LoadMatchesList(cancellationToken, matchEvent.MatchId);
            return View(matchEvent);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MatchEvent matchEvent, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var existingEvent = await _matchEventRepository.GetOneAsync(e => e.EventId == matchEvent.EventId, cancellationToken: cancellationToken);
                if (existingEvent == null) return NotFound();

                existingEvent.EventType = matchEvent.EventType;
                existingEvent.Minute = matchEvent.Minute;
                existingEvent.MatchId = matchEvent.MatchId;
                existingEvent.PlayerId = matchEvent.PlayerId; 

                await _matchEventRepository.CommitAsync(cancellationToken);

                TempData["Success"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index), new { matchId = matchEvent.MatchId });
            }

            await LoadMatchesList(cancellationToken, matchEvent.MatchId);
            await LoadPlayersForMatch(matchEvent.MatchId, cancellationToken, matchEvent.PlayerId);
            return View(matchEvent);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var matchEvent = await _matchEventRepository.GetOneAsync(e => e.EventId == id, cancellationToken: cancellationToken);
            if (matchEvent != null)
            {
                int? matchId = matchEvent.MatchId;
                _matchEventRepository.Delete(matchEvent);
                await _matchEventRepository.CommitAsync(cancellationToken);

                TempData["Success"] = "Event deleted successfully.";
                return RedirectToAction(nameof(Index), new { matchId = matchId });
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadMatchesList(CancellationToken cancellationToken, int? selectedMatchId = null)
        {
            var matches = await _matchRepository.GetAsync(
                include: q => q.Include(m => m.HomeTeam).Include(m => m.AwayTeam),
                orderBy: q => q.OrderByDescending(m => m.MatchDate),
                cancellationToken: cancellationToken);

            ViewBag.Matches = matches.Select(m => new SelectListItem
            {
                Value = m.MatchId.ToString(),
                Text = $"{m.HomeTeam?.TeamName} vs {m.AwayTeam?.TeamName} ({m.MatchDate?.ToShortDateString()})",
                Selected = m.MatchId == selectedMatchId
            }).ToList();

            ViewBag.EventTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Goal", Text = "Goal" },
                new SelectListItem { Value = "YellowCard", Text = "Yellow Card" },
                new SelectListItem { Value = "RedCard", Text = "Red Card" },
                new SelectListItem { Value = "Substitution", Text = "Substitution" },
                new SelectListItem { Value = "Penalty", Text = "Penalty Kick" },
                new SelectListItem { Value = "OwnGoal", Text = "Own Goal" }
            };
        }

        private async Task LoadPlayersForMatch(int? matchId, CancellationToken cancellationToken, int? selectedPlayerId = null)
        {
            if (matchId == null)
            {
                
                ViewBag.Players = new SelectList(new List<Player>(), "PlayerId", "Name");
                return;
            }

            
            var match = await _matchRepository.GetOneAsync(m => m.MatchId == matchId, cancellationToken: cancellationToken);

            if (match != null)
            {

                var players = await _playerRepository.GetAsync(
                    expression: p => p.TeamId == match.HomeTeamId || p.TeamId == match.AwayTeamId,
                    orderBy: q => q.OrderBy(p => p.TeamId).ThenBy(p => p.Fname), 
                    cancellationToken: cancellationToken);

                var playerList = players.Select(p => new
                {
                    PlayerId = p.PlayerId,
                    FullName = $"{p.Fname} {p.Lname} (#{p.ShirtNumber})"
                });

                ViewBag.Players = new SelectList(playerList, "PlayerId", "FullName", selectedPlayerId);
            }
        }
    }
}
 