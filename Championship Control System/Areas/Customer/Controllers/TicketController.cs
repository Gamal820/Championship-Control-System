using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TicketController : Controller
    {
        private readonly IRepository<Match> _matchRepo;

        public TicketController(IRepository<Match> matchRepo)
        {
            _matchRepo = matchRepo;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var matches = await _matchRepo.GetAsync(
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Include(m => m.Stadium),
                cancellationToken: cancellationToken
            );

            
             matches = matches.Where(m => m.Status == "Scheduled").ToList();

            return View(matches);
        }
    }
}
