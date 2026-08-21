using DB_GranjaLaFlor.Models.ViewModels.Broods;
using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Architecture Decision | Thin Controller
     * Controllers coordinate HTTP requests and responses only.
     * Business rules and database operations are delegated to the Service layer.
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/controllers/actions
     */
    [Authorize]
    public class BroodsController : Controller
    {
        private readonly BroodService _broodService;
        private readonly ILogger<BroodsController> _logger;

        public BroodsController(
            BroodService broodService,
            ILogger<BroodsController> logger)
        {
            _broodService = broodService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(
                "Entering BroodsController.Index().");

            var activeBroods = await _broodService.GetAllActiveAsync();

            _logger.LogInformation(
                "BroodsController.Index() loaded {BroodCount} active broods.",
                activeBroods.Count);

            return View(activeBroods);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation(
                "Entering BroodsController.Create() GET.");

            var model = new BroodFormViewModel
            {
                BroodDate = DateTime.Today,
                BroodNames = BroodService.GetBroodNameSelectList(),
                BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BroodFormViewModel model)
        {
            _logger.LogInformation(
                "Entering BroodsController.Create() POST. BroodName: {BroodName}, BroilerHouseId: {BroilerHouseId}",
                model.BroodName,
                model.BroilerHouseId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "BroodsController.Create() POST validation failed. BroodName: {BroodName}",
                    model.BroodName);

                model.BroodNames = BroodService.GetBroodNameSelectList();
                model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();
                return View(model);
            }

            try
            {
                await _broodService.CreateAsync(model);

                _logger.LogInformation(
                    "Brood created successfully. BroodName: {BroodName}",
                    model.BroodName);

                TempData["SuccessMessage"] = "La camada fue registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            /*
              * Architecture Decision | Business Rule Handling:Business rule violations are thrown from the Service layer as
              * InvalidOperationException. Controllers capture those exceptions,add the message to ModelState and return the same
              * View so users can correct their input without losing the entered data. Reference:
              * https://learn.microsoft.com/aspnet/core/mvc/models/validation
              * https://learn.microsoft.com/dotnet/api/system.invalidoperationexception
             */
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while creating brood. BroodName: {BroodName}",
                    model.BroodName);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
                
                model.BroodNames = BroodService.GetBroodNameSelectList();
                model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating brood. BroodName: {BroodName}",
                    model.BroodName);

                TempData["ErrorMessage"] = "No fue posible registrar la camada. Intente nuevamente.";

                model.BroodNames = BroodService.GetBroodNameSelectList();
                model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();

                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation(
                "Entering BroodsController.Details(). BroodId: {BroodId}",
                id);

            var brood = await _broodService.GetByIdAsync(id);

            if (brood == null)
            {
                _logger.LogWarning(
                    "Brood not found. BroodId: {BroodId}",
                    id);

                return NotFound();
            }

            return View(brood);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation(
                "Entering BroodsController.Edit() GET. BroodId: {BroodId}",
                id);

            var model = await _broodService.GetFormByIdAsync(id);

            if (model == null)
            {
                _logger.LogWarning(
                    "Brood not found for Edit. BroodId: {BroodId}",
                    id);

                return NotFound();
            }

            model.BroodNames = BroodService.GetBroodNameSelectList();
            model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BroodFormViewModel model)
        {
            _logger.LogInformation(
                "Entering BroodsController.Edit() POST. RouteBroodId: {RouteBroodId}, FormBroodId: {FormBroodId}, BroodName: {BroodName}",
                id,
                model.BroodId,
                model.BroodName);

            if (id != model.BroodId)
            {
                return BadRequest();
            }

            ModelState.Remove(nameof(BroodFormViewModel.BroodDate));

            if (!ModelState.IsValid)
            {
                model.BroodNames = BroodService.GetBroodNameSelectList();
                model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();

                return View(model);
            }

            try
            {
                await _broodService.UpdateAsync(model);

                TempData["SuccessMessage"] = "La camada fue actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            /*
              * Architecture Decision | Business Rule Handling:Business rule violations are thrown from the Service layer as
              * InvalidOperationException. Controllers capture those exceptions,add the message to ModelState and return the same
              * View so users can correct their input without losing the entered data. Reference:
              * https://learn.microsoft.com/aspnet/core/mvc/models/validation
              * https://learn.microsoft.com/dotnet/api/system.invalidoperationexception
             */
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while updating Brood. " +
                    "BroodId: {BroodId}, BroodName: {BroodName}",
                    model.BroodId,
                    model.BroodName);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                model.BroodNames = BroodService.GetBroodNameSelectList();
                model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating brood. BroodId: {BroodId}, BroodName: {BroodName}",
                    model.BroodId,
                    model.BroodName);

                TempData["ErrorMessage"] = "No fue posible actualizar la camada. Intente nuevamente.";

                model.BroodNames = BroodService.GetBroodNameSelectList();
                model.BroilerHouses = await _broodService.GetBroilerHouseSelectListAsync();

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Entering BroodsController.Delete() GET. BroodId: {BroodId}",
                id);

            var brood = await _broodService.GetByIdAsync(id);

            if (brood == null)
            {
                return NotFound();
            }

            return View(brood);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _broodService
                    .SoftDeleteAsync(id);

                TempData["SuccessMessage"] =
                    "La camada fue desactivada correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while deactivating " +
                    "Brood. BroodId: {BroodId}",
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
                    "Unexpected error while deactivating Brood. " +
                    "BroodId: {BroodId}",
                    id);

                TempData["ErrorMessage"] =
                    "No fue posible desactivar la camada. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }

        public async Task<IActionResult> Inactive()
        {
            _logger.LogInformation(
                "Entering BroodsController.Inactive().");

            var inactiveBroods = await _broodService.GetAllInactiveAsync();

            return View(inactiveBroods);
        }

        [HttpGet]
        public async Task<IActionResult> Activate(int id)
        {
            _logger.LogInformation(
                "Entering BroodsController.Activate() GET. BroodId: {BroodId}",
                id);

            var brood = await _broodService.GetByIdAsync(id);

            if (brood == null)
            {
                return NotFound();
            }

            return View(brood);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            try
            {
                await _broodService.ActivateAsync(id);

                TempData["SuccessMessage"] = "La camada fue activada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "No fue posible activar la camada. Intente nuevamente.";

                return RedirectToAction(nameof(Inactive));
            }
        }

    }
}