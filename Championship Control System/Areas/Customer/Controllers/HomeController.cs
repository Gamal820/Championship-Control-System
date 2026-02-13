using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area(areaName: "Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<Match> _matchRepository;
        private readonly IRepository<TeamStanding> _standingRepository;

        public HomeController(
            IRepository<Match> matchRepository,
            IRepository<TeamStanding> standingRepository)
        {
            _matchRepository = matchRepository;
            _standingRepository = standingRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            var todayMatches = await _matchRepository.GetAsync(
                m => m.MatchDate.HasValue && m.MatchDate.Value.Date == today,
                include: q => q
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Include(m => m.Stadium)
                    .Include(m => m.Championship),
                tracked: false,
                cancellationToken: cancellationToken
            );

            // ✅ League Standings
            var standings = await _standingRepository.GetAsync(
                include: q => q
                    .Include(s => s.Team)
                    .Include(s => s.Championship),
                tracked: false,
                cancellationToken: cancellationToken
            );

            // ترتيب حسب النقاط (Win=3 / Draw=1)
            var orderedStandings = standings
                .Select(s => new
                {
                    TeamName = s.Team.TeamName,
                    Played = s.Played ?? 0,
                    Points = ((s.Won ?? 0) * 3) + (s.Draw ?? 0)
                })
                .OrderByDescending(s => s.Points)
                .ToList();

            // نبعت الداتا للـ View
            ViewBag.TodayMatches = todayMatches;
            ViewBag.Standings = orderedStandings;

            return View();
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

    }
}
