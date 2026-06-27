using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DB_GranjaLaFlor.Controllers
{
    // Enables/enforces authentication/authentication to controllers that need to be proteced.  
    [Authorize(Roles = "Propietario")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserService userService,
            RoleService roleService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        // Use only in thos class by Create GET
        private async Task LoadRolesAsync()
        {
            var activeRoles = await _roleService.GetAllActiveAsync();

            ViewBag.Roles = new SelectList(
                activeRoles,
                "RoleId",
                "RoleName");
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering UsersController.Index().");

            var activeUsers = await _userService.GetAllActiveAsync();

            _logger.LogInformation(
                "UsersController.Index() loaded {UserCount} active users.",
                activeUsers.Count);

            return View(activeUsers);
        }

        // To retrieve Active Roles from RoleService.
        // Using ViewBag as it is simple and we are not applying complex ViewModel logic yet.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation(
                "Entering UsersController.Create() GET.");

            await LoadRolesAsync();

            _logger.LogInformation(
                "Active roles loaded for the Create User view.");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            _logger.LogInformation(
                "Entering UsersController.Create() POST. UserName: {UserName}, UserEmail: {UserEmail}, RoleId: {RoleId}",
                user.UserName,
                user.UserEmail,
                user.RoleId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "UsersController.Create() POST validation failed. UserName: {UserName}, UserEmail: {UserEmail}",
                    user.UserName,
                    user.UserEmail);

                await LoadRolesAsync();

                return View(user);
            }

            try
            {
                await _userService.CreateAsync(user);

                TempData["SuccessMessage"] = "El usuario fue registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business validation failed while creating user. UserEmail: {UserEmail}",
                    user.UserEmail);

                TempData["ErrorMessage"] = ex.Message;

                await LoadRolesAsync();

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating user. UserEmail: {UserEmail}",
                    user.UserEmail);

                TempData["ErrorMessage"] = "No fue posible registrar el usuario. Intente nuevamente.";

                await LoadRolesAsync();

                return View(user);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation(
                "Entering UsersController.Details(). UserId: {UserId}",
                id);

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning(
                    "UsersController.Details() user not found. UserId: {UserId}",
                    id);

                return NotFound();
            }

            _logger.LogInformation(
                "UsersController.Details() loaded user. UserId: {UserId}, UserEmail: {UserEmail}",
                user.UserId,
                user.UserEmail);

            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation(
                "Entering UsersController.Edit() GET. UserId: {UserId}",
                id);

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning(
                    "UsersController.Edit() GET user not found. UserId: {UserId}",
                    id);

                return NotFound();
            }

            await LoadRolesAsync();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            _logger.LogInformation(
                "Entering UsersController.Edit() POST. RouteUserId: {RouteUserId}, FormUserId: {FormUserId}, UserEmail: {UserEmail}",
                id,
                user.UserId,
                user.UserEmail);

            if (id != user.UserId)
            {
                _logger.LogWarning(
                    "UsersController.Edit() POST id mismatch. RouteUserId: {RouteUserId}, FormUserId: {FormUserId}",
                    id,
                    user.UserId);

                return BadRequest();
            }

            // Password is managed through a separate Password Recovery process.
            // Remove password validation from ModelState to avoid validation errors.
            ModelState.Remove(nameof(user.UserPassword));
            ModelState.Remove(nameof(user.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "UsersController.Edit() POST validation failed. UserId: {UserId}, UserEmail: {UserEmail}",
                    user.UserId,
                    user.UserEmail);

                await LoadRolesAsync();

                return View(user);
            }

            try
            {
                await _userService.UpdateAsync(user);

                TempData["SuccessMessage"] = "El usuario fue actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Business validation failed while updating user. UserId: {UserId}, UserEmail: {UserEmail}",
                    user.UserId,
                    user.UserEmail);

                TempData["ErrorMessage"] = ex.Message;

                await LoadRolesAsync();

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while updating user. UserId: {UserId}, UserEmail: {UserEmail}",
                    user.UserId,
                    user.UserEmail);

                TempData["ErrorMessage"] = "No fue posible actualizar el usuario. Intente nuevamente.";

                await LoadRolesAsync();

                return View(user);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Entering UsersController.Delete() GET. UserId: {UserId}",
                id);

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning(
                    "UsersController.Delete() GET user not found. UserId: {UserId}",
                    id);

                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Entering UsersController.Delete() POST. UserId: {UserId}",
                id);

            try
            {
                await _userService.SoftDeleteAsync(id);

                _logger.LogInformation(
                    "User deactivated successfully. UserId: {UserId}",
                    id);

                TempData["SuccessMessage"] = "El usuario fue eliminado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while deactivating user. UserId: {UserId}",
                    id);

                TempData["ErrorMessage"] = "No fue posible eliminar el usuario. Intente nuevamente.";

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        public async Task<IActionResult> Inactive()
        {
            _logger.LogInformation(
                "Entering UsersController.Inactive().");

            var inactiveUsers = await _userService.GetAllInactiveAsync();

            _logger.LogInformation(
                "UsersController.Inactive() loaded {UserCount} inactive users.",
                inactiveUsers.Count);

            return View(inactiveUsers);
        }


        public async Task<IActionResult> Activate(int id)
        {
            _logger.LogInformation(
                "Entering UsersController.Activate() GET. UserId: {UserId}",
                id);

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning(
                    "UsersController.Activate() GET user not found. UserId: {UserId}",
                    id);

                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Activate")]
        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            _logger.LogInformation(
                "Entering UsersController.Activate() POST. UserId: {UserId}",
                id);

            try
            {
                await _userService.ActivateAsync(id);

                _logger.LogInformation(
                    "User activated successfully. UserId: {UserId}",
                    id);

                TempData["SuccessMessage"] = "El usuario fue reactivado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while activating user. UserId: {UserId}",
                    id);

                TempData["ErrorMessage"] = "No fue posible reactivar el usuario. Intente nuevamente.";

                return RedirectToAction(nameof(Activate), new { id });
            }
        }




    }
}