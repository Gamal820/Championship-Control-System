using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MatchController : Controller
    {
        private readonly IRepository<Match> _matchRepository;

        public MatchController(IRepository<Match> matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<IActionResult> Index(
            string filter = "All",
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.Today;

            var matches = await _matchRepository.GetAsync(
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Include(m => m.Stadium)
                    .Include(m => m.Championship),
                tracked: false,
                cancellationToken: cancellationToken
            );

            // ===== Filter =====
            matches = filter switch
            {
                "Today" => matches
                    .Where(m => m.MatchDate.HasValue &&
                                m.MatchDate.Value.Date == today)
                    .ToList(),

                "Upcoming" => matches
                    .Where(m => m.MatchDate > today)
                    .ToList(),

                _ => matches
            };

            ViewBag.Filter = filter;

            return View(matches.OrderBy(m => m.MatchDate));
        }
    }
}
