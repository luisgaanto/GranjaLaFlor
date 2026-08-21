using DB_GranjaLaFlor.Models.Entities;
using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

         /*
         *_logger is for debugging, for admin QA and Testing. 
         *TempData is for user notification. 
         */

namespace DB_GranjaLaFlor.Controllers
{
    /*
     * Authorization | Roles Management:  Enables/enforces authentication/authentication to controllers that need to be proteced.
     * Only users with the SuperAdmin role can access the Roles management module.
     * ASP.NET Core validates the role using the Role Claim created during the authentication process.
     */
    [Authorize(Roles = "SuperAdmin")]

    /*
     * RolesController inherits from Controller --> ASP.NET Core MVC. --> provides features such as methods: view(), 
     * RedirectToAction()..., Properties: ModelState, TempData..., or Action Methods: Index(), Create()...
    */
    public class RolesController : Controller
    {
        // variable from RoleService where logic happens.
        private readonly RoleService _roleService;
        private readonly ILogger<RolesController> _logger; // ILogger: triggering logs for events 

        /*
         * Creating construtor based on below method. So constructor is "RolesController" is made of:
         * 1- Get service as a parameter: roleService where the logic happens and 2-Get logger as a parameter: to get logs for events. 
         * RoleService is registered in the Dependency Injection container using: builder.Services.AddScoped<RoleService>();.
         * When ASP.NET Core creates RolesController, the RoleService dependency is resolved from the Dependency Injection container
         *  and injected into the constructor.
         */
        public RolesController(
            RoleService roleService,
            ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        // IActionResult = Recommended option based on doc since it provides some flexibility when returning view, redirection....

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering RolesController.Index().");

            var activeRoles = await _roleService.GetAllActiveAsync();

            _logger.LogInformation(
                "RolesController.Index() loaded {RoleCount} active roles.",
                activeRoles.Count);

            return View(activeRoles);
        }

        // GET: Roles/Inactive
        public async Task<IActionResult> Inactive()
        {
            _logger.LogInformation("Entering RolesController.Inactive().");

            var inactiveRoles = await _roleService.GetAllInactiveAsync();

            _logger.LogInformation(
                "RolesController.Index() loaded {RoleCount} active roles.",
                inactiveRoles.Count);

            return View(inactiveRoles);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation(
                "Entering RolesController.Details(). RoleId: {RoleId}",
                id);

            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                _logger.LogWarning(
                    "RolesController.Details() role not found. RoleId: {RoleId}",
                    id);

                return NotFound();
            }

            _logger.LogInformation(
                "RolesController.Details() loaded role. RoleId: {RoleId}, RoleName: {RoleName}, RoleDescription: {RoleDescription}, RoleState: {RoleState}",
                role.RoleId,
                role.RoleName,
                role.RoleDescription, 
                role.RoleState);

            return View(role);
        }


        // GET: Roles/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Entering RolesController.Create() GET.");

