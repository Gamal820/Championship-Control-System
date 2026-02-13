using Championship_Control_System.Models;
using Championship_Control_System.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.TeamManagerRole}")]
    public class CoachController : Controller
    {
        private readonly IRepository<Coach> _coachRepository;
        private readonly IRepository<Team> _teamRepository;

        public CoachController(IRepository<Coach> coachRepository, IRepository<Team> teamRepository)
        {
            _coachRepository = coachRepository;
            _teamRepository = teamRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var coaches = await _coachRepository.GetAsync( include: e => e.Include(c => c.Team),tracked: false,
                cancellationToken: cancellationToken);

            return View(coaches);
        }


        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {

            var allTeams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            var allCoaches = await _coachRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            var takenTeamIds = allCoaches
                .Where(c => c.TeamId != null)
                .Select(c => c.TeamId)
                .ToList();

            var availableTeams = allTeams
                .Where(t => !takenTeamIds.Contains(t.TeamId))
                .Select(t => new SelectListItem
                {
                    Value = t.TeamId.ToString(),
                    Text = t.TeamName
                });


            var createCoachVM = new CreateCoachVM
            {
                Teams = availableTeams

            };

            return View(createCoachVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCoachVM createCoachVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var allTeams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
                var allCoaches = await _coachRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

                var takenTeamIds = allCoaches
                    .Where(c => c.TeamId != null)
                    .Select(c => c.TeamId)
                    .ToList();

                createCoachVM.Teams = allTeams
                    .Where(t => !takenTeamIds.Contains(t.TeamId))
                    .Select(t => new SelectListItem
                    {
                        Value = t.TeamId.ToString(),
                        Text = t.TeamName
                    });

                return View(createCoachVM);
            }

            var coach = new Coach
            {
                Name = createCoachVM.Name,
                TeamId = createCoachVM.TeamId,
                BirthData = createCoachVM.BirthDate.HasValue
                    ? DateOnly.FromDateTime(createCoachVM.BirthDate.Value)
                    : null
            };


            if (createCoachVM.ImageFile is not null && createCoachVM.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createCoachVM.ImageFile.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\coaches", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    createCoachVM.ImageFile.CopyTo(stream);
                }

                // Save image name/path in db
                coach.Img = fileName;
            }

            await _coachRepository.AddAsync(coach, cancellationToken);
            await _coachRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Coach created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var coach = await _coachRepository.GetOneAsync(
                c => c.CoachId == id,
                tracked: false,
                cancellationToken: cancellationToken);

            if (coach is null) return NotFound();

            var allTeams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            var allCoaches = await _coachRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);


            var takenTeamIds = allCoaches
                .Where(c => c.TeamId != null && c.TeamId != coach.TeamId) // <--- هنا التريك
                .Select(c => c.TeamId)
                .ToList();

            var availableTeams = allTeams
                .Where(t => !takenTeamIds.Contains(t.TeamId))
                .Select(t => new SelectListItem
                {
                    Value = t.TeamId.ToString(),
                    Text = t.TeamName,
                    Selected = t.TeamId == coach.TeamId // تحديد الفريق الحالي
                });

            var updateCoachVM = new UpdateCoachVM
            {
                CoachId = coach.CoachId,
                Name = coach.Name,
                BirthDate = coach.BirthData.HasValue ? coach.BirthData.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                TeamId = coach.TeamId,
                CurrentImg = coach.Img,
                Teams = availableTeams
            };

            return View(updateCoachVM);
        }
        


        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCoachVM updateCoachVM, CancellationToken cancellationToken)
        {
           
            if (!ModelState.IsValid)
            {
                var allTeams = await _teamRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
                var allCoaches = await _coachRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

                var takenTeamIds = allCoaches
                    .Where(c => c.TeamId != null && c.TeamId != updateCoachVM.TeamId)
                    .Select(c => c.TeamId)
                    .ToList();

                updateCoachVM.Teams = allTeams
                    .Where(t => !takenTeamIds.Contains(t.TeamId))
                    .Select(t => new SelectListItem
                    {
                        Value = t.TeamId.ToString(),
                        Text = t.TeamName,
                        Selected = t.TeamId == updateCoachVM.TeamId
                    });

                var coachForImg = await _coachRepository.GetOneAsync(c => c.CoachId == updateCoachVM.CoachId, tracked: false, cancellationToken: cancellationToken);
                updateCoachVM.CurrentImg = coachForImg?.Img;

                return View(updateCoachVM);
            }

            var existingCoach = await _coachRepository.GetOneAsync(c => c.CoachId == updateCoachVM.CoachId,
                cancellationToken: cancellationToken);

            if (existingCoach is null)
                return NotFound();

            // update coach info
            existingCoach.Name = updateCoachVM.Name;
            existingCoach.TeamId = updateCoachVM.TeamId;
            existingCoach.BirthData = updateCoachVM.BirthDate.HasValue? DateOnly.FromDateTime(updateCoachVM.BirthDate.Value): null;

            // upload new image (optional)
            if (updateCoachVM.ImageFile is not null && updateCoachVM.ImageFile.Length > 0)
            {
                // old image name
                var oldImg = existingCoach.Img;

                // save new image
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateCoachVM.ImageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "coaches");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    updateCoachVM.ImageFile.CopyTo(stream);
                }

                // update db
                existingCoach.Img = fileName;

                // delete old image from wwwroot
                if (!string.IsNullOrEmpty(oldImg))
                {
                    var oldPath = Path.Combine(folderPath, oldImg);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
            }

            await _coachRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Coach updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var coach = await _coachRepository.GetOneAsync(c => c.CoachId == id, cancellationToken: cancellationToken);

            if (coach is not null)
            {
                // delete image from wwwroot if exists
                if (!string.IsNullOrEmpty(coach.Img))
                {
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "coaches", coach.Img);
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }

                _coachRepository.Delete(coach);
                await _coachRepository.CommitAsync(cancellationToken);
            }

            TempData["Success"] = "Coach deleted successfully.";
            return RedirectToAction(nameof(Index));
        }



    }
}
