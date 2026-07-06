using DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates;
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

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(
                "Entering IncomeConcentratesController.Index().");

            var incomes = await _incomeConcentrateService.GetAllActiveAsync();

            return View(incomes);
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
          * UI Support | Concentrate Calculation
          * Returns the current accumulated concentrate for the selected brood.This endpoint is called asynchronously from 
          * the Create view to display the estimated accumulated amount before the record is saved.
         */
        [HttpGet]
        public async Task<IActionResult> GetCurrentAccumulatedByBrood(int broodId)
        {
            var accumulated =
                await _incomeConcentrateService.GetCurrentAccumulatedByBroodAsync(broodId);

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
    }
}