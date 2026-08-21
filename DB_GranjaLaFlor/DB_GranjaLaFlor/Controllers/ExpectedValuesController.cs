using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGranjaLaFlor.Models.ViewModels.ExpectedValue;

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Architecture Decision | Thin Controller
     * Controllers coordinate HTTP requests and responses only.
     * Business rules and database operations are delegated
     * to the Service layer.
     */


    /*
     * Authorization | Expected Values Module
     *
     * All operational roles can access the Expected Values
     * module for consultation.
     *
     * Modification actions are restricted individually
     * to Propietario, Operario and SuperAdmin.
     */
    [Authorize(Roles = "Propietario,Operario,Administrador,SuperAdmin")]
    public class ExpectedValuesController : Controller
    {
        private readonly ExpectedValueService _expectedValueService;
        private readonly ILogger<ExpectedValuesController> _logger;

        public ExpectedValuesController(
            ExpectedValueService expectedValueService,
            ILogger<ExpectedValuesController> logger)
        {
            _expectedValueService = expectedValueService;
            _logger = logger;
        }

        /*
         * GET: ExpectedValues: Retrieves and displays the six fixed Expected Value records.
         */
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering ExpectedValuesController.Index().");

            try
            {
                var expectedValues = await _expectedValueService.GetAllAsync();

                return View(expectedValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading Expected Values.");

                TempData["ErrorMessage"] = "No se pudieron consultar los valores esperados. " + "Intente nuevamente.";

                return View(
                    new List<ExpectedValueListViewModel>());
            }
        }

        /*
         * GET: ExpectedValues/Edit/5
         * Retrieves and displays the selected Expected Value
         * record required by the Edit form.
         */
        [Authorize(Roles = "Propietario,Operario,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogInformation(
                "Entering ExpectedValuesController.Edit() GET. " + "ExpectedValueId: {ExpectedValueId}", id
                );

            if (!id.HasValue)
            {
                _logger.LogWarning(
                    "Expected Value Edit request received " + "without an identifier.");

                TempData["ErrorMessage"] = "No se proporcionó un identificador válido " + "para editar el valor esperado.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                var model = await _expectedValueService.GetFormByIdAsync(id.Value);

                if (model == null)
                {
                    _logger.LogWarning(
                        "Expected Value was not found while loading Edit. " + "ExpectedValueId: {ExpectedValueId}", id.Value
                        );

                    TempData["ErrorMessage"] = "El valor esperado seleccionado no existe.";

                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading Expected Value Edit. " + "ExpectedValueId: {ExpectedValueId}",
                    id.Value);

                TempData["ErrorMessage"] =
                    "No se pudo cargar la información del valor esperado. " + "Intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        /*
         * POST: ExpectedValues/Edit/5
         * Receives the updated values and delegates their validation
         * and persistence to the Service layer.
         */
        [Authorize(Roles = "Propietario,Operario,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpectedValueFormViewModel model)
        {
            _logger.LogInformation("Entering ExpectedValuesController.Edit() POST. " + "ExpectedValueId: {ExpectedValueId}", model.ExpectedValueId
                );

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _expectedValueService.UpdateAsync(model);

                TempData["SuccessMessage"] = "El valor esperado fue actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while updating " + "Expected Value. ExpectedValueId: {ExpectedValueId}",
                    model.ExpectedValueId);

                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating Expected Value. " +  "ExpectedValueId: {ExpectedValueId}",
                    model.ExpectedValueId);

                TempData["ErrorMessage"] =
                    "No se pudo actualizar el valor esperado. " + "Intente nuevamente.";

                return View(model);
            }
        }
    }
}