using Championship_Control_System.Models;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;


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
        public async Task<IActionResult> Pay(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            // Load cart + match (and optionally teams for nice messages)
            var cartItems = (await _cartRepo.GetAsync(x => x.UserId == user.Id,
                include: q => q.Include(c => c.Match).ThenInclude(m => m.HomeTeam)
                    .Include(c => c.Match).ThenInclude(m => m.AwayTeam),
                cancellationToken: cancellationToken
            )).ToList();

            if (!cartItems.Any())
            {
                TempData["error-notification"] = "Cart is empty.";
                return RedirectToAction("Index");
            }

            // Validate availability before payment
            var requiredPerMatch = cartItems
                .GroupBy(x => x.MatchId)
                .Select(g => new { MatchId = g.Key, Needed = g.Sum(x => x.Count) })
                .ToList();

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

            // Build Stripe Checkout Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",

               
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Cart/StripeSuccess?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Cart/StripeCancel",

                LineItems = new List<SessionLineItemOptions>()
            };

            foreach (var item in cartItems)
            {
                var match = item.Match;
                var name = $"{match?.HomeTeam?.TeamName ?? "Home"} vs {match?.AwayTeam?.TeamName ?? "Away"}";
                var unitPrice = item.Price; // EGP

                // Stripe expects minor units (cents): EGP uses 2 decimals
                var unitAmount = (long)Math.Round(unitPrice * 100m);

                options.LineItems.Add(new SessionLineItemOptions
                {
                    Quantity = item.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        UnitAmount = unitAmount,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = name
                        }
                    }
                });
            }

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            
            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> StripeSuccess(string session_id, CancellationToken cancellationToken)
        {
           

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cartItems = (await _cartRepo.GetAsync(
                x => x.UserId == user.Id,
                include: q => q.Include(c => c.Match),
                cancellationToken: cancellationToken
            )).ToList();

            if (!cartItems.Any())
            {
                TempData["error-notification"] = "Cart is empty.";
                return RedirectToAction("Index");
            }

            var requiredPerMatch = cartItems
                .GroupBy(x => x.MatchId)
                .Select(g => new { MatchId = g.Key, Needed = g.Sum(x => x.Count) })
                .ToList();

            foreach (var r in requiredPerMatch)
            {
                var match = cartItems.First(x => x.MatchId == r.MatchId).Match;
                var available = match?.AvailableTicket ?? 0;

                if (available < r.Needed)
                {
                    TempData["error-notification"] = $"Not enough tickets available for match #{r.MatchId}.";
                    return RedirectToAction("Index");
                }
            }

            foreach (var r in requiredPerMatch)
            {
                var match = cartItems.First(x => x.MatchId == r.MatchId).Match!;
                match.AvailableTicket = (match.AvailableTicket ?? 0) - r.Needed;
            }

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

            await _cartRepo.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Payment successful! Tickets booked.";
            return RedirectToAction("Success", "Cart", new { area = "Customer" });
        }


        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }


        [HttpGet]
        public IActionResult StripeCancel()
        {
            TempData["error-notification"] = "Payment was cancelled.";
            return RedirectToAction("Index");
        }



    }
}
