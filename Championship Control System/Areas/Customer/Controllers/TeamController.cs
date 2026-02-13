using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TeamController : Controller
    {
        private readonly IRepository<Team> _teamRepository;

        public TeamController(IRepository<Team> teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAsync(
                include: q => q
                    .Include(t => t.Stadium)
                    .Include(t => t.Coach),
                tracked: false,
                cancellationToken: cancellationToken
            );

            return View(teams);
        }
    }
}