            return View();
        }

        // [HttpPost] = Allow POST requests only
        // Micro recommendation to proetc against CSRF attacks (Cross-Site Request Forgery) when using POST methods and MVC. 
        // POST: Roles/Create[HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            _logger.LogInformation(
                "Entering RolesController.Create() POST. RoleName: {RoleName}",
                role.RoleName);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "RolesController.Create() POST validation failed. RoleName: {RoleName}",
                    role.RoleName);

                return View(role);
            }

            try
            {
                _logger.LogInformation(
                    "Calling RoleService.CreateAsync(). RoleName: {RoleName}",
                    role.RoleName);

                await _roleService.CreateAsync(role);

                _logger.LogInformation(
                    "Role created successfully. RoleName: {RoleName},  RoleDescription: {RoleDescription}, RoleState: {RoleState}",
                    role.RoleName,
                    role.RoleDescription,
                    role.RoleState);

                TempData["SuccessMessage"] = "El rol fue registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business validation failed while creating role. RoleName: {RoleName}",
                    role.RoleName);

                TempData["ErrorMessage"] = ex.Message;

                return View(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating role. RoleName: {RoleName}",
                    role.RoleName);

                TempData["ErrorMessage"] = "No fue posible registrar el rol. Intente nuevamente.";

                return View(role);
            }
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation(
                "Entering RolesController.Edit() GET. RoleId: {RoleId}",
                id);

            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                _logger.LogWarning(
                    "RolesController.Edit() GET role not found. RoleId: {RoleId}",
                    id);

                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role)
        {
            _logger.LogInformation(
                "Entering RolesController.Edit() POST. RouteRoleId: {RouteRoleId}, FormRoleId: {FormRoleId}, RoleName: {RoleName}",
                id,
                role.RoleId,
                role.RoleName);

            if (id != role.RoleId)
            {
                _logger.LogWarning(
                    "RolesController.Edit() POST id mismatch. RouteRoleId: {RouteRoleId}, FormRoleId: {FormRoleId}",
                    id,
                    role.RoleId);

                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "RolesController.Edit() POST validation failed. RoleId: {RoleId}, RoleName: {RoleName}",
                    role.RoleId,
                    role.RoleName);

                return View(role);
            }

            try
            {
                _logger.LogInformation(
                    "Calling RoleService.UpdateAsync(). RoleId: {RoleId}, RoleName: {RoleName}, RoleDescription: {RoleDescription}, RoleState: {RoleState}",
                    role.RoleId,
                    role.RoleName,
                    role.RoleDescription,
                    role.RoleState);

                await _roleService.UpdateAsync(role);

                _logger.LogInformation(
                    "Role updated successfully. RoleId: {RoleId}, RoleName: {RoleName}",
                    role.RoleId,
                    role.RoleName);

                TempData["SuccessMessage"] = "El rol fue actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business validation failed while updating role. RoleId: {RoleId}, RoleName: {RoleName}",
                    role.RoleId,
                    role.RoleName);

                TempData["ErrorMessage"] = ex.Message;

                return View(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating role. RoleId: {RoleId}, RoleName: {RoleName}",
                    role.RoleId,
                    role.RoleName);

                TempData["ErrorMessage"] = "No fue posible actualizar el rol. Intente nuevamente.";

                return View(role);
            }
        }


        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Entering RolesController.Delete() GET. RoleId: {RoleId}",
                id);

            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                _logger.LogWarning(
                    "RolesController.Delete() GET role not found. RoleId: {RoleId}, RoleName: {RoleName}",
                    id,
                    role.RoleName);

                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ASP.NET Core MVC : Doc recommends to use "ActionName" when Post method contains same parameters as Get method. 
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
    int id)
        {
            _logger.LogInformation(
                "Entering RolesController.Delete() POST. " +
                "RoleId: {RoleId}",
                id);

            try
            {
                await _roleService
                    .SoftDeleteAsync(id);

                _logger.LogInformation(
                    "Role deactivated successfully. " +
                    "RoleId: {RoleId}",
                    id);

                TempData["SuccessMessage"] =
                    "El rol fue desactivado correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business rule validation failed while deactivating " +
                    "Role. RoleId: {RoleId}",
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
                    "Unexpected error while deactivating Role. " +
                    "RoleId: {RoleId}",
                    id);

                TempData["ErrorMessage"] =
                    "No fue posible desactivar el rol. " +
                    "Intente nuevamente.";

                return RedirectToAction(
                    nameof(Index));
            }
        }

        // GET: Roles/Activate/5
        public async Task<IActionResult> Activate(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Activate")]
        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            _logger.LogInformation(
                "Entering RolesController.Activate() POST. RoleId: {RoleId}",
                id);

            try
            {
                await _roleService.ActivateAsync(id);

                _logger.LogInformation(
                    "Role activated successfully. RoleId: {RoleId}",
                    id);

                TempData["SuccessMessage"] = "El rol fue reactivado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while activating role. RoleId: {RoleId}",
                    id);

                TempData["ErrorMessage"] = "No fue posible reactivar el rol. Intente nuevamente.";

                return RedirectToAction(nameof(Activate), new { id });
            }
        }




    }
}
