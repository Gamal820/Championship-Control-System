using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChampionshipController : Controller
    {
        private readonly IRepository<Championship> _championshipRepository;
        private readonly IRepository<Team> _teamRepository;

        public ChampionshipController(IRepository<Championship> championshipRepository, IRepository<Team> teamRepository)
        {
            _championshipRepository = championshipRepository;
            _teamRepository = teamRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var championships = await _championshipRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            return View(championships);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            var createChampionshipvm = new CreateChampionshipVM
            {
                Teams =teams.Select(t=>new SelectListItem
                {
                    Value= t.TeamId.ToString(),
                    Text=t.TeamName,
                })
            };


            return View(createChampionshipvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChampionshipVM createChampionshipVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

                createChampionshipVM.Teams = teams.Select(t => new SelectListItem
                {
                    Value = t.TeamId.ToString(),
                    Text = t.TeamName
                });

                return View(createChampionshipVM);
            }

            var championship = new Championship
            {
                ChampionshipName = createChampionshipVM.ChampionshipName,
                Season = createChampionshipVM.Season,
                Country = createChampionshipVM.Country,
                StartDate = createChampionshipVM.StartDate.HasValue ? DateOnly.FromDateTime(createChampionshipVM.StartDate.Value) : null,
                EndDate = createChampionshipVM.EndDate.HasValue ? DateOnly.FromDateTime(createChampionshipVM.EndDate.Value) : null
            };

            // add Logo
            if (createChampionshipVM.LogoFile is not null && createChampionshipVM.LogoFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createChampionshipVM.LogoFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "championships");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    createChampionshipVM.LogoFile.CopyTo(stream);
                }

                championship.Logo = fileName;
            }

            // add Teams
            if (createChampionshipVM.TeamIds is not null && createChampionshipVM.TeamIds.Count > 0)
            {
                var selectedTeams = await _teamRepository.GetAsync(t => createChampionshipVM.TeamIds.Contains(t.TeamId),
                    tracked: true,
                    cancellationToken: cancellationToken);

                foreach (var t in selectedTeams)
                    championship.Teams.Add(t);
            }

            await _championshipRepository.AddAsync(championship, cancellationToken);
            await _championshipRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Championship created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var championship = await _championshipRepository.GetOneAsync( c => c.ChampionshipId == id,include: q => q.Include(c => c.Teams),
                tracked: false,
                cancellationToken: cancellationToken);

            if (championship is null) return NotFound();

            var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            var updateChampionshipVM = new UpdateChampionshipVM
            {
                ChampionshipId = championship.ChampionshipId,
                ChampionshipName = championship.ChampionshipName,
                Season = championship.Season,
                Country = championship.Country,

                StartDate = championship.StartDate.HasValue? championship.StartDate.Value.ToDateTime(TimeOnly.MinValue): (DateTime?)null,

                EndDate = championship.EndDate.HasValue? championship.EndDate.Value.ToDateTime(TimeOnly.MinValue): (DateTime?)null,

                CurrentLogo = championship.Logo,

                TeamIds = championship.Teams.Select(t => t.TeamId).ToList(),

                Teams = teams.Select(t => new SelectListItem
                {
                    Value = t.TeamId.ToString(),
                    Text = t.TeamName,
                    Selected = championship.Teams.Any(x => x.TeamId == t.TeamId)
                })
            };

            return View(updateChampionshipVM);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UpdateChampionshipVM vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                // Refill teams for the view (keep selected)
                var teams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

                vm.Teams = teams.Select(t => new SelectListItem
                {
                    Value = t.TeamId.ToString(),
                    Text = t.TeamName,
                    Selected = vm.TeamIds.Contains(t.TeamId)
                });

                // Keep current logo displayed
                var old = await _championshipRepository.GetOneAsync(
                    c => c.ChampionshipId == vm.ChampionshipId,
                    tracked: false,
                    cancellationToken: cancellationToken);

                vm.CurrentLogo = old?.Logo;

                return View(vm);
            }

            var championship = await _championshipRepository.GetOneAsync(
                c => c.ChampionshipId == vm.ChampionshipId,
                include: q => q.Include(c => c.Teams),
                tracked: true,
                cancellationToken: cancellationToken);

            if (championship is null) return NotFound();

            // Update main fields
            championship.ChampionshipName = vm.ChampionshipName;
            championship.Season = vm.Season;
            championship.Country = vm.Country;
            championship.StartDate = vm.StartDate.HasValue ? DateOnly.FromDateTime(vm.StartDate.Value) : null;
            championship.EndDate = vm.EndDate.HasValue ? DateOnly.FromDateTime(vm.EndDate.Value) : null;

            // Update Teams (many-to-many)
            championship.Teams.Clear();

            if (vm.TeamIds is not null && vm.TeamIds.Count > 0)
            {
                var selectedTeams = await _teamRepository.GetAsync(
                    t => vm.TeamIds.Contains(t.TeamId),
                    tracked: true,
                    cancellationToken: cancellationToken);

                foreach (var t in selectedTeams)
                    championship.Teams.Add(t);
            }

            // Upload new logo (optional) + delete old file
            if (vm.LogoFile is not null && vm.LogoFile.Length > 0)
            {
                var oldLogo = championship.Logo;

                // Save new logo
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.LogoFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "championships");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    vm.LogoFile.CopyTo(stream);
                }

                championship.Logo = fileName;

                // Delete old logo
                if (!string.IsNullOrEmpty(oldLogo))
                {
                    var oldPath = Path.Combine(folderPath, oldLogo);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
            }

            await _championshipRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Championship updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var championship = await _championshipRepository.GetOneAsync(
                c => c.ChampionshipId == id,
                cancellationToken: cancellationToken);

            if (championship is not null)
            {
                // delete logo from wwwroot 
                if (!string.IsNullOrEmpty(championship.Logo))
                {
                    var logoPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot", "images", "championships",
                        championship.Logo);

                    if (System.IO.File.Exists(logoPath))
                        System.IO.File.Delete(logoPath);
                }

                _championshipRepository.Delete(championship);
                await _championshipRepository.CommitAsync(cancellationToken);
            }

            TempData["Success"] = "Championship deleted successfully.";
            return RedirectToAction(nameof(Index));
        }


    }
}
