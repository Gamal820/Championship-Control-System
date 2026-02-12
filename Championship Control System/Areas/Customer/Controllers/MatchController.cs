using Microsoft.AspNetCore.Mvc;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
    }
}
