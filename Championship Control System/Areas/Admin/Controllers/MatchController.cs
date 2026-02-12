using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MatchController : Controller
    {
        private readonly IRepository<Match> _matchRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Championship> _championshipRepository;

        public MatchController(
            IRepository<Match> matchRepository,
            IRepository<Team> teamRepository,
            IRepository<Stadium> stadiumRepository,
            IRepository<Championship> championshipRepository)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _stadiumRepository = stadiumRepository;
            _championshipRepository = championshipRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var matches = await _matchRepository.GetAsync(
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Include(m => m.Stadium)
                    .Include(m => m.Championship),
                tracked: false,
                cancellationToken: cancellationToken);

            return View(matches);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await LoadDropDowns(cancellationToken);
            return View(new Match());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Match match, CancellationToken cancellationToken)
        {
            if (match.HomeTeamId == match.AwayTeamId)
            {
                ModelState.AddModelError("", "Home team and Away team must be different.");
            }

            if (ModelState.IsValid)
            {
                await _matchRepository.AddAsync(match, cancellationToken);
                await _matchRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match created successfully.";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropDowns(cancellationToken);
            return View(match);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var match = await _matchRepository.GetOneAsync(
                m => m.MatchId == id,
                cancellationToken: cancellationToken);

            if (match is null)
                return NotFound();

            await LoadDropDowns(cancellationToken);
            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Match match, CancellationToken cancellationToken)
        {
            if (match.HomeTeamId == match.AwayTeamId)
            {
                ModelState.AddModelError("", "Home team and Away team must be different.");
            }

            if (ModelState.IsValid)
            {
                var existingMatch = await _matchRepository.GetOneAsync(m => m.MatchId == match.MatchId);
                if (existingMatch is null) return NotFound();

                existingMatch.MatchDate = match.MatchDate;
                //Ticket Price
                existingMatch.TicketPrice = match.TicketPrice;
                //Available Ticket
                existingMatch.AvailableTicket = match.AvailableTicket;
                existingMatch.HomeGoals = match.HomeGoals;
                existingMatch.AwayGoals = match.AwayGoals;
                existingMatch.AvailableTicket = match.AvailableTicket;
                existingMatch.Status = match.Status;
                existingMatch.HomeTeamId = match.HomeTeamId;
                existingMatch.AwayTeamId = match.AwayTeamId;
                existingMatch.StadiumId = match.StadiumId;
                existingMatch.ChampionshipId = match.ChampionshipId;

                await _matchRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropDowns(cancellationToken);
            return View(match);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var match = await _matchRepository.GetOneAsync(m => m.MatchId == id);
            if (match is not null)
            {
                _matchRepository.Delete(match);
                await _matchRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // الطريقة الأضمن لملء القوائم وتجنب NullReferenceException
        private async Task LoadDropDowns(CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken) ?? new List<Team>();
            ViewBag.Teams = teams.Select(t => new SelectListItem
            {
                Value = t.TeamId.ToString(),
                Text = t.TeamName
            }).ToList();

            var stadiums = await _stadiumRepository.GetAsync(tracked: false, cancellationToken: cancellationToken) ?? new List<Stadium>();
            ViewBag.Stadiums = stadiums.Select(s => new SelectListItem
            {
                Value = s.StadiumId.ToString(),
                Text = s.StadiumName
            }).ToList();

            var championships = await _championshipRepository.GetAsync(tracked: false, cancellationToken: cancellationToken) ?? new List<Championship>();
            ViewBag.Championships = championships.Select(c => new SelectListItem
            {
                Value = c.ChampionshipId.ToString(),
                Text = c.ChampionshipName 
            }).ToList();
        }
    }
}