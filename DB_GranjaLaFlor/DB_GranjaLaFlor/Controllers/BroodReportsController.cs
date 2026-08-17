using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Mvc;
using ProjectGranjaLaFlor.Models.ViewModels.BroodReport;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Controller | Brood Reports
     *
     * Coordinates the HTTP requests associated with the
     * generation and consultation of historical Brood Reports.
     *
     * Business validations, snapshot generation and database
     * access are delegated to BroodReportService.
     */
    public class BroodReportsController : Controller
    {
        private readonly BroodReportService _broodReportService;
        private readonly ILogger<BroodReportsController> _logger;

        public BroodReportsController(
            BroodReportService broodReportService,
            ILogger<BroodReportsController> logger)
        {
            _broodReportService = broodReportService;
            _logger = logger;
        }


        /*
         * GET: BroodReports
         *
         * Displays the historical list of generated
         * Brood Reports.
         */
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(
                "Entering BroodReportsController.Index().");

            try
            {
                var broodReports =
                    await _broodReportService
                        .GetAllAsync();

                return View(
                    broodReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Brood Reports Index.");

                TempData["ErrorMessage"] =
                    "No se pudo cargar el historial de reportes de camada. " +
                    "Intente nuevamente.";

                return View(
                    new List<BroodReportListViewModel>());
            }
        }


        /*
         * GET: BroodReports/Create
         *
         * Displays the form used to generate
         * a new historical Brood Report.
         */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation(
                "Entering BroodReportsController.Create() GET.");

            try
            {
                var model =
                    await _broodReportService
                        .GetCreateViewModelAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Brood Report Create.");

                TempData["ErrorMessage"] =
                    "No se pudo cargar el formulario para generar el reporte de camada. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }


        /*
         * POST: BroodReports/Create
         *
         * Receives the selected Broiler House, Brood
         * and report number and delegates the historical
         * report generation to BroodReportService.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BroodReportFormViewModel model)
        {
            _logger.LogInformation(
                "Entering BroodReportsController.Create() POST. " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "BroodId: {BroodId}, " +
                "ReportNumber: {ReportNumber}",
                model.BroilerHouseId,
                model.BroodId,
                model.ReportNumber);

            /*
             * Model Validation | Create Form
             */
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Brood Report Create form contains invalid information. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "ReportNumber: {ReportNumber}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.ReportNumber);

                await _broodReportService
                    .PopulateFormOptionsAsync(
                        model);

                return View(
                    model);
            }

            try
            {
                /*
                 * Business Operation | Create Brood Report
                 */
                var broodReportId =
                    await _broodReportService
                        .CreateAsync(model);

                _logger.LogInformation(
                    "Brood Report generated successfully. " +
                    "BroodReportId: {BroodReportId}, " +
                    "BroodId: {BroodId}",
                    broodReportId,
                    model.BroodId);

                TempData["SuccessMessage"] =
                    "El reporte de camada fue generado correctamente.";

                /*
                 * Redirects directly to Details so the user
                 * can review the generated historical snapshot.
                 */
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = broodReportId
                    });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while generating Brood Report. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "ReportNumber: {ReportNumber}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.ReportNumber);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await _broodReportService
                    .PopulateFormOptionsAsync(
                        model);

                return View(
                    model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while generating Brood Report. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "ReportNumber: {ReportNumber}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.ReportNumber);

                TempData["ErrorMessage"] =
                    "No se pudo generar el reporte de camada. " +
                    "Intente nuevamente.";

                await _broodReportService
                    .PopulateFormOptionsAsync(
                        model);

                return View(
                    model);
            }
        }


        /*
         * GET: BroodReports/Details/5
         *
         * Retrieves and displays the historical
         * Brood Report identified by the received ID.
         */
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogInformation(
                "Entering BroodReportsController.Details(). " +
                "BroodReportId: {BroodReportId}",
                id);

            if (!id.HasValue)
            {
                _logger.LogWarning(
                    "Brood Report Details request received without an identifier.");

                TempData["ErrorMessage"] =
                    "No se proporcionó un identificador válido " +
                    "para consultar el reporte de camada.";

                return RedirectToAction(
                    nameof(Index));
            }

            try
            {
                var broodReport =
                    await _broodReportService
                        .GetByIdAsync(
                            id.Value);

                if (broodReport == null)
                {
                    _logger.LogWarning(
                        "Brood Report was not found while loading Details. " +
                        "BroodReportId: {BroodReportId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El reporte de camada seleccionado no existe.";

                    return RedirectToAction(
                        nameof(Index));
                }

                return View(
                    broodReport);
            }
            catch (InvalidOperationException ex)
            {
                /*
                 * This may occur, for example, if the historical
                 * snapshot cannot be deserialized correctly.
                 */
                _logger.LogWarning(
                    ex,
                    "Business validation failed while loading Brood Report Details. " +
                    "BroodReportId: {BroodReportId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    ex.Message;

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Brood Report Details. " +
                    "BroodReportId: {BroodReportId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    "No se pudo consultar el reporte de camada. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }


        /*
         * GET: BroodReports/GetBroodsByBroilerHouse
         *
         * Returns the active Broods associated with the
         * selected Broiler House.
         *
         * This endpoint is used by the Create view
         * to populate the Brood dropdown dynamically.
         */
        [HttpGet]
        public async Task<IActionResult> GetBroodsByBroilerHouse(
            int broilerHouseId)
        {
            _logger.LogInformation(
                "Entering BroodReportsController.GetBroodsByBroilerHouse(). " +
                "BroilerHouseId: {BroilerHouseId}",
                broilerHouseId);

            try
            {
                var broodOptions =
                    await _broodReportService
                        .GetBroodsByBroilerHouseAsync(
                            broilerHouseId);

                return Json(
                    broodOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Broods for Brood Report. " +
                    "BroilerHouseId: {BroilerHouseId}",
                    broilerHouseId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "No se pudieron cargar las camadas."
                    });
            }
        }
    }
}