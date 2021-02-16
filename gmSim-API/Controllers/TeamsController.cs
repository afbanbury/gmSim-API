using Microsoft.AspNetCore.Mvc;

namespace gmSim_API.Controllers
{
    public class Teams : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}