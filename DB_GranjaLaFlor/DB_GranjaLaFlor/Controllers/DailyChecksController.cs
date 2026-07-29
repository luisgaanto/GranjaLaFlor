using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Architecture Decision | Thin Controller
     * Controllers coordinate HTTP requests and responses only.
     * Business rules, calculations and database operations are delegated
     * to the Service layer.
     */
    [Authorize]
    public class DailyChecksController : Controller
    {
        private readonly DailyCheckService _dailyCheckService;
        private readonly ILogger<DailyChecksController> _logger;

        public DailyChecksController(
            DailyCheckService dailyCheckService,
            ILogger<DailyChecksController> logger)
        {
            _dailyCheckService = dailyCheckService;
            _logger = logger;
        }

        /*
         * UI Request | Daily Check Index
         * Receives the optional filters selected by the user and delegates
         * the retrieval of records and dropdown options to the Service layer.
         */
        [HttpGet]
        public async Task<IActionResult> Index(
            string? broodName,
            int? year,
            int? broilerHouseId,
            string? dailyCheckWeek,
            string? dailyCheckDay)
        {
            _logger.LogInformation(
                "Entering DailyChecksController.Index(). " +
                "BroodName: {BroodName}, " +
                "Year: {Year}, " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "DailyCheckWeek: {DailyCheckWeek}, " +
                "DailyCheckDay: {DailyCheckDay}",
                broodName,
                year,
                broilerHouseId,
                dailyCheckWeek,
                dailyCheckDay);

            /*
             * Delegates the Daily Check records, current filter values
             * and dropdown menu options to the Service layer.
             */
            var model = await _dailyCheckService.GetFilterViewModelAsync(
                broodName,
                year,
                broilerHouseId,
                dailyCheckWeek,
                dailyCheckDay);

            return View(model);
        }
    }
}