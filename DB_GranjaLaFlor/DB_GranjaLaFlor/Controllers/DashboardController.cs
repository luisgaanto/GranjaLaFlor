using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * MVC Controller | Dashboard
     *
     * Handles the authenticated user's Dashboard
     * and delegates operational data retrieval to
     * DashboardService.
     */
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;


        /*
         * Dependency Injection | Dashboard Controller
         *
         * ASP.NET Core provides the DashboardService and
         * Logger instances registered in the DI container.
         */
        public DashboardController(
            DashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _dashboardService =
                dashboardService;

            _logger =
                logger;
        }


        /*
         * GET: Dashboard
         *
         * Displays the current production information for
         * every active Broiler House.
         */
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(
                "Entering DashboardController.Index(). " +
                "User: {UserName}",
                User.Identity?.Name);

            /*
             * UI Data | Dashboard
             *
             * Delegates the retrieval of current production
             * information to DashboardService.
             */
            var model =
                await _dashboardService
                    .GetDashboardAsync();

            return View(model);
        }
    }
}