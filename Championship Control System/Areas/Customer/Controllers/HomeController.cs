using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Championship_Control_System.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

     //
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

      
        //
        public IActionResult Index()
        {
            return View();
        }


    }
}
