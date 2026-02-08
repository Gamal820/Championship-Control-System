using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MatchEventController : Controller
    {
        private readonly IRepository<MatchEvent> _matchEventRepository;
        private readonly IRepository<Match> _matchRepository;

        public MatchEventController(
            IRepository<MatchEvent> matchEventRepository,
            IRepository<Match> matchRepository)
        {
            _matchEventRepository = matchEventRepository;
            _matchRepository = matchRepository;
        }

        //  Index 
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var events = await _matchEventRepository.GetAsync(
                include: q => q.Include(e => e.Match),
                tracked: false,
                cancellationToken: cancellationToken);

            return View(events);
        }

        //  Create  
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await LoadMatches(cancellationToken);
            return View(new MatchEvent());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MatchEvent matchEvent, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                await _matchEventRepository.AddAsync(matchEvent, cancellationToken);
                await _matchEventRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match Event created successfully.";
                return RedirectToAction(nameof(Index));
            }

            await LoadMatches(cancellationToken);
            return View(matchEvent);
        }

        //   Edit 
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var matchEvent = await _matchEventRepository.GetOneAsync(
                e => e.EventId == id,
                include: q => q.Include(e => e.Match),
                cancellationToken: cancellationToken);

            if (matchEvent == null)
                return NotFound();

            await LoadMatches(cancellationToken, matchEvent.MatchId);
            return View(matchEvent);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MatchEvent matchEvent, CancellationToken cancellationToken)
        {
            var existingEvent = await _matchEventRepository.GetOneAsync(
                e => e.EventId == matchEvent.EventId,
                cancellationToken: cancellationToken);

            if (existingEvent == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existingEvent.Minute = matchEvent.Minute;
                existingEvent.EventType = matchEvent.EventType;
                existingEvent.MatchId = matchEvent.MatchId;

                await _matchEventRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            await LoadMatches(cancellationToken, matchEvent.MatchId);
            return View(matchEvent);
        }

        //   Delete 
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var matchEvent = await _matchEventRepository.GetOneAsync(
                e => e.EventId == id,
                cancellationToken: cancellationToken);

            if (matchEvent != null)
            {
                _matchEventRepository.Delete(matchEvent);
                await _matchEventRepository.CommitAsync(cancellationToken);
                TempData["Success"] = "Match Event deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        //  Helper method to load matches for dropdown
        private async Task LoadMatches(CancellationToken cancellationToken, int? selectedMatchId = null)
        {
            var matches = await _matchRepository.GetAsync(
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam),
                tracked: false,
                cancellationToken: cancellationToken);

            ViewBag.Matches = new SelectList(
                matches,
                "MatchId",
                "MatchId",
                selectedMatchId
            );
        }
    }
}
