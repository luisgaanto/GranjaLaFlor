using DB_GranjaLaFlor.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DB_GranjaLaFlor.Controllers
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
            return View();
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


/*
using DB_GranjaLaFlor.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DB_GranjaLaFlor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .ToListAsync();

            return View(roles);
        }
    }
}
*/