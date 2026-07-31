using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectGranjaLaFlor.ViewModels.IncomeConcentrates;
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
    public class IncomeConcentrateService
    {
        private readonly ApplicationDbContext _context;

        /*
         * Business Constant | Quintal Conversion
         * One quintal is equivalent to 46 kilograms.Used const to set 45 value as it never changes. 
        */
        private const decimal KilosPerQuintal = 46m;

        public IncomeConcentrateService(ApplicationDbContext context)
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

        /*
             * Business Calculation | Recalculate Brood Accumulated
             * Recalculates the accumulated concentrate (kilograms) for all active
             * income records belonging to the specified Brood.
             *
             * Records are processed in chronological order to preserve the business
             * rule that each accumulated value represents the running total at the
             * time the income was registered.
         */
        private async Task RecalculateAccumulatedAsync(int broodId)
        {
            // Object list: lists valid incomeconcentrate in incomes var. 
            var incomes = await _context.IncomeConcentrates
                .Where(income =>
                    income.BroodId == broodId &&
                    income.IncomeState)
                .OrderBy(income => income.IncomeConcentrateDate)
                .ThenBy(income => income.IncomeConcentrateId)
                .ToListAsync();

            decimal accumulated = 0; // Initiates var at 0, used then in the loop to calculate real accumulated. 

            foreach (var income in incomes)
            {
                //Sums incomekilos to the running accumulated total using accumulated = accumulated + income.IncomeKilos.
                accumulated += income.IncomeKilos;
                income.IncomeAccumulated = accumulated;
            }

            await _context.SaveChangesAsync();
        }

        /*
        public async Task<List<IncomeConcentrateListViewModel>> GetAllActiveAsync()
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Include(income => income.Brood)
                    .ThenInclude(brood => brood.BroilerHouse)
                .Where(income => income.IncomeState)
                .OrderByDescending(income => income.IncomeConcentrateDate)
                .ThenBy(income => income.Brood != null
                    ? income.Brood.BroodName
                    : string.Empty)
                .ThenByDescending(income => income.IncomeConcentrateId)

                /* Converts the entity model into a ViewModel by creating a new object that contains only the properties required by 
                 * the view. This helps separate the data layer from the presentation layer, reducing unnecessary data exposure and 
                 * improving maintainability. The resulting ViewModel is what the system uses to display the list of income concentrate 
                 * records. Reference: https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying?utm_source=chatgpt.com#project-only-properties-you-need
                
                .Select(income => new IncomeConcentrateListViewModel
                {
                    IncomeConcentrateId = income.IncomeConcentrateId,
                    IncomeConcentrateDate = income.IncomeConcentrateDate,
                    IncomeQuintals = income.IncomeQuintals,
                    IncomeKilos = income.IncomeKilos,
                    IncomeAccumulated = income.IncomeAccumulated,
                    IncomeDescription = income.IncomeDescription,
                    IncomeState = income.IncomeState,
                    BroodId = income.BroodId,
                    BroodName = income.Brood != null
                        ? income.Brood.BroodName
                        // if not found, return an empty string. ? equal to allow null 
                        : string.Empty,
                    BroodYear = income.Brood != null
                        ? income.Brood.BroodDate.Year
                        // if not found, return 0 as value
                        : 0,
                    BroilerHouseName = income.Brood != null &&
                        income.Brood.BroilerHouse != null
                        ? income.Brood.BroilerHouse.BroilerHouseName
                        : string.Empty
                })
                .Take(10)
                .ToListAsync();
        }
        */

        public async Task<List<IncomeConcentrateListViewModel>> GetAllActiveAsync(string? broodName = null, int? year = null,  int? broilerHouseId = null)
        {

            /*
             * Creates the initial query used to retrieve active income concentrate records. The query is not executed at this point. It is stored in a
             * variable so optional filters can be added before calling ToListAsync(). In summary, if there is not filters applied, show active data. 
             */
            var query = _context.IncomeConcentrates
                .AsNoTracking()
                .Include(income => income.Brood)
                    .ThenInclude(brood => brood.BroilerHouse)
                .Where(income => income.IncomeState)
                .AsQueryable();

            /*
                 * Applies the Brood name filter only when the user selects
                 * a Brood name. Different Brood records may share the same
                 * name but belong to different years or Broiler Houses.
             */
            if (!string.IsNullOrWhiteSpace(broodName))
            {
                query = query.Where(income =>
                    income.Brood != null &&
                    income.Brood.BroodName == broodName);
            }


            /*
             * Applies the year filter using the year stored in BroodDate.
             * The year is obtained through the relationship between
             * IncomeConcentrate and Brood.
             */
            if (year.HasValue)
            {
                query = query.Where(income =>
                    income.Brood != null &&
                    income.Brood.BroodDate.Year == year.Value);
            }

            /*
             * Applies the broiler house filter through the Brood relationship.
             * If broilerHouseId is null, records from all broiler houses
             * remain included in the query.
             */
            if (broilerHouseId.HasValue)
            {
                query = query.Where(income =>
                    income.Brood != null &&
                    income.Brood.BroilerHouseId == broilerHouseId.Value);
            }

            /*
             * Executes the query after applying the optional filters.
             * If the user does not select any filter, the method returns the
             * same active records that were displayed before adding the filters.
             */
            return await query
                .OrderByDescending(income => income.IncomeConcentrateDate)
                .ThenBy(income => income.Brood != null
                    ? income.Brood.BroodName
                    : string.Empty)
                .ThenByDescending(income => income.IncomeConcentrateId)

                /*
                 * Converts the entity model into a ViewModel by creating a new object that contains only the properties required by 
                 * the view. This helps separate the data layer from the presentation layer, reducing
                 * unnecessary data exposure and improving maintainability.
                 *
                 * The resulting ViewModel is what the system uses to display the list of income concentrate records. Reference:
                 * https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying#project-only-properties-you-need
                 */
                .Select(income => new IncomeConcentrateListViewModel
                {
                    IncomeConcentrateId = income.IncomeConcentrateId,
                    IncomeConcentrateDate = income.IncomeConcentrateDate,
                    IncomeQuintals = income.IncomeQuintals,
                    IncomeKilos = income.IncomeKilos,
                    IncomeAccumulated = income.IncomeAccumulated,
                    IncomeDescription = income.IncomeDescription,
                    IncomeState = income.IncomeState,
                    BroodId = income.BroodId,

                    BroodName = income.Brood != null
                        ? income.Brood.BroodName
                        // If the brood is not found, returns an empty string.
                        : string.Empty,

                    BroodYear = income.Brood != null
                        ? income.Brood.BroodDate.Year
                        // If the brood is not found, returns 0 as the value.
                        : 0,

                    BroilerHouseName = income.Brood != null &&
                        income.Brood.BroilerHouse != null
                        ? income.Brood.BroilerHouse.BroilerHouseName
                        // If the broiler house is not found, returns an empty string.
                        : string.Empty
                })
                .Take(10)
                .ToListAsync();
        }


        /*
         * UI Data | Income Concentrate Index Filter: Creates the complete ViewModel required by the Index view.
         * The method retrieves the active Income Concentrate records by calling GetAllActiveAsync(). It also loads the Brood, year and
         * Broiler House options used by the filter dropdown menus.
         */
        public async Task<IncomeConcentrateFilterViewModel>GetFilterViewModelAsync(string? broodName = null, int? year = null,int? broilerHouseId = null)
        {
            /*
             * Retrieves the active Income Concentrate records. If all filter values are null, GetAllActiveAsync returns
             * the same active list displayed before adding the filters.
             */
            var incomeConcentrates = await GetAllActiveAsync(broodName, year,broilerHouseId);

            /*
              * Retrieves unique Brood names so the dropdown
              * does not repeat the same name.
            */
            var availableBroodNames = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState)
                .Select(brood =>
                    brood.BroodName)
                .Distinct()
                .OrderBy(name =>
                    name)
                .ToListAsync();

            /*
             * UI Data | Brood Filter Options: Retrieves every active Brood associated with an active Broiler House.
             * The Brood name, Broiler House and year are displayed to clearly identify each option.
             */
            var broodOptions = availableBroodNames
                .Select(name =>
                    new SelectListItem
                    {
                        Value = name,
                        Text = name,

                        /*
                         * Preserves the selected Brood name
                         * after submitting the filter form.
                         */
                        Selected =
                            !string.IsNullOrWhiteSpace(broodName) &&
                            name == broodName
                    })
                .ToList();

            /*
             * UI Data | Year Filter Options
             * Retrieves the different years from active Broods
             * associated with active Broiler Houses.
             *
             * Distinct prevents duplicated years in the dropdown menu.
             */
            var availableYears = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState)
                .Select(brood =>
                    brood.BroodDate.Year)
                .Distinct()
                .OrderByDescending(broodYear =>
                    broodYear)
                .ToListAsync();

            /*
             * Converts each available year into a SelectListItem
             * used by the year dropdown menu.
             */
            var yearOptions = availableYears
                .Select(broodYear => new SelectListItem
                {
                    Value = broodYear.ToString(),
                    Text = broodYear.ToString(),

                    /*
                     * Preserves the selected year after submitting
                     * the filter form.
                     */
                    Selected = year.HasValue &&
                               broodYear == year.Value
                })
                .ToList();

            /*
             * UI Data | Broiler House Filter Options
             * Retrieves every active Broiler House used by the
             * Broiler House filter dropdown menu.
             */
            var broilerHouseOptions = await _context.BroilerHouses
                .AsNoTracking()
                .Where(broilerHouse =>
                    broilerHouse.BroilerHouseState)
                .OrderBy(broilerHouse =>
                    broilerHouse.BroilerHouseName)
                .Select(broilerHouse => new SelectListItem
                {
                    Value = broilerHouse.BroilerHouseId.ToString(),
                    Text = broilerHouse.BroilerHouseName,

                    /*
                     * Preserves the selected Broiler House after
                     * submitting the filter form.
                     */
                    Selected = broilerHouseId.HasValue &&
                               broilerHouse.BroilerHouseId ==
                               broilerHouseId.Value
                })
                .ToListAsync();

            /*
             * Creates the ViewModel required by the Index view.
             * The current Income Concentrate list remains unchanged.
             * The filter values and dropdown options are added to
             * the same page.
             */
            return new IncomeConcentrateFilterViewModel
            {
                BroodName = broodName,
                Year = year,
                BroilerHouseId = broilerHouseId,

                BroodOptions = broodOptions,
                YearOptions = yearOptions,
                BroilerHouseOptions = broilerHouseOptions,

                IncomeConcentrates = incomeConcentrates
            };
        }



        //Details - Delete - Activate. Use GetById ViewModel. 
        public async Task<IncomeConcentrateGetByIdViewModel?> GetByIdAsync(int id)
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Include(income => income.Brood)
                    .ThenInclude(brood => brood.BroilerHouse)
                .Where(income => income.IncomeConcentrateId == id)
                .Select(income => new IncomeConcentrateGetByIdViewModel
                {
                    IncomeConcentrateId = income.IncomeConcentrateId,
                    IncomeConcentrateDate = income.IncomeConcentrateDate,
                    IncomeQuintals = income.IncomeQuintals,
                    IncomeKilos = income.IncomeKilos,
                    IncomeAccumulated = income.IncomeAccumulated,
                    IncomeDescription = income.IncomeDescription,
                    IncomeState = income.IncomeState,
                    BroodId = income.BroodId,
                    BroodName = income.Brood != null
                        ? income.Brood.BroodName
                        : string.Empty,
                    BroilerHouseName =
                    income.Brood != null &&
                    income.Brood.BroilerHouse != null
                        ? income.Brood.BroilerHouse.BroilerHouseName
                        : string.Empty

                })
                .FirstOrDefaultAsync();
        }


        /*
          * UI Data | Unique Active Brood Select List: Returns one option per Brood name: the Brood must be active and its related Broiler
          * House must also be active. Not duplicated Brood names are grouped in memory and only one option is displayed in the Create form.
          * Groups broods by name and calendar year to ensure that each brood name appears only once per year in the dropdown list.
          * Called in GET - Create
         
        public async Task<List<SelectListItem>> GetBroodSelectListAsync()
        {
            var validBroods = await _context.Broods
                .AsNoTracking()
                .Include(brood => brood.BroilerHouse)
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState)
                .OrderBy(brood => brood.BroodName)
                .ThenBy(brood => brood.BroodId)
                .ToListAsync();

            return validBroods
                // Groups broods by their name and registration year.
                .GroupBy(brood => new
                {
                    brood.BroodName,
                    Year = brood.BroodDate.Year
                })

                // Keeps only the first brood from each group.
                .Select(group => group.First())

                // Converts each brood into a SelectListItem
                // used by the ASP.NET Core dropdown.
                .Select(brood => new SelectListItem
                {
                    Value = brood.BroodId.ToString(),
                    Text = $"{brood.BroodName} - Año {brood.BroodDate.Year}"
                })

                // Executes the LINQ query and returns the result as a List.
                .ToList();

        }
        */

        /*
          * UI Data | Active Brood and Broiler House Select List: Returns every active Brood associated with an active Broiler House.
          * Concentrate income is managed independently for each Brood and Broiler House because every Broiler House has its own concentrate
          * silo. Therefore, Broods with the same name and year must not be grouped.
         */
        public async Task<List<SelectListItem>> GetBroodSelectListAsync()
        {
            return await _context.Broods
                .AsNoTracking()
                .Include(brood => brood.BroilerHouse)
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState)
                .OrderByDescending(brood => brood.BroodDate.Year)
                .ThenBy(brood => brood.BroodName)
                .ThenBy(brood => brood.BroilerHouse!.BroilerHouseName)
                // Converts each brood into a SelectListItem
                // used by the ASP.NET Core dropdown.
                .Select(brood => new SelectListItem
                {
                    Value = brood.BroodId.ToString(),

                    Text = $"{brood.BroodName} - " +
                           $"{brood.BroilerHouse!.BroilerHouseName} - " +
                           $"Año {brood.BroodDate.Year}"
                })
                // Executes the LINQ query and returns the result as a List.
                .ToListAsync();
        }



        /*
          * Business Calculation | Current Accumulated Concentrate: Retrieves the current accumulated concentrate for the selected Brood.
          * When editing an existing Income Concentrate record, the current record can be excluded to avoid adding it twice in the accumulated preview.
          * This method supports both Create and Edit views following the DRY principle.
         */
        public async Task<decimal> GetCurrentAccumulatedByBroodAsync(
            int broodId,
            int? excludeIncomeConcentrateId = null)
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Where(income =>
                    income.BroodId == broodId &&
                    income.IncomeState &&
                    (!excludeIncomeConcentrateId.HasValue ||
                    //Exclude value being edited. 
                     income.IncomeConcentrateId != excludeIncomeConcentrateId.Value))
                //Sum all IncomeKilos from Seleted brood in the form to display a pre-view ofcurrent accumulated concentrate. 
                .SumAsync(income => income.IncomeKilos);
        }

        /*
         * Business Query | Current Brood Accumulated: Retrieves the latest accumulated concentrate (kilograms) for the selected Brood.
         * Unlike the preview calculation used in Create/Edit, this method returns the official accumulated value stored in the most recent
         * active Income Concentrate record. This value is intended to be consumed by other modules such as Daily Checks and Weekly Checks.
         */
        public async Task<decimal> GetCurrentAccumulatedAsync(int broodId)
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Where(income =>
                    income.BroodId == broodId &&
                    income.IncomeState)
                .OrderByDescending(income => income.IncomeConcentrateDate)
                .ThenByDescending(income => income.IncomeConcentrateId)
                .Select(income => income.IncomeAccumulated)
                .FirstOrDefaultAsync();
        }


        public async Task CreateAsync(IncomeConcentrateFormViewModel model)
        {
            /*
             * Business Rule | Income Quintals
             * Concentrate income must be greater than zero.
             */
            if (model.IncomeQuintals <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad de quintales debe ser mayor que cero.");
            }

            /*
              * Business Rule | Active Brood Validacotion
              * The selected brood must exist, remain active,
              * and belong to an active Broiler House.
            */
            var broodExists = await _context.Broods
                .Include(brood => brood.BroilerHouse)
                .AnyAsync(brood =>
                    brood.BroodId == model.BroodId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState);

            if (!broodExists)
            {
                throw new InvalidOperationException(
                    "Camada no disponible.");
            }
            /*
             * Business Rule | Automatic Kilogram Calculation
             * The system automatically converts the entered quintals
             * into kilograms using the standard conversion factor.
            */
            var incomeKilos = model.IncomeQuintals * KilosPerQuintal;

            /*
             * Business Rule | Accumulated Concentrate
             * Retrieves the accumulated kilograms from all active
             * concentrate income records belonging to the same selected brood.
             * This value is used to calculate the new accumulated amount.
            
            var previousAccumulated = await _context.IncomeConcentrates
                .Where(income =>
                    income.BroodId == model.BroodId && //valdiates only the selected Broodid exsiting in DB. 
                    income.IncomeState) //validates ecah of the records are active.  
                .SumAsync(income => income.IncomeKilos);//If above is meet, then sum only Incomekilos property 
            */

            //creates (rewrites) a new object (income) to update IncomeAccumulated by sum: "previousAccumulated + incomeKilos".
            var income = new IncomeConcentrate
            {
                IncomeConcentrateDate = model.IncomeConcentrateDate,
                IncomeQuintals = model.IncomeQuintals,
                IncomeKilos = incomeKilos,
                /*
                 * Business Rule | Running Accumulated:The accumulated concentrate is calculated by adding the
                 * current income to the accumulated active concentrate previously registered for the same brood.
                
                IncomeAccumulated = previousAccumulated + incomeKilos,
                */
                IncomeAccumulated = 0,
                IncomeDescription = string.IsNullOrWhiteSpace(model.IncomeDescription)
                    ? null
                    : NormalizeText(model.IncomeDescription),
                IncomeState = true,
                BroodId = model.BroodId
            };

            _context.IncomeConcentrates.Add(income);

            await _context.SaveChangesAsync();

            /*
             * Business Rule | Running Accumulated
             * Recalculates the accumulated concentrate for every active
             * Income Concentrate belonging to the selected Brood.
             */
            await RecalculateAccumulatedAsync(model.BroodId);

        }

        //Edit - GET
        public async Task<IncomeConcentrateFormViewModel?> GetFormByIdAsync(int id)
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Where(income => income.IncomeConcentrateId == id)
                .Select(income => new IncomeConcentrateFormViewModel
                {
                    IncomeConcentrateId = income.IncomeConcentrateId,
                    IncomeConcentrateDate = income.IncomeConcentrateDate,
                    IncomeQuintals = income.IncomeQuintals,
                    IncomeKilos = income.IncomeKilos,
                    IncomeAccumulated = income.IncomeAccumulated,
                    IncomeDescription = income.IncomeDescription,
                    BroodId = income.BroodId
                })
                .FirstOrDefaultAsync();
        }

        /*
        public async Task UpdateAsync(IncomeConcentrateFormViewModel model)
        {
            /*
             * Business Rule | Income Quintals
             * Concentrate income must be greater than zero.
             
            if (model.IncomeQuintals <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad de quintales debe ser mayor que cero.");
            }

            var income = await _context.IncomeConcentrates
                .FirstOrDefaultAsync(income =>
                    income.IncomeConcentrateId == model.IncomeConcentrateId &&
                    income.IncomeState);

            if (income == null)
            {
                throw new InvalidOperationException(
                    "Ingreso de concentrado no encontrado.");
            }

            var broodExists = await _context.Broods
                .Include(brood => brood.BroilerHouse)
                .AnyAsync(brood =>
                    brood.BroodId == model.BroodId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState);

            if (!broodExists)
            {
                throw new InvalidOperationException(
                    "Camada no disponible.");
            }

            var incomeKilos = model.IncomeQuintals * KilosPerQuintal;

            /*
            var previousAccumulated = await _context.IncomeConcentrates
                .Where(existingIncome =>
                    existingIncome.BroodId == model.BroodId &&
                    existingIncome.IncomeState &&
                    // Need to exclude the record being edited as it already exists in DB.
                    existingIncome.IncomeConcentrateId != model.IncomeConcentrateId)
                .SumAsync(existingIncome => existingIncome.IncomeKilos);
            
            income.IncomeConcentrateDate = model.IncomeConcentrateDate;
            income.IncomeQuintals = model.IncomeQuintals;
            income.IncomeKilos = incomeKilos;
            /*
            income.IncomeAccumulated = previousAccumulated + incomeKilos;
            
            income.IncomeDescription = string.IsNullOrWhiteSpace(model.IncomeDescription)
                ? null
                : NormalizeText(model.IncomeDescription);
            income.BroodId = model.BroodId;

            await _context.SaveChangesAsync();

            /*
             * Business Rule | Running Accumulated
             * Recalculates the accumulated concentrate for every active
             * Income Concentrate belonging to the selected Brood.
             
            await RecalculateAccumulatedAsync(model.BroodId);
        }
        */

        public async Task UpdateAsync(IncomeConcentrateFormViewModel model)
        {
            /*
             * Business Rule | Income Quintals
             * Concentrate income must be greater than zero.
             */
            if (model.IncomeQuintals <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad de quintales debe ser mayor que cero.");
            }

            var income = await _context.IncomeConcentrates
                .FirstOrDefaultAsync(income =>
                    income.IncomeConcentrateId == model.IncomeConcentrateId &&
                    income.IncomeState);

            if (income == null)
            {
                throw new InvalidOperationException(
                    "Ingreso de concentrado no encontrado.");
            }

            /*
             * Business Rule | Previous Brood Tracking
             * Stores the original Brood before updating the record.
             * If the user changes the Brood in Edit, both the previous
             * and the new Brood accumulated values must be recalculated.
             */
            var previousBroodId = income.BroodId;

            var broodExists = await _context.Broods
                .Include(brood => brood.BroilerHouse)
                .AnyAsync(brood =>
                    brood.BroodId == model.BroodId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState);

            if (!broodExists)
            {
                throw new InvalidOperationException(
                    "Camada no disponible.");
            }

            var incomeKilos = model.IncomeQuintals * KilosPerQuintal;

            income.IncomeConcentrateDate = model.IncomeConcentrateDate;
            income.IncomeQuintals = model.IncomeQuintals;
            income.IncomeKilos = incomeKilos;
            income.IncomeAccumulated = 0;
            income.IncomeDescription = string.IsNullOrWhiteSpace(model.IncomeDescription)
                ? null
                : NormalizeText(model.IncomeDescription);
            income.BroodId = model.BroodId;

            await _context.SaveChangesAsync();

            /*
             * Business Rule | Recalculate New Brood Accumulated
             * Recalculates the accumulated concentrate for the Brood currently
             * assigned to the edited record.
             */
            await RecalculateAccumulatedAsync(model.BroodId);

            /*
             * Business Rule | Recalculate Previous Brood Accumulated
             * If the edited record was moved to a different Brood, the previous
             * Brood also needs to be recalculated because it no longer includes
             * this income concentrate record.
             */
            if (previousBroodId != model.BroodId)
            {
                await RecalculateAccumulatedAsync(previousBroodId);
            }
        }

        public async Task SoftDeleteAsync(int id)
        {
            var income = await _context.IncomeConcentrates
                .FirstOrDefaultAsync(income =>
                    income.IncomeConcentrateId == id &&
                    income.IncomeState);

            if (income == null)
            {
                throw new InvalidOperationException(
                    "Ingreso de concentrado no encontrado o ya se encuentra inactivo.");
            }

            var broodId = income.BroodId;

            income.IncomeState = false;

            await _context.SaveChangesAsync();

            /*
             * Business Rule | Recalculate Accumulated After Delete: When an Income Concentrate is deactivated, it must no longer
             * be considered in accumulated calculations. The selected Brood is recalculated to keep all active records consistent.
             */
            await RecalculateAccumulatedAsync(broodId);
        }


        public async Task<List<IncomeConcentrateListViewModel>> GetAllInactiveAsync()
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Include(income => income.Brood)
                    .ThenInclude(brood => brood.BroilerHouse)
                .Where(income => !income.IncomeState)
                .OrderByDescending(income => income.IncomeConcentrateDate)
                .Select(income => new IncomeConcentrateListViewModel
                {
                    IncomeConcentrateId = income.IncomeConcentrateId,
                    IncomeConcentrateDate = income.IncomeConcentrateDate,
                    IncomeQuintals = income.IncomeQuintals,
                    IncomeKilos = income.IncomeKilos,
                    IncomeAccumulated = income.IncomeAccumulated,
                    IncomeDescription = income.IncomeDescription,
                    IncomeState = income.IncomeState,
                    BroodId = income.BroodId,
                    BroodName = income.Brood != null
                        ? income.Brood.BroodName
                        : string.Empty,
                    BroodYear = income.Brood != null
                        ? income.Brood.BroodDate.Year
                        : 0,
                    BroilerHouseName =
                    income.Brood != null &&
                    income.Brood.BroilerHouse != null
                        ? income.Brood.BroilerHouse.BroilerHouseName
                        : string.Empty
                })
                .ToListAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var income = await _context.IncomeConcentrates
                .FirstOrDefaultAsync(income =>
                    income.IncomeConcentrateId == id);

            if (income == null)
            {
                throw new InvalidOperationException(
                    "Ingreso de concentrado no encontrado.");
            }

            if (income.IncomeState)
            {
                throw new InvalidOperationException(
                    "El ingreso de concentrado ya se encuentra activo.");
            }

            income.IncomeState = true;

            await _context.SaveChangesAsync();

            /*
             * Business Rule | Recalculate Accumulated After Reactivation
             * When an Income Concentrate is reactivated, it must be included
             * again in the accumulated calculation of its Brood.
             */
            await RecalculateAccumulatedAsync(income.BroodId);
        }


        /*
          * Business Rule | Recalculate Running Accumulated: Recalculates the accumulated concentrate for all active Income
         * Concentrate records belonging to the specified Brood. Records are processed chronologically to guarantee that every
         * accumulated value reflects the sum of all previous active concentrate incomes.
         
        private async Task RecalculateAccumulatedAsync(int broodId)
        {
            // Object list: lists valid incomeconcentrate in incomes var. 
            var incomes = await _context.IncomeConcentrates
                .Where(income =>
                    income.BroodId == broodId &&
                    income.IncomeState)
                .OrderBy(income => income.IncomeConcentrateDate)
                .ThenBy(income => income.IncomeConcentrateId)
                .ToListAsync();

            decimal accumulated = 0; // Initiates var at 0, used then in the loop to calculate real accumulated. 

            // For each incomekilos record in object list "incomes"
            foreach (var income in incomes)
            {
                //Sums incomekilos to the running accumulated total using accumulated = accumulated + income.IncomeKilos.
                accumulated += income.IncomeKilos;

                income.IncomeAccumulated = accumulated;
            }

            await _context.SaveChangesAsync();
        }
        */

    }
}