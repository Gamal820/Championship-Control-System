using Microsoft.AspNetCore.Mvc;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class StandingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
