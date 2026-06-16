using DB_GranjaLaFlor.Models.Entities;
using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Mvc;

namespace DB_GranjaLaFlor.Controllers
{
    public class RolesController : Controller
    {

        private readonly RoleService _roleService;

        public RolesController(RoleService roleService)
        {
            _roleService = roleService;
        }

        // IActionResult = Recommended option based on doc dince it provides some flexibility when returning view, redirection....

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var activeRoles = await _roleService.GetAllActiveAsync();

            return View(activeRoles);
        }

        // GET: Roles/Inactive
        public async Task<IActionResult> Inactive()
        {
            var inactiveRoles = await _roleService.GetAllInactiveAsync();

            return View(inactiveRoles);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }


        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // [HttpPost] = Allow POST requests only
        // Micro recommendation to proetc against CSRF attacks (Cross-Site Request Forgery) when using POST methods and MVC. 
        // POST: Roles/Create[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            try
            {
                await _roleService.CreateAsync(role);

                TempData["SuccessMessage"] = "El rol fue registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "No fue posible registrar el rol. Intente nuevamente.";

                return View(role);
            }
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
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
        public async Task<IActionResult> Edit(int id, Role role)
        {
            if (id != role.RoleId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(role);
            }

            await _roleService.UpdateAsync(role);

            return RedirectToAction(nameof(Index));
        }





        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ASP.NET Core MVC : Doc recommends to use "ActionName" when Post method contains same parameters as Get method. 
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roleService.SoftDeleteAsync(id);

            return RedirectToAction(nameof(Index));
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
            await _roleService.ActivateAsync(id);

            return RedirectToAction(nameof(Inactive));
        }


    }
}
