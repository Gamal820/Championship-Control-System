using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StandingController : Controller
    {
        private readonly IRepository<Championship> _championshipRepo;

        public StandingController(IRepository<Championship> championshipRepo)
        {
            _championshipRepo = championshipRepo;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var championships = await _championshipRepo.GetAsync(
                include: q => q.Include(c => c.TeamStandings).ThenInclude(ts => ts.Team),
                cancellationToken: cancellationToken
            );

            return View(championships);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var championship = await _championshipRepo.GetOneAsync(
                c => c.ChampionshipId == id,
                include: q => q.Include(c => c.TeamStandings).ThenInclude(ts => ts.Team),
                cancellationToken: cancellationToken
            );

            if (championship == null) return NotFound();

            championship.TeamStandings = championship.TeamStandings
                .OrderByDescending(ts => (ts.Won ?? 0) * 3 + (ts.Draw ?? 0))
                .ThenByDescending(ts => ts.GoalDifference ?? 0)
                .ToList();

            return View(championship);
        }

    }
}
