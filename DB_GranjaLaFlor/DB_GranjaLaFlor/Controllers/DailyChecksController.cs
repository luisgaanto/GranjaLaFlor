using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGranjaLaFlor.Models.ViewModels;

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
            int? broodId,
            int? year,
            int? broilerHouseId,
            string? dailyCheckWeek,
            string? dailyCheckDay)
        {
            _logger.LogInformation(
                "Entering DailyChecksController.Index(). " +
                "BroodId: {BroodId}, " +
                "Year: {Year}, " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "DailyCheckWeek: {DailyCheckWeek}, " +
                "DailyCheckDay: {DailyCheckDay}",
                broodId,
                year,
                broilerHouseId,
                dailyCheckWeek,
                dailyCheckDay);

            /*
             * Delegates the Daily Check records, current filter values
             * and dropdown menu options to the Service layer.
             */
            var model = await _dailyCheckService.GetFilterViewModelAsync(
                broodId,
                year,
                broilerHouseId,
                dailyCheckWeek,
                dailyCheckDay);

            return View(model);
        }

        /*
         * GET: DailyChecks/Create
         * Displays the Create view and loads the information
         * required to register a new Daily Check.
         */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation(
                "Entering DailyChecksController.Create() GET.");

            var model =
                await _dailyCheckService.GetCreateViewModelAsync();

            return View(model);
        }

        /*
         * UI Request | Broods by Broiler House
         * Retrieves the active Broods associated with the selected
         * Broiler House and returns them to the Create or Edit view.
         */
        [HttpGet]
        public async Task<IActionResult> GetBroodsByBroilerHouse(
            int broilerHouseId)
        {
            _logger.LogInformation(
                "Entering DailyChecksController.GetBroodsByBroilerHouse(). " +
                "BroilerHouseId: {BroilerHouseId}",
                broilerHouseId);

            if (broilerHouseId <= 0)
            {
                _logger.LogWarning(
                    "Invalid BroilerHouseId received while retrieving Broods. " +
                    "BroilerHouseId: {BroilerHouseId}",
                    broilerHouseId);

                return BadRequest(new
                {
                    message =
                        "Debe seleccionar una pollera válida."
                });
            }

            var broodOptions =
                await _dailyCheckService
                    .GetBroodsByBroilerHouseAsync(
                        broilerHouseId);

            return Json(broodOptions.Select(option => new
            {
                value = option.Value,
                text = option.Text
            }));
        }

        /*
         * UI Request | Selected Brood Information
         * Retrieves the Brood, mortality and concentrate information
         * displayed after selecting a Broiler House and a Brood.
         */
        [HttpGet]
        public async Task<IActionResult> GetBroodInformation(
            int broilerHouseId,
            int broodId)
        {
            _logger.LogInformation(
                "Entering DailyChecksController.GetBroodInformation(). " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "BroodId: {BroodId}",
                broilerHouseId,
                broodId);

            if (broilerHouseId <= 0 || broodId <= 0)
            {
                _logger.LogWarning(
                    "Invalid identifiers received while retrieving " +
                    "Daily Check Brood information. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}",
                    broilerHouseId,
                    broodId);

                return BadRequest(new
                {
                    message =
                        "Debe seleccionar una pollera y una camada válidas."
                });
            }

            var broodInformation =
                await _dailyCheckService.GetBroodInformationAsync(
                    broilerHouseId,
                    broodId);

            if (broodInformation == null)
            {
                _logger.LogWarning(
                    "Daily Check Brood information was not found. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}",
                    broilerHouseId,
                    broodId);

                return NotFound(new
                {
                    message =
                        "La camada seleccionada no está disponible " +
                        "para la pollera indicada."
                });
            }

            return Json(new
            {
                broodInformation.BroilerHouseId,
                broodInformation.BroodId,
                broodInformation.BroodBirdInitialNum,
                broodInformation.IncomeConcentrateId,
                broodInformation.IncomeAccumulated,
                broodInformation.AccumulatedMortality,
                broodInformation.DailyBirdBalance,
                broodInformation.AccumulatedConsumption,
                broodInformation.ConcentrateBalance
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DailyCheckFormViewModel model)
        {
            _logger.LogInformation(
                "Entering DailyChecksController.Create() POST. " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "BroodId: {BroodId}, " +
                "DailyCheckWeek: {DailyCheckWeek}, " +
                "DailyCheckDay: {DailyCheckDay}",
                model.BroilerHouseId,
                model.BroodId,
                model.DailyCheckWeek,
                model.DailyCheckDay);

            if (!ModelState.IsValid)
            {
                await _dailyCheckService
                    .PopulateFormOptionsAsync(model);

                return View(model);
            }

            try
            {
                await _dailyCheckService.CreateAsync(model);

                TempData["SuccessMessage"] =
                    "El control diario fue registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while creating " +
                    "Daily Check. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "DailyCheckWeek: {DailyCheckWeek}, " +
                    "DailyCheckDay: {DailyCheckDay}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.DailyCheckWeek,
                    model.DailyCheckDay);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await _dailyCheckService
                    .PopulateFormOptionsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating Daily Check. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "DailyCheckWeek: {DailyCheckWeek}, " +
                    "DailyCheckDay: {DailyCheckDay}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.DailyCheckWeek,
                    model.DailyCheckDay);

                TempData["ErrorMessage"] =
                    "No se pudo registrar el control diario. " +
                    "Intente nuevamente.";

                await _dailyCheckService
                    .PopulateFormOptionsAsync(model);

                return View(model);
            }
        }

        /*
 * GET: DailyChecks/Details/5
 * Retrieves and displays the complete information
 * of the selected Daily Check.
 */
        [HttpGet]
        public async Task<IActionResult> Details(
            int? id)
        {
            _logger.LogInformation(
                "Entering DailyChecksController.Details(). " +
                "DailyCheckId: {DailyCheckId}",
                id);

            if (!id.HasValue)
            {
                _logger.LogWarning(
                    "Daily Check Details request received without an identifier.");

                TempData["ErrorMessage"] =
                    "No se proporcionó un identificador válido para consultar el control diario.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                var dailyCheck =
                    await _dailyCheckService.GetByIdAsync(
                        id.Value);

                if (dailyCheck == null)
                {
                    _logger.LogWarning(
                        "Daily Check was not found while loading Details. " +
                        "DailyCheckId: {DailyCheckId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El control diario seleccionado no existe.";

                    return RedirectToAction(nameof(Index));
                }

                return View(dailyCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Daily Check Details. " +
                    "DailyCheckId: {DailyCheckId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    "No se pudo consultar el detalle del control diario. " +
                    "Intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }





    }
}