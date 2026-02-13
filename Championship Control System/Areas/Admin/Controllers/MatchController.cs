using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.TournamentManagerRole}")]
    public class MatchController : Controller
    {
        private readonly IRepository<Match> _matchRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Championship> _championshipRepository;
        private readonly IRepository<TeamStanding> _standingRepo;

        public MatchController(
            IRepository<Match> matchRepository,
            IRepository<Team> teamRepository,
            IRepository<Stadium> stadiumRepository,
            IRepository<Championship> championshipRepository,
            IRepository<TeamStanding> standingRepo)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _stadiumRepository = stadiumRepository;
            _championshipRepository = championshipRepository;
            _standingRepo = standingRepo;
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
                if (match.AvailableTicket == null || match.AvailableTicket == 0)
                {
                    var stadium = await _stadiumRepository.GetOneAsync(s => s.StadiumId == match.StadiumId, cancellationToken: cancellationToken);
                    if (stadium != null) match.AvailableTicket = stadium.Capacity;
                }

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
                var existingMatch = await _matchRepository.GetOneAsync(m => m.MatchId == match.MatchId, tracked: false, cancellationToken: cancellationToken);
                if (existingMatch is null) return NotFound();

                var matchToUpdate = await _matchRepository.GetOneAsync(m => m.MatchId == match.MatchId, cancellationToken: cancellationToken);

                matchToUpdate.MatchDate = match.MatchDate;
                matchToUpdate.TicketPrice = match.TicketPrice;
                matchToUpdate.HomeGoals = match.HomeGoals;       
                matchToUpdate.AwayGoals = match.AwayGoals;       
                matchToUpdate.AvailableTicket = match.AvailableTicket;
                matchToUpdate.HomeTeamId = match.HomeTeamId;
                matchToUpdate.AwayTeamId = match.AwayTeamId;
                matchToUpdate.StadiumId = match.StadiumId;
                matchToUpdate.ChampionshipId = match.ChampionshipId;
                matchToUpdate.Status = match.Status;

                if (match.Status == "Finished" && existingMatch.Status != "Finished")
                {
                    await UpdateStandings(matchToUpdate);
                }

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

        

        private async Task UpdateStandings(Match match)
        {
            if (match.HomeTeamId == null || match.AwayTeamId == null || match.ChampionshipId == null) return;


            var homeStanding = await _standingRepo.GetOneAsync(s => s.TeamId == match.HomeTeamId && s.ChampionshipId == match.ChampionshipId);
            var awayStanding = await _standingRepo.GetOneAsync(s => s.TeamId == match.AwayTeamId && s.ChampionshipId == match.ChampionshipId);

            if (homeStanding == null)
            {
                homeStanding = new TeamStanding
                {
                    TeamId = match.HomeTeamId,
                    ChampionshipId = match.ChampionshipId,
                    Played = 0,
                    Won = 0,
                    Draw = 0,
                    Lost = 0,
                    GoalDifference = 0
                };
                await _standingRepo.AddAsync(homeStanding);
            }
            if (awayStanding == null)
            {
                awayStanding = new TeamStanding
                {
                    TeamId = match.AwayTeamId,
                    ChampionshipId = match.ChampionshipId,
                    Played = 0,
                    Won = 0,
                    Draw = 0,
                    Lost = 0,
                    GoalDifference = 0
                };
                await _standingRepo.AddAsync(awayStanding);
            }

            homeStanding.Played = (homeStanding.Played ?? 0) + 1;
            awayStanding.Played = (awayStanding.Played ?? 0) + 1;

            int homeGoals = match.HomeGoals ?? 0;
            int awayGoals = match.AwayGoals ?? 0;

            homeStanding.GoalDifference = (homeStanding.GoalDifference ?? 0) + (homeGoals - awayGoals);
            awayStanding.GoalDifference = (awayStanding.GoalDifference ?? 0) + (awayGoals - homeGoals);

            if (homeGoals > awayGoals) 
            {
                homeStanding.Won = (homeStanding.Won ?? 0) + 1;
                awayStanding.Lost = (awayStanding.Lost ?? 0) + 1;
            }
            else if (awayGoals > homeGoals) 
            {
                awayStanding.Won = (awayStanding.Won ?? 0) + 1;
                homeStanding.Lost = (homeStanding.Lost ?? 0) + 1;
            }
            else 
            {
                homeStanding.Draw = (homeStanding.Draw ?? 0) + 1;
                awayStanding.Draw = (awayStanding.Draw ?? 0) + 1;
            }
        }


        private async Task LoadDropDowns(CancellationToken cancellationToken ,Match? match = null)
        {
            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken) ?? new List<Team>();
            ViewBag.HomeTeams = new SelectList(teams, "TeamId", "TeamName", match?.HomeTeamId);
            ViewBag.AwayTeams = new SelectList(teams, "TeamId", "TeamName", match?.AwayTeamId);

            var stadiums = await _stadiumRepository.GetAsync(tracked: false, cancellationToken: cancellationToken) ?? new List<Stadium>();
            ViewBag.Stadiums = new SelectList(stadiums, "StadiumId", "StadiumName", match?.StadiumId);

            var championships = await _championshipRepository.GetAsync(tracked: false, cancellationToken: cancellationToken) ?? new List<Championship>();
            ViewBag.Championships = new SelectList(championships, "ChampionshipId", "ChampionshipName", match?.ChampionshipId);
        }

    }
}