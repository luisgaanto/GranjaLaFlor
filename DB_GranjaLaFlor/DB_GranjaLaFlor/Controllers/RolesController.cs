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
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // [HttpPost] = Allow POST requests only
        // Micro recommendation to proetc against CSRF attacks (Cross-Site Request Forgery) when using POST methods and MVC. 
        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            await _roleService.CreateAsync(role);

            return RedirectToAction(nameof(Index));
        }

        

        // GET: Roles/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Role role)
        {
            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Delete/5
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: Roles/Delete/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Role role)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
