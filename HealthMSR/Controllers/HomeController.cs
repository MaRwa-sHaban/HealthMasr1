using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}