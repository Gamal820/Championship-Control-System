using Microsoft.AspNetCore.Mvc;

namespace Championship_Control_System.Areas.Admin.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
