using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.TeamManagerRole}")]
    public class PlayerController : Controller
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Team> _teamRepository;

        public PlayerController(IRepository<Player> playerRepository, IRepository<Team> teamRepository)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetAsync(include: q => q.Include(t => t.Team), tracked: false, cancellationToken: cancellationToken);
            return View(player);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            ViewBag.Teams = new SelectList(teams, "TeamId", "TeamName");
            ViewBag.Positions = new SelectList(new List<string> { "Goalkeeper", "Defender", "Midfielder", "Forward" });

            return View(new Player());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Player player, IFormFile img, CancellationToken cancellationToken)
        {
            if (img is not null)
            {
                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "players", imgName);

                using (var stream = System.IO.File.Create(imgPath))
                {
                    await img.CopyToAsync(stream, cancellationToken);
                }

                player.Img = "/images/players/" + imgName;
            }

            if (ModelState.IsValid)
            {
                await _playerRepository.AddAsync(player, cancellationToken);
                await _playerRepository.CommitAsync(cancellationToken);

                TempData["Success"] = "Player created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            ViewBag.Teams = new SelectList(teams, "TeamId", "TeamName");
            ViewBag.Positions = new SelectList(new List<string> { "Goalkeeper", "Defender", "Midfielder", "Forward" });

            return View(player);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetOneAsync(e => e.PlayerId == id, include: q => q.Include(t => t.Team), cancellationToken: cancellationToken);

            if (player is null)
            {
                return NotFound();
            }

            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            ViewBag.Teams = new SelectList(teams, "TeamId", "TeamName");
            ViewBag.Positions = new SelectList(new List<string> { "Goalkeeper", "Defender", "Midfielder", "Forward" });

            return View(player);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Player player, IFormFile? img, CancellationToken cancellationToken)
        {
            var existingPlayer = await _playerRepository.GetOneAsync(e => e.PlayerId == player.PlayerId, cancellationToken: cancellationToken);

            if (existingPlayer is null)
                return NotFound();

            if (img is not null)
            {
                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "players", imgName);

                if (!string.IsNullOrEmpty(existingPlayer.Img))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingPlayer.Img.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                using (var stream = System.IO.File.Create(imgPath))
                {
                    await img.CopyToAsync(stream, cancellationToken);
                }

                existingPlayer.Img = "/images/players/" + imgName;
            }

            if (ModelState.IsValid)
            {
                existingPlayer.Fname = player.Fname;
                existingPlayer.Lname = player.Lname;
                existingPlayer.Position = player.Position;
                existingPlayer.BirthDate = player.BirthDate;
                existingPlayer.Nationality = player.Nationality;
                existingPlayer.ShirtNumber = player.ShirtNumber;
                existingPlayer.TeamId = player.TeamId;

                await _playerRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Player updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            ViewBag.Teams = new SelectList(teams, "TeamId", "TeamName");
            ViewBag.Positions = new SelectList(new List<string> { "Goalkeeper", "Defender", "Midfielder", "Forward" });

            return View(player);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetOneAsync(e => e.PlayerId == id, cancellationToken: cancellationToken);

            if (player is not null)
            {
                if (!string.IsNullOrEmpty(player.Img))
                {
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", player.Img.TrimStart('/'));
                    if (System.IO.File.Exists(imgPath))
                    {
                        System.IO.File.Delete(imgPath);
                    }
                }

                _playerRepository.Delete(player);
                await _playerRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Player deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}