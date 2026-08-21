using DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates;
using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGranjaLaFlor.ViewModels.IncomeConcentrates;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Architecture Decision | Thin Controller
     * Controllers coordinate HTTP requests and responses only.
     * Business rules, calculations and database operations are delegated
     * to the Service layer.
     */
    [Authorize]
    public class IncomeConcentratesController : Controller
    {
        private readonly IncomeConcentrateService _incomeConcentrateService;
        private readonly ILogger<IncomeConcentratesController> _logger;

        public IncomeConcentratesController(
            IncomeConcentrateService incomeConcentrateService,
            ILogger<IncomeConcentratesController> logger)
        {
            _incomeConcentrateService = incomeConcentrateService;
            _logger = logger;
        }
        /*
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Index().");

            var incomes = await _incomeConcentrateService.GetAllActiveAsync();

            return View(incomes);
        }
        */

        [HttpGet]
        public async Task<IActionResult> Index(string? broodName, int? year, int? broilerHouseId)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Index(). " +
                "broodName: {broodName}, Year: {Year}, " +
                "BroilerHouseId: {BroilerHouseId}",
                broodName,
                year,
                broilerHouseId);

            /*
             * Delegates the Income Concentrate records, filter values and dropdown menu options to the Service layer.
             * The controller only coordinates the HTTP request and returns the completed ViewModel to the Index view.
             */
            var model =
                await _incomeConcentrateService.GetFilterViewModelAsync(
                    broodName,
                    year,
                    broilerHouseId);

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Details(). IncomeConcentrateId: {IncomeConcentrateId}",
                id);

            var income = await _incomeConcentrateService.GetByIdAsync(id);

            if (income == null)
            {
                TempData["ErrorMessage"] = "Ingreso de concentrado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(income);
        }


        /*
          * UI Support | Concentrate Calculation: Returns the current accumulated concentrate for the selected Brood.
          * In Edit mode, the current record can be excluded to prevent duplicate calculation in the accumulated preview.
         */
        [HttpGet]
        public async Task<IActionResult> GetCurrentAccumulatedByBrood(
            int broodId,
            int? excludeIncomeConcentrateId = null)
        {
            var accumulated =
                await _incomeConcentrateService.GetCurrentAccumulatedByBroodAsync(
                    broodId,
                    excludeIncomeConcentrateId);

            return Json(accumulated);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Create() GET.");

            var model = new IncomeConcentrateFormViewModel
            {
                IncomeConcentrateDate = DateTime.Today,
                Broods = await _incomeConcentrateService.GetBroodSelectListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeConcentrateFormViewModel model)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Create() POST. BroodId: {BroodId}, IncomeQuintals: {IncomeQuintals}",
                model.BroodId,
                model.IncomeQuintals);

            if (!ModelState.IsValid)
            {
                model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();
                return View(model);
            }

            try
            {
                await _incomeConcentrateService.CreateAsync(model);

                TempData["SuccessMessage"] = "El ingreso de concentrado fue registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while creating income concentrate.");

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating income concentrate.");

                TempData["ErrorMessage"] = "No se pudo registrar el ingreso de concentrado. Intente nuevamente.";

                model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Edit() GET. IncomeConcentrateId: {IncomeConcentrateId}",
                id);

            var model = await _incomeConcentrateService.GetFormByIdAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Ingreso de concentrado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IncomeConcentrateFormViewModel model)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Edit() POST. RouteId: {RouteId}, FormId: {FormId}, BroodId: {BroodId}, IncomeQuintals: {IncomeQuintals}",
                id,
                model.IncomeConcentrateId,
                model.BroodId,
                model.IncomeQuintals);

            if (id != model.IncomeConcentrateId)
            {
                TempData["ErrorMessage"] = "Solicitud inválida.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();
                return View(model);
            }

            try
            {
                await _incomeConcentrateService.UpdateAsync(model);

                TempData["SuccessMessage"] = "El ingreso de concentrado fue actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            // catches bunisness rule erros in service and show them.
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while updating income concentrate. IncomeConcentrateId: {IncomeConcentrateId}",
                    model.IncomeConcentrateId);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating income concentrate. IncomeConcentrateId: {IncomeConcentrateId}",
                    model.IncomeConcentrateId);

                TempData["ErrorMessage"] = "No se pudo actualizar el ingreso de concentrado. Intente nuevamente.";

                model.Broods = await _incomeConcentrateService.GetBroodSelectListAsync();

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var income = await _incomeConcentrateService.GetByIdAsync(id);

            if (income == null)
            {
                TempData["ErrorMessage"] = "Ingreso de concentrado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(income);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _incomeConcentrateService.SoftDeleteAsync(id);

                TempData["SuccessMessage"] = "El ingreso de concentrado fue desactivado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while deactivating " +
                    "Income Concentrate. IncomeConcentrateId: {IncomeConcentrateId}",
                    id);
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(Index));
                //return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while deactivating income concentrate. IncomeConcentrateId: {IncomeConcentrateId}",
                    id);

                TempData["ErrorMessage"] = "No se pudo desactivar el ingreso de concentrado. Intente nuevamente.";

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        public async Task<IActionResult> Inactive()
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Inactive().");

            var inactiveIncomes =
                await _incomeConcentrateService.GetAllInactiveAsync();

            return View(inactiveIncomes);
        }

        [HttpGet]
        public async Task<IActionResult> Activate(int id)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Activate() GET. IncomeConcentrateId: {IncomeConcentrateId}",
                id);

            var income = await _incomeConcentrateService.GetByIdAsync(id);

            if (income == null)
            {
                TempData["ErrorMessage"] = "Ingreso de concentrado no encontrado.";
                return RedirectToAction(nameof(Inactive));
            }

            return View(income);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.ActivateConfirmed() POST. IncomeConcentrateId: {IncomeConcentrateId}",
                id);

            try
            {
                await _incomeConcentrateService.ActivateAsync(id);

                TempData["SuccessMessage"] = "El ingreso de concentrado fue reactivado correctamente.";

                return RedirectToAction(nameof(Inactive));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while activating income concentrate. IncomeConcentrateId: {IncomeConcentrateId}",
                    id);

                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(Activate), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while activating income concentrate. IncomeConcentrateId: {IncomeConcentrateId}",
                    id);

                TempData["ErrorMessage"] = "No se pudo reactivar el ingreso de concentrado. Intente nuevamente.";

                return RedirectToAction(nameof(Activate), new { id });
            }
        }


    }
}