using Microsoft.AspNetCore.Mvc;

namespace gmSim_API.Controllers
{
    public class FixturesController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}