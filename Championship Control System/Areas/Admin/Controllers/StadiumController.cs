using Championship_Control_System.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StadiumController : Controller
    {

        private readonly IRepository<Stadium> _stadiumRepository;

        public StadiumController(IRepository<Stadium> stadiumRepository)
        {
            _stadiumRepository = stadiumRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var stadiums = await _stadiumRepository.GetAsync(include: q => q.Include(s => s.Team), tracked: false, cancellationToken: cancellationToken);

            return View(stadiums);
        }

        public IActionResult Create()
        {

            return View(new Stadium());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Stadium stadium, CancellationToken cancellationToken)
        {

            if (ModelState.IsValid)
            {
                await _stadiumRepository.AddAsync(stadium, cancellationToken);
                await _stadiumRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(stadium);
        }


        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var stadium = await _stadiumRepository.GetOneAsync(e => e.StadiumId == id, cancellationToken: cancellationToken);

            if (stadium is null)
            {
                return NotFound();
            }
            return View(stadium);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Stadium stadium, CancellationToken cancellationToken)
        {
            var existingStadium = await _stadiumRepository.GetOneAsync(e => e.StadiumId == stadium.StadiumId, include: q => q.Include(s => s.Team), cancellationToken: cancellationToken);

            if (existingStadium is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existingStadium.StadiumName = stadium.StadiumName;
                existingStadium.Capacity = stadium.Capacity;
                existingStadium.City = stadium.City;

                await _stadiumRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(stadium);
        }


        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var stadium = await _stadiumRepository.GetOneAsync(e => e.StadiumId == id, cancellationToken: cancellationToken);

            if (stadium is not null)
            {
                _stadiumRepository.Delete(stadium);
                await _stadiumRepository.CommitAsync(cancellationToken);
            }
            return RedirectToAction(nameof(Index));
        }




    }
}
