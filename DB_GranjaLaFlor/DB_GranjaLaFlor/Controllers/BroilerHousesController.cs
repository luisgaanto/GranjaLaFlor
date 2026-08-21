using DB_GranjaLaFlor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DB_GranjaLaFlor.Controllers
{
    [Authorize(Roles = "Propietario,Operario,Administrador,SuperAdmin")]
    public class BroilerHousesController : Controller
    {
        private readonly BroilerHouseService _broilerHouseService;

        public BroilerHousesController(
            BroilerHouseService broilerHouseService)
        {
            _broilerHouseService = broilerHouseService;
        }

        /*
         * Displays all active Broiler Houses.
         * This module is read-only.
         */
        public async Task<IActionResult> Index()
        {
            var broilerHouses =
                await _broilerHouseService.GetAllActiveAsync();

            return View(broilerHouses);
        }
    }
}