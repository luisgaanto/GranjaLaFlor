using DB_GranjaLaFlor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DB_GranjaLaFlor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /*
         * Public Landing Page.
         * Allows anonymous users to access the application's main page.
         */
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /*
         * Private Dashboard.
         * Only authenticated users can access this page after Login.
         */
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        /*
         * Public Privacy page.
         */
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        /*
         * Displays the application's error page.
         * Response caching is disabled to ensure error information is always current.
         */
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
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