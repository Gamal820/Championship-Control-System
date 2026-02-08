using Championship_Control_System.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {

        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Coach> _coachRepository;

        public TeamController(IRepository<Team> teamRepository , IRepository<Stadium> stadiumRepository, IRepository<Coach> coachRepository)
        {
            _teamRepository = teamRepository;
            _stadiumRepository = stadiumRepository;
            _coachRepository = coachRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAsync(include: q => q.Include(t => t.Stadium).Include(t => t.Coach), tracked: false, cancellationToken: cancellationToken);

            return View(teams);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var availableStadiums = await _stadiumRepository.GetAsync(expression: s => s.Team == null,tracked: false,cancellationToken: cancellationToken);

            ViewBag.Stadiums = new SelectList(availableStadiums, "StadiumId", "StadiumName");

            var availableCoaches = await _coachRepository.GetAsync(c => c.Team == null, tracked: false);
            ViewBag.Coaches = new SelectList(availableCoaches, "CoachId", "Name");

            return View(new Team());

        }

        [HttpPost]
        public async Task<IActionResult> Create(Team team, IFormFile logo, CancellationToken cancellationToken)
        {
            if (logo is not null)
            {
                var logoName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "teams", logoName);
                using (var stream = System.IO.File.Create(logoPath))
                {
                    await logo.CopyToAsync(stream, cancellationToken);
                }

                team.Logo = "/images/teams/" + logoName;
            }
            if (ModelState.IsValid)
            {
                await _teamRepository.AddAsync(team, cancellationToken);
                await _teamRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Team created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetOneAsync(e => e.TeamId == id, include : q => q.Include(t=>t.Stadium).Include(t =>t.Coach), cancellationToken: cancellationToken);

            if (team is null)
            {
                return NotFound();
            }

            var stadiums = await _stadiumRepository.GetAsync(s => s.Team == null || s.StadiumId == team.StadiumId, tracked: false);
            ViewBag.Stadiums = new SelectList(stadiums, "StadiumId", "StadiumName", team.StadiumId);

            var coaches = await _coachRepository.GetAsync(c => c.Team == null || c.CoachId == team.CoachId, tracked: false);
            ViewBag.Coaches = new SelectList(coaches, "CoachId", "Name", team.CoachId);

            return View(team);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Team team, IFormFile? logo, CancellationToken cancellationToken)
        {
            var existingTeam = await _teamRepository.GetOneAsync(e => e.TeamId == team.TeamId, include: q => q.Include(t => t.Stadium).Include(t => t.Coach), cancellationToken: cancellationToken);

            if (existingTeam is null)
                return NotFound();


            if (logo is not null)
            {
                var logoName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "teams", logoName);
                using (var stream = System.IO.File.Create(logoPath))
                {
                    await logo.CopyToAsync(stream, cancellationToken);
                }

                if (existingTeam.Logo is not null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "teams", existingTeam.Logo);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                existingTeam.Logo = "/images/teams/" + logoName;
            }
            if (ModelState.IsValid)
            {
                existingTeam.TeamName = team.TeamName;
                existingTeam.FoundationDate = team.FoundationDate;
                existingTeam.Country = team.Country;
                existingTeam.CoachId = team.CoachId;
                existingTeam.StadiumId = team.StadiumId;

                await _teamRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Team updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetOneAsync(e => e.TeamId == id, cancellationToken: cancellationToken);

            if (team is not null)
            {
                _teamRepository.Delete(team);
                await _teamRepository.CommitAsync(cancellationToken);
            }
            TempData["Success"] = "Team deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

    }

}
