using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocTube.Core.Models.Domains;
using SocTube.Core.ViewModels;
using SocTube.Persistence;
using SocTube.Persistence.Repositories;
using System.Diagnostics;

namespace SocTube.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View();

            else
                return RedirectToAction("Users", "SocTube");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
