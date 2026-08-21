using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Architecture Decision | Thin Controller
     * Controllers coordinate HTTP requests and responses only.
     * Business rules, calculations and database operations are delegated
     * to the Service layer.
     */
    [Authorize]
    public class WeeklyChecksController : Controller
    {
        private readonly WeeklyCheckService _weeklyCheckService;
        private readonly ILogger<WeeklyChecksController> _logger;

        public WeeklyChecksController(
            WeeklyCheckService weeklyCheckService,
            ILogger<WeeklyChecksController> logger)
        {
            _weeklyCheckService = weeklyCheckService;
            _logger = logger;
        }

        /*
         * UI Request | Weekly Check Index
         * Receives the optional filters selected by the user and delegates
         * the retrieval of records and dropdown options to the Service layer.
         */
        [HttpGet]
        public async Task<IActionResult> Index(int? broodId,int? year,int? broilerHouseId,string? weeklyCheckWeek)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Index(). " +
                "BroodId: {BroodId}, " +
                "Year: {Year}, " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "WeeklyCheckWeek: {WeeklyCheckWeek}",
                broodId,
                year,
                broilerHouseId,
                weeklyCheckWeek);

            try
            {
                /*
                 * Delegates the Weekly Check records, current filter values
                 * and dropdown menu options to the Service layer.
                 */
                var model =
                    await _weeklyCheckService
                        .GetFilterViewModelAsync(
                            broodId,
                            year,
                            broilerHouseId,
                            weeklyCheckWeek);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Weekly Check Index.");

                TempData["ErrorMessage"] =
                    "No se pudo cargar la información de controles semanales. " +
                    "Intente nuevamente.";

                return View(
                    new WeeklyCheckFilterViewModel());
            }
        }

        /*
         * GET: WeeklyChecks/Create
         * Displays the Create view and loads the information
         * required to register a new Weekly Check.
         */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Create() GET.");

            try
            {
                var model =
                    await _weeklyCheckService
                        .GetCreateViewModelAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Weekly Check Create.");

                TempData["ErrorMessage"] =
                    "No se pudo cargar el formulario de control semanal. " +
                    "Intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
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
                "Entering WeeklyChecksController.GetBroodsByBroilerHouse(). " +
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

            try
            {
                var broodOptions =
                    await _weeklyCheckService
                        .GetBroodsByBroilerHouseAsync(
                            broilerHouseId);

                return Json(
                    broodOptions.Select(option => new
                    {
                        value = option.Value,
                        text = option.Text
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while retrieving Broods. " +
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

        /*
         * UI Request | Weekly Check Information
         * Retrieves the selected Brood, Expected Values and seven
         * Daily Checks required to preview the Weekly Check calculations.
         */
        [HttpGet]
        public async Task<IActionResult> GetWeeklyCheckInformation(
            int broilerHouseId,
            int broodId,
            string weeklyCheckWeek,
            decimal totalBirdWeight = 0)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.GetWeeklyCheckInformation(). " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "BroodId: {BroodId}, " +
                "WeeklyCheckWeek: {WeeklyCheckWeek}, " +
                "TotalBirdWeight: {TotalBirdWeight}",
                broilerHouseId,
                broodId,
                weeklyCheckWeek,
                totalBirdWeight);

            if (broilerHouseId <= 0 ||
                broodId <= 0 ||
                string.IsNullOrWhiteSpace(
                    weeklyCheckWeek))
            {
                _logger.LogWarning(
                    "Invalid information received while retrieving " +
                    "Weekly Check information. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    broilerHouseId,
                    broodId,
                    weeklyCheckWeek);

                return BadRequest(new
                {
                    message =
                        "Debe seleccionar una pollera, camada y semana válidas."
                });
            }

            try
            {
                var weeklyCheckInformation =
                    await _weeklyCheckService
                        .GetWeeklyCheckInformationAsync(
                            broilerHouseId,
                            broodId,
                            weeklyCheckWeek,
                            totalBirdWeight);

                if (weeklyCheckInformation == null)
                {
                    _logger.LogWarning(
                        "Weekly Check information was not found. " +
                        "BroilerHouseId: {BroilerHouseId}, " +
                        "BroodId: {BroodId}, " +
                        "WeeklyCheckWeek: {WeeklyCheckWeek}",
                        broilerHouseId,
                        broodId,
                        weeklyCheckWeek);

                    return NotFound(new
                    {
                        message =
                            "La camada seleccionada no está disponible " +
                            "para la pollera indicada."
                    });
                }

                return Json(new
                {
                    weeklyCheckInformation.BroodBirdInitialNum,
                    weeklyCheckInformation.FinalDailyBirdBalance,
                    weeklyCheckInformation.FinalAccumulatedConsumption,
                    weeklyCheckInformation.FinalConcentrateBalance,
                    weeklyCheckInformation.FinalAccumulatedMortality,

                    weeklyCheckInformation.ExpectedValueId,

                    weeklyCheckInformation.WeeklyExpectedConsumption,
                    weeklyCheckInformation.WeeklyExpectedWeight,
                    weeklyCheckInformation.WeeklyExpectedConversion,
                    weeklyCheckInformation.WeeklyExpectedMortality,

                    weeklyCheckInformation.SampleBirdQuantity,
                    weeklyCheckInformation.AverageWeeklyWeight,
                    weeklyCheckInformation.WeeklyRealConsumption,
                    weeklyCheckInformation.WeeklyConsumptionDifference,
                    weeklyCheckInformation.WeeklyWeightDifference,
                    weeklyCheckInformation.WeeklyRealConversion,
                    weeklyCheckInformation.WeeklyConversionDifference,
                    weeklyCheckInformation.WeeklyRealMortality,
                    weeklyCheckInformation.WeeklyMortalityDifference,

                    dailyChecks =
                        weeklyCheckInformation.DailyChecks
                            .Select(dailyCheck => new
                            {
                                dailyCheck.DailyCheckId,
                                dailyCheck.DailyCheckDate,
                                dailyCheck.DailyCheckDay,
                                dailyCheck.DailyCheckWeek,
                                dailyCheck.TotalDailyMortality,
                                dailyCheck.AccumulatedMortality,
                                dailyCheck.DailyBirdBalance,
                                dailyCheck.IncomeAccumulated,
                                dailyCheck.ConsumptionKilos,
                                dailyCheck.AccumulatedConsumption,
                                dailyCheck.ConcentrateBalance
                            })
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while retrieving " +
                    "Weekly Check information. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    broilerHouseId,
                    broodId,
                    weeklyCheckWeek);

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while retrieving Weekly Check information. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    broilerHouseId,
                    broodId,
                    weeklyCheckWeek);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "No se pudo cargar la información del control semanal."
                    });
            }
        }

        /*
         * POST: WeeklyChecks/Create
         * Receives the Weekly Check information and delegates its
         * validation, calculations and persistence to the Service layer.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeeklyCheckFormViewModel model)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Create() POST. " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "BroodId: {BroodId}, " +
                "WeeklyCheckWeek: {WeeklyCheckWeek}",
                model.BroilerHouseId,
                model.BroodId,
                model.WeeklyCheckWeek);

            if (!ModelState.IsValid)
            {
                await _weeklyCheckService
                    .PopulateFormOptionsAsync(model);

                return View(model);
            }

            try
            {
                await _weeklyCheckService
                    .CreateAsync(model);

                TempData["SuccessMessage"] =
                    "El control semanal fue registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while creating " +
                    "Weekly Check. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.WeeklyCheckWeek);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await _weeklyCheckService
                    .PopulateFormOptionsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating Weekly Check. " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    model.BroilerHouseId,
                    model.BroodId,
                    model.WeeklyCheckWeek);

                TempData["ErrorMessage"] =
                    "No se pudo registrar el control semanal. " +
                    "Intente nuevamente.";

                await _weeklyCheckService
                    .PopulateFormOptionsAsync(model);

                return View(model);
            }
        }


        /*
         * GET: WeeklyChecks/Details/5
         * Retrieves and displays the complete information
         * of the selected Weekly Check.
         */
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Details(). " +
                "WeeklyCheckId: {WeeklyCheckId}",
                id);

            /*
             * Request Validation | Weekly Check ID
             * Confirms that a valid identifier was provided
             * before querying the Weekly Check.
             */
            if (!id.HasValue)
            {
                _logger.LogWarning(
                    "Weekly Check Details request received without an identifier.");

                TempData["ErrorMessage"] =
                    "No se proporcionó un identificador válido para consultar el control semanal.";

                return RedirectToAction(
                    nameof(Index));
            }

            try
            {
                /*
                 * Data Query | Weekly Check
                 * Retrieves the complete Weekly Check information
                 * required by the Details view.
                 */
                var weeklyCheck =
                    await _weeklyCheckService
                        .GetByIdAsync(
                            id.Value);

                if (weeklyCheck == null)
                {
                    _logger.LogWarning(
                        "Weekly Check was not found while loading Details. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El control semanal seleccionado no existe.";

                    return RedirectToAction(
                        nameof(Index));
                }

                _logger.LogInformation(
                    "Weekly Check Details loaded successfully. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id.Value);

                return View(
                    weeklyCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Weekly Check Details. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    "No se pudo consultar el detalle del control semanal. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }

        /*
         * GET: WeeklyChecks/Edit/5
         * Retrieves the selected Weekly Check and prepares
         * the form required by the Edit view.
         */
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Edit(). " +
                "WeeklyCheckId: {WeeklyCheckId}",
                id);

            /*
             * Request Validation | Weekly Check ID
             * Confirms that a valid identifier was provided
             * before loading the Edit form.
             */
            if (!id.HasValue)
            {
                _logger.LogWarning(
                    "Weekly Check Edit request received without an identifier.");

                TempData["ErrorMessage"] =
                    "No se proporcionó un identificador válido para editar el control semanal.";

                return RedirectToAction(
                    nameof(Index));
            }

            try
            {
                /*
                 * UI Data | Weekly Check Edit Form
                 * Retrieves the Weekly Check information and
                 * prepares the form model required by Edit.
                 */
                var model =
                    await _weeklyCheckService
                        .GetFormByIdAsync(
                            id.Value);

                if (model == null)
                {
                    _logger.LogWarning(
                        "Weekly Check was not found while loading Edit. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El control semanal seleccionado no existe.";

                    return RedirectToAction(
                        nameof(Index));
                }

                /*
                 * Business Validation | Weekly Check State
                 * Only active Weekly Checks can be edited.
                 */
                var weeklyCheck =
                    await _weeklyCheckService
                        .GetByIdAsync(
                            id.Value);

                if (weeklyCheck == null ||
                    !weeklyCheck.WeeklyCheckState)
                {
                    _logger.LogWarning(
                        "Inactive Weekly Check cannot be edited. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El control semanal seleccionado no está disponible para edición.";

                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation(
                    "Weekly Check Edit form loaded successfully. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id.Value);

                return View(
                    model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Weekly Check Edit form. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    "No se pudo cargar el control semanal para edición. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }

        /*
         * POST: WeeklyChecks/Edit/5
         * Receives the modified Weekly Check information and delegates
         * its validation, recalculation and persistence to the Service layer.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WeeklyCheckFormViewModel model)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Edit() POST. " +
                "WeeklyCheckId: {WeeklyCheckId}, " +
                "BroilerHouseId: {BroilerHouseId}, " +
                "BroodId: {BroodId}, " +
                "WeeklyCheckWeek: {WeeklyCheckWeek}",
                model.WeeklyCheckId,
                model.BroilerHouseId,
                model.BroodId,
                model.WeeklyCheckWeek);

            /*
             * Model Validation | Edit Form
             */
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Weekly Check Edit form contains invalid information. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    model.WeeklyCheckId);

                try
                {
                    model =
                        await _weeklyCheckService
                            .ReloadEditFormAsync(
                                model);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Weekly Check Edit information could not be completely " +
                        "reloaded after ModelState validation failure. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        model.WeeklyCheckId);

                    await _weeklyCheckService
                        .PopulateFormOptionsAsync(
                            model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Unexpected error while reloading Weekly Check Edit form " +
                        "after ModelState validation failure. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        model.WeeklyCheckId);

                    await _weeklyCheckService
                        .PopulateFormOptionsAsync(
                            model);
                }

                return View(
                    model);
            }

            try
            {
                /*
                 * Business Operation | Update Weekly Check
                 */
                await _weeklyCheckService
                    .UpdateAsync(
                        model);

                _logger.LogInformation(
                    "Weekly Check updated successfully. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    model.WeeklyCheckId);

                TempData["SuccessMessage"] =
                    "El control semanal fue actualizado correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while updating Weekly Check. " +
                    "WeeklyCheckId: {WeeklyCheckId}, " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    model.WeeklyCheckId,
                    model.BroilerHouseId,
                    model.BroodId,
                    model.WeeklyCheckWeek);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                try
                {
                    model =
                        await _weeklyCheckService
                            .ReloadEditFormAsync(
                                model);
                }
                catch (InvalidOperationException reloadException)
                {
                    _logger.LogWarning(
                        reloadException,
                        "Weekly Check Edit information could not be completely " +
                        "reloaded after business validation failure. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        model.WeeklyCheckId);

                    await _weeklyCheckService
                        .PopulateFormOptionsAsync(
                            model);
                }

                return View(
                    model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating Weekly Check. " +
                    "WeeklyCheckId: {WeeklyCheckId}, " +
                    "BroilerHouseId: {BroilerHouseId}, " +
                    "BroodId: {BroodId}, " +
                    "WeeklyCheckWeek: {WeeklyCheckWeek}",
                    model.WeeklyCheckId,
                    model.BroilerHouseId,
                    model.BroodId,
                    model.WeeklyCheckWeek);

                TempData["ErrorMessage"] =
                    "No se pudo actualizar el control semanal. " +
                    "Intente nuevamente.";

                try
                {
                    model =
                        await _weeklyCheckService
                            .ReloadEditFormAsync(
                                model);
                }
                catch (Exception reloadException)
                {
                    _logger.LogError(
                        reloadException,
                        "Unexpected error while reloading Weekly Check Edit form. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        model.WeeklyCheckId);

                    await _weeklyCheckService
                        .PopulateFormOptionsAsync(
                            model);
                }

                return View(
                    model);
            }
        }

        /*
         * GET: WeeklyChecks/Delete/5
         * Retrieves and displays the Weekly Check information
         * required to confirm its logical deactivation.
         */
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Delete() GET. " +
                "WeeklyCheckId: {WeeklyCheckId}",
                id);

            if (!id.HasValue)
            {
                _logger.LogWarning(
                    "Weekly Check Delete request received without an identifier.");

                TempData["ErrorMessage"] =
                    "No se proporcionó un identificador válido " +
                    "para desactivar el control semanal.";

                return RedirectToAction(
                    nameof(Index));
            }

            try
            {
                /*
                 * Data Query | Weekly Check
                 * Retrieves the complete Weekly Check information
                 * required by the Delete confirmation view.
                 */
                var weeklyCheck =
                    await _weeklyCheckService
                        .GetByIdAsync(
                            id.Value);

                if (weeklyCheck == null)
                {
                    _logger.LogWarning(
                        "Weekly Check was not found while loading Delete. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El control semanal seleccionado no existe.";

                    return RedirectToAction(
                        nameof(Index));
                }

                /*
                 * Business Validation | Weekly Check State
                 * Prevents an inactive Weekly Check from being
                 * displayed for deactivation.
                 */
                if (!weeklyCheck.WeeklyCheckState)
                {
                    _logger.LogWarning(
                        "Inactive Weekly Check received while loading Delete. " +
                        "WeeklyCheckId: {WeeklyCheckId}",
                        id.Value);

                    TempData["ErrorMessage"] =
                        "El control semanal seleccionado ya se encuentra inactivo.";

                    return RedirectToAction(
                        nameof(Index));
                }

                return View(
                    weeklyCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Weekly Check Delete. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    "No se pudo cargar la información del control semanal. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }

        /*
         * POST: WeeklyChecks/Delete/5
         * Delegates the logical deactivation of the selected
         * Weekly Check to the Service layer.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Entering WeeklyChecksController.Delete() POST. " +
                "WeeklyCheckId: {WeeklyCheckId}",
                id);

            try
            {
                /*
                 * Business Operation | Soft Delete Weekly Check
                 * Delegates the logical deactivation to the
                 * Weekly Check Service.
                 */
                await _weeklyCheckService
                    .SoftDeleteAsync(id);

                _logger.LogInformation(
                    "Weekly Check deactivated successfully. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id);

                TempData["SuccessMessage"] =
                    "El control semanal fue eliminado correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            //validations in service layer
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while deactivating " +
                    "Weekly Check. WeeklyCheckId: {WeeklyCheckId}",
                    id);

                TempData["ErrorMessage"] =
                    ex.Message;

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while deactivating Weekly Check. " +
                    "WeeklyCheckId: {WeeklyCheckId}",
                    id);

                TempData["ErrorMessage"] =
                    "No se pudo desactivar el control semanal. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }




    }
}