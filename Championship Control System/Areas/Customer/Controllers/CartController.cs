using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<CartItem> _cartRepo;
        private readonly IRepository<Match> _matchRepo;
        private readonly IRepository<Ticket> _ticketRepo;

        public CartController(UserManager<ApplicationUser> userManager,IRepository<CartItem> cartRepo,IRepository<Match> matchRepo,
            IRepository<Ticket> ticketRepo)
        {
            _userManager = userManager;
            _cartRepo = cartRepo;
            _matchRepo = matchRepo;
            _ticketRepo = ticketRepo;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepo.GetAsync(
                e => e.UserId == user.Id,
                include: q => q
                    .Include(x => x.Match)
                    .ThenInclude(m => m.HomeTeam)
                    .Include(x => x.Match)
                    .ThenInclude(m => m.AwayTeam)
                    .Include(x => x.Match)
                    .ThenInclude(m => m.Stadium),
                cancellationToken: cancellationToken
            );

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int matchId, int count = 1, CancellationToken cancellationToken = default)
        {
            if (count <= 0) count = 1;

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var match = await _matchRepo.GetOneAsync(m => m.MatchId == matchId, cancellationToken: cancellationToken);
            if (match is null) return NotFound();

            var unitPrice = match.TicketPrice ?? 0m;

            var existing = await _cartRepo.GetOneAsync(
                c => c.UserId == user.Id && c.MatchId == matchId,
                cancellationToken: cancellationToken
            );

            if (existing is not null)
            {
                existing.Count += count;
            }
            else
            {
                await _cartRepo.AddAsync(new CartItem
                {
                    UserId = user.Id,
                    MatchId = matchId,
                    Count = count,
                    Price = unitPrice
                }, cancellationToken);
            }

            await _cartRepo.CommitAsync(cancellationToken);
            return RedirectToAction("Index", "Cart", new { area = "Customer" });

        }


        public async Task<IActionResult> Increment(int matchId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var item = await _cartRepo.GetOneAsync(c => c.UserId == user.Id && c.MatchId == matchId, cancellationToken: cancellationToken);
            if (item is null) return NotFound();

            item.Count += 1;
            await _cartRepo.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Decrement(int matchId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var item = await _cartRepo.GetOneAsync(c => c.UserId == user.Id && c.MatchId == matchId, cancellationToken: cancellationToken);
            if (item is null) return NotFound();

            if (item.Count <= 1) _cartRepo.Delete(item);
            else item.Count -= 1;

            await _cartRepo.CommitAsync(cancellationToken);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int matchId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var item = await _cartRepo.GetOneAsync(c => c.UserId == user.Id && c.MatchId == matchId, cancellationToken: cancellationToken);
            if (item is null) return NotFound();

            _cartRepo.Delete(item);
            await _cartRepo.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
        {
            // Get current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            // Get user's cart items including related Match
            var cartItems = (await _cartRepo.GetAsync(x => x.UserId == user.Id,include: q => q.Include(c => c.Match),
                cancellationToken: cancellationToken
            )).ToList();

            
            if (!cartItems.Any())
            {
                TempData["error-notification"] = "Cart is empty.";
                return RedirectToAction("Index");
            }

            // Calculate total tickets needed per match
            var requiredPerMatch = cartItems
                .GroupBy(x => x.MatchId)
                .Select(g => new { MatchId = g.Key, Needed = g.Sum(x => x.Count) })
                .ToList();

            // Validate available tickets for each match
            foreach (var r in requiredPerMatch)
            {
                var match = cartItems.First(x => x.MatchId == r.MatchId).Match;
                var available = match?.AvailableTicket ?? 0;

                if (available < r.Needed)
                {
                    var home = match?.HomeTeam?.TeamName ?? "Home";
                    var away = match?.AwayTeam?.TeamName ?? "Away";

                    TempData["error-notification"] =
                        $"Not enough tickets for {home} vs {away}. Needed: {r.Needed}, Available: {available}.";

                    return RedirectToAction("Index");
                }
            }

            // Deduct available tickets for each match
            foreach (var r in requiredPerMatch)
            {
                var match = cartItems.First(x => x.MatchId == r.MatchId).Match;
                match!.AvailableTicket = (match.AvailableTicket ?? 0) - r.Needed;
            }

            // Create Ticket records and clear cart
            foreach (var item in cartItems)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    await _ticketRepo.AddAsync(new Ticket
                    {
                        UserId = user.Id,
                        MatchId = item.MatchId,
                        TicketPrice = item.Price,
                        SeatNumber = null,
                        BookingDate = DateTime.UtcNow
                    }, cancellationToken);
                }

                _cartRepo.Delete(item);
            }

            // Save changes
            await _cartRepo.CommitAsync(cancellationToken);

            // Redirect to success page
            TempData["success-notification"] = "Checkout completed successfully!";
            return RedirectToAction("Success", "Cart", new { area = "Customer" });
        }


        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }


    }
}
