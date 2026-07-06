using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using DB_GranjaLaFlor.Models.ViewModels.Broods;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Architecture Decision | Service Layer
     * Business logic and database access are implemented inside Services.
     * Controllers should coordinate HTTP requests and delegate data operations
     * to the Service layer.
     * Reference:
     * https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection
     */
    public class BroodService
    {
        private readonly ApplicationDbContext _context;

        public BroodService(ApplicationDbContext context)
        {
            _context = context;
        }


        private static string NormalizeText(string value)
        {
            return Regex.Replace(
                value.Trim(),
                @"\s+",
                " ");
        }

        public async Task<List<BroodListViewModel>> GetAllActiveAsync()
        {
            return await _context.Broods
                .AsNoTracking()
                .Include(brood => brood.BroilerHouse)
                .Where(brood => brood.BroodState)
                .OrderBy(brood => brood.BroodDate)
                .Select(brood => new BroodListViewModel
                {
                    BroodId = brood.BroodId,
                    BroodName = brood.BroodName,
                    BroodDate = brood.BroodDate,
                    BroodBirdInitialNum = brood.BroodBirdInitialNum,
                    BroodDescription = brood.BroodDescription,
                    BroodState = brood.BroodState,
                    BroilerHouseName = brood.BroilerHouse != null
                        ? brood.BroilerHouse.BroilerHouseName
                        : string.Empty
                })
                .Take(10) 
                .ToListAsync();
        }

        public async Task<BroodGetByIdViewModel?> GetByIdAsync(int id)
        {
            return await _context.Broods
                .AsNoTracking()
                .Include(brood => brood.BroilerHouse)
                .Where(brood => brood.BroodId == id)
                .Select(brood => new BroodGetByIdViewModel
                {
                    BroodId = brood.BroodId,
                    BroodName = brood.BroodName,
                    BroodDate = brood.BroodDate,
                    BroodBirdInitialNum = brood.BroodBirdInitialNum,
                    BroodDescription = brood.BroodDescription,
                    BroodState = brood.BroodState,
                    BroilerHouseId = brood.BroilerHouseId,
                    BroilerHouseName = brood.BroilerHouse != null
                        ? brood.BroilerHouse.BroilerHouseName
                        : string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<SelectListItem>> GetBroilerHouseSelectListAsync()
        {
            return await _context.BroilerHouses
                .AsNoTracking()
                .Where(broilerHouse => broilerHouse.BroilerHouseState)
                .OrderBy(broilerHouse => broilerHouse.BroilerHouseName)
                .Select(broilerHouse => new SelectListItem
                {
                    Value = broilerHouse.BroilerHouseId.ToString(),
                    Text = broilerHouse.BroilerHouseName
                })
                .ToListAsync();
        }

        public async Task CreateAsync(BroodFormViewModel model)
        {
            /*
             * Business Rule | Initial Bird Count: a Brood must start with at least one bird. Services contain business logic, 
             * while Controllers only coordinate HTTP requests. Reference: https://learn.microsoft.com/aspnet/core/mvc/overview
            */
            if (model.BroodBirdInitialNum <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad inicial de aves debe ser mayor que cero.");
            }

            var brood = new Brood
            {
                BroodName = NormalizeText(model.BroodName),
                BroodDate = DateTime.Today,
                BroodBirdInitialNum = model.BroodBirdInitialNum,
                BroodDescription = string.IsNullOrWhiteSpace(model.BroodDescription)
                    ? null
                    : NormalizeText(model.BroodDescription),
                BroodState = true,
                BroilerHouseId = model.BroilerHouseId
            };

            _context.Broods.Add(brood);
            await _context.SaveChangesAsync();
        }

        /*
         * UI Data | Returns the available Brood names used to populate
         * the Create/Edit dropdown list.
         */
        public static List<SelectListItem> GetBroodNameSelectList()
        {
            return Enumerable.Range(1, 7)
                .Select(number => new SelectListItem
                {
                    Value = $"Camada {number}",
                    Text = $"Camada {number}"
                })
                .ToList();
        }

        public async Task<BroodFormViewModel?> GetFormByIdAsync(int id)
        {
            return await _context.Broods
                .AsNoTracking()
                .Where(brood => brood.BroodId == id)
                .Select(brood => new BroodFormViewModel
                {
                    BroodId = brood.BroodId,
                    BroodName = brood.BroodName,
                    BroodDate = brood.BroodDate,
                    BroodBirdInitialNum = brood.BroodBirdInitialNum,
                    BroodDescription = brood.BroodDescription,
                    BroilerHouseId = brood.BroilerHouseId
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(BroodFormViewModel model)
        {

            /*
             * Business Rule | Initial Bird Count: a Brood must start with at least one bird. Services contain business logic, 
             * while Controllers only coordinate HTTP requests. Reference: https://learn.microsoft.com/aspnet/core/mvc/overview
            */
            if (model.BroodBirdInitialNum <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad inicial de aves debe ser mayor que cero.");
            }

            var existingBrood = await _context.Broods
                .FirstOrDefaultAsync(brood => brood.BroodId == model.BroodId);

            if (existingBrood == null)
            {
                throw new InvalidOperationException("Camada no encontrada.");
            }

            existingBrood.BroodName = NormalizeText(model.BroodName);
            existingBrood.BroodBirdInitialNum = model.BroodBirdInitialNum;
            existingBrood.BroodDescription = string.IsNullOrWhiteSpace(model.BroodDescription)
                ? null
                : NormalizeText(model.BroodDescription);
            existingBrood.BroilerHouseId = model.BroilerHouseId;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var brood = await _context.Broods
                .FirstOrDefaultAsync(brood => brood.BroodId == id);

            if (brood == null)
            {
                throw new InvalidOperationException("Camada no encontrada.");
            }

            brood.BroodState = false;

            await _context.SaveChangesAsync();
        }

        public async Task<List<BroodListViewModel>> GetAllInactiveAsync()
        {
            return await _context.Broods
                .AsNoTracking()
                .Include(brood => brood.BroilerHouse)
                .Where(brood => !brood.BroodState)
                .OrderBy(brood => brood.BroodDate)
                .Select(brood => new BroodListViewModel
                {
                    BroodId = brood.BroodId,
                    BroodName = brood.BroodName,
                    BroodDate = brood.BroodDate,
                    BroodBirdInitialNum = brood.BroodBirdInitialNum,
                    BroodDescription = brood.BroodDescription,
                    BroodState = brood.BroodState,
                    BroilerHouseName = brood.BroilerHouse != null
                        ? brood.BroilerHouse.BroilerHouseName
                        : string.Empty
                })
                .ToListAsync();
        }


        public async Task ActivateAsync(int id)
        {
            var brood = await _context.Broods
                .FirstOrDefaultAsync(brood => brood.BroodId == id);

            if (brood == null)
            {
                throw new InvalidOperationException("Camada no encontrada.");
            }

            brood.BroodState = true;

            await _context.SaveChangesAsync();
        }

    }
}