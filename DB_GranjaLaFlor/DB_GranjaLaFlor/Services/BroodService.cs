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
                //Orders Broods from newest to oldest
                .OrderByDescending(brood =>
                    brood.BroodDate)
                .ThenByDescending(brood =>
                    brood.BroodId)
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


        /*
         * Business Operation | Create Brood
         *
         * Creates a new Brood after validating its initial bird
         * population, Broiler House availability and operational
         * uniqueness.
         */
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


            /*
             * Business Validation | Broiler House
             *
             * Confirms that the selected Broiler House exists
             * and is currently active.
             */
            var broilerHouseExists =
                await _context.BroilerHouses
                    .AsNoTracking()
                    .AnyAsync(
                        broilerHouse =>
                            broilerHouse.BroilerHouseId ==
                                model.BroilerHouseId &&
                            broilerHouse.BroilerHouseState);

            if (!broilerHouseExists)
            {
                throw new InvalidOperationException(
                    "La pollera seleccionada no existe o está inactiva.");
            }


            /*
             * Business Validation | Duplicate Active Brood
             *
             * Prevents more than one active Brood from using the
             * same Brood number inside the same Broiler House.
             *
             * BroodName is obtained from the controlled dropdown
             * and therefore does not require text normalization.
             */
            var duplicateExists =
                await _context.Broods
                    .AsNoTracking()
                    .AnyAsync(
                        brood =>
                            brood.BroodName ==
                                model.BroodName &&
                            brood.BroilerHouseId ==
                                model.BroilerHouseId &&
                            brood.BroodState);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Ya existe una camada activa con el mismo número " +
                    "en la pollera seleccionada.");
            }


            /*
             * Entity Mapping | Brood
             *
             * Creates a new Brood entity using the information
             * validated by the Service layer.
             */
            var brood =
                new Brood
                {
                    BroodName =
                        model.BroodName,

                    BroodDate =
                        DateTime.Today,

                    BroodBirdInitialNum =
                        model.BroodBirdInitialNum,

                    BroodDescription =
                        string.IsNullOrWhiteSpace(
                            model.BroodDescription)
                            ? null
                            : NormalizeText(
                                model.BroodDescription),

                    BroodState =
                        true,

                    BroilerHouseId =
                        model.BroilerHouseId
                };


            /*
             * Database Operation | Create Brood
             */
            _context.Broods.Add(
                brood);

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


        /*
         * Business Operation | Update Brood
         *
         * Updates an active Brood only when it has not yet been
         * referenced by operational production information.
         *
         * Once operational records exist, the Brood becomes part
         * of the production history and cannot be modified.
         */
        public async Task UpdateAsync(
            BroodFormViewModel model)
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


            /*
             * Business Validation | Existing Brood
             *
             * Confirms that the selected Brood exists.
             */
            var existingBrood =
                await _context.Broods
                    .FirstOrDefaultAsync(
                        brood =>
                            brood.BroodId ==
                                model.BroodId);

            if (existingBrood == null)
            {
                throw new InvalidOperationException(
                    "Camada no encontrada.");
            }


            /*
             * Business Validation | Brood State
             *
             * Only active Broods can be edited.
             */
            if (!existingBrood.BroodState)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada se encuentra inactiva.");
            }


            /*
             * Business Validation | Income Concentrate Dependency
             *
             * Any Income Concentrate reference makes the Brood part
             * of the operational production history.
             *
             * State is intentionally not evaluated because inactive
             * records still preserve their historical relationship.
             */
            var hasIncomeConcentrates =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .AnyAsync(
                        income =>
                            income.BroodId ==
                                existingBrood.BroodId);


            /*
             * Business Validation | Daily Check Dependency
             *
             * Any Daily Check reference prevents the Brood from
             * being modified, including inactive historical records.
             */
            var hasDailyChecks =
                await _context.DailyChecks
                    .AsNoTracking()
                    .AnyAsync(
                        dailyCheck =>
                            dailyCheck.BroodId ==
                                existingBrood.BroodId);


            /*
             * Business Validation | Weekly Check Dependency
             *
             * Existing Weekly Check records preserve calculations
             * generated from the Brood operational information.
             */
            var hasWeeklyChecks =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .AnyAsync(
                        weeklyCheck =>
                            weeklyCheck.BroodId ==
                                existingBrood.BroodId);


            /*
             * Business Rule | Operational Brood Protection
             *
             * Once any operational information references the Brood,
             * the complete Brood record becomes read-only.
             *
             * This protects:
             *
             * - Brood number
             * - Broiler House
             * - Initial bird quantity
             * - Historical operational calculations
             */
            if (hasIncomeConcentrates ||
                hasDailyChecks ||
                hasWeeklyChecks)
            {
                throw new InvalidOperationException(
                    "La camada no puede ser modificada porque contiene " +
                    "información operativa asociada.");
            }


            /*
             * Business Validation | Broiler House
             *
             * Confirms that the selected Broiler House exists
             * and is currently active.
             */
            var broilerHouseExists =
                await _context.BroilerHouses
                    .AsNoTracking()
                    .AnyAsync(
                        broilerHouse =>
                            broilerHouse.BroilerHouseId ==
                                model.BroilerHouseId &&
                            broilerHouse.BroilerHouseState);

            if (!broilerHouseExists)
            {
                throw new InvalidOperationException(
                    "La pollera seleccionada no existe o está inactiva.");
            }


            /*
             * Business Validation | Duplicate Active Brood
             *
             * Prevents the update from creating another active
             * Brood with the same number inside the same Broiler House.
             *
             * The current Brood is excluded from the validation.
             */
            var duplicateExists =
                await _context.Broods
                    .AsNoTracking()
                    .AnyAsync(
                        brood =>
                            brood.BroodId !=
                                model.BroodId &&
                            brood.BroodName ==
                                model.BroodName &&
                            brood.BroilerHouseId ==
                                model.BroilerHouseId &&
                            brood.BroodState);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Ya existe una camada activa con el mismo número " +
                    "en la pollera seleccionada.");
            }


            /*
             * Entity Update | Brood
             *
             * Updates the Brood only when no operational
             * dependencies have been found.
             */
            existingBrood.BroodName =
                model.BroodName;

            existingBrood.BroodBirdInitialNum =
                model.BroodBirdInitialNum;

            existingBrood.BroodDescription =
                string.IsNullOrWhiteSpace(
                    model.BroodDescription)
                    ? null
                    : NormalizeText(
                        model.BroodDescription);

            existingBrood.BroilerHouseId =
                model.BroilerHouseId;


            /*
             * Database Operation | Save Changes
             */
            await _context.SaveChangesAsync();
        }


        /*
 * Business Operation | Soft Delete Brood
 *
 * Logically deactivates an active Brood only when
 * no active operational records depend on it.
 */
        public async Task SoftDeleteAsync(
            int id)
        {
            /*
             * Business Validation | Existing Brood
             */
            var brood =
                await _context.Broods
                    .FirstOrDefaultAsync(
                        brood =>
                            brood.BroodId == id);

            if (brood == null)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no existe.");
            }


            /*
             * Business Validation | Brood State
             */
            if (!brood.BroodState)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada ya se encuentra inactiva.");
            }


            /*
             * Business Validation | Income Concentrate Dependency
             */
            var hasActiveIncomeConcentrates =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .AnyAsync(
                        income =>
                            income.BroodId ==
                                brood.BroodId &&
                            income.IncomeState);

            if (hasActiveIncomeConcentrates)
            {
                throw new InvalidOperationException(
                    "La camada no puede ser desactivada porque " +
                    "contiene ingresos de concentrado activos.");
            }


            /*
             * Business Validation | Daily Check Dependency
             */
            var hasActiveDailyChecks =
                await _context.DailyChecks
                    .AsNoTracking()
                    .AnyAsync(
                        dailyCheck =>
                            dailyCheck.BroodId ==
                                brood.BroodId &&
                            dailyCheck.DailyCheckState);

            if (hasActiveDailyChecks)
            {
                throw new InvalidOperationException(
                    "La camada no puede ser desactivada porque " +
                    "contiene controles diarios activos.");
            }


            /*
             * Business Validation | Weekly Check Dependency
             */
            var hasActiveWeeklyChecks =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .AnyAsync(
                        weeklyCheck =>
                            weeklyCheck.BroodId ==
                                brood.BroodId &&
                            weeklyCheck.WeeklyCheckState);

            if (hasActiveWeeklyChecks)
            {
                throw new InvalidOperationException(
                    "La camada no puede ser desactivada porque " +
                    "contiene controles semanales activos.");
            }


            /*
             * Logical Deletion | Brood State
             */
            brood.BroodState =
                false;

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