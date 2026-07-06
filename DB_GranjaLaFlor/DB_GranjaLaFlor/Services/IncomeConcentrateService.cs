using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates;
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

        public async Task<List<IncomeConcentrateListViewModel>> GetAllActiveAsync()
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Include(income => income.Brood)
                .Where(income => income.IncomeState)
                .OrderByDescending(income => income.IncomeConcentrateDate)
                /* Converts the entity model into a ViewModel by creating a new object that contains only the properties required by 
                 * the view. This helps separate the data layer from the presentation layer, reducing unnecessary data exposure and 
                 * improving maintainability. The resulting ViewModel is what the system uses to display the list of income concentrate 
                 * records. Reference: https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying?utm_source=chatgpt.com#project-only-properties-you-need
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
                        : string.Empty
                })
                .Take(10)
                .ToListAsync();
        }

        public async Task<IncomeConcentrateGetByIdViewModel?> GetByIdAsync(int id)
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Include(income => income.Brood)
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
                        : string.Empty
                })
                .FirstOrDefaultAsync();
        }

        /*
          * UI Data | Unique Active Brood Select List: Returns one option per Brood name: the Brood must be active and its related Broiler
          * House must also be active. Not duplicated Brood names are grouped in memory and only one option is displayed in the Create form.
          * Groups broods by name and calendar year to ensure that each brood name appears only once per year in the dropdown list.
          * Called in GET - Create
         */
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

        /*
          * Business Calculation | Current Accumulated Concentrate
          * Retrieves the current accumulated concentrate (in kilograms) for the selected brood. This value is used to estimate the new
          * accumulated amount before saving the record, allowing the user to preview the calculation on the Create form.
         */
        public async Task<decimal> GetCurrentAccumulatedByBroodAsync(int broodId)
        {
            return await _context.IncomeConcentrates
                .AsNoTracking()
                .Where(income =>
                    income.BroodId == broodId &&
                    income.IncomeState)
                //Sum all IncomeKilos from deletecd brood in form to display a pre-view ofcurrent accumulated concentrate. 
                .SumAsync(income => income.IncomeKilos);
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
            */
            var previousAccumulated = await _context.IncomeConcentrates
                .Where(income =>
                    income.BroodId == model.BroodId && //valdiates only the selected Broodid exsiting in DB. 
                    income.IncomeState) //validates ecah of the records are active.  
                .SumAsync(income => income.IncomeKilos);//If above is meet, then sum only Incomekilos property 


            //creates (rewrites) a new object (income) to update IncomeAccumulated by sum: "previousAccumulated + incomeKilos".
            var income = new IncomeConcentrate
            {
                IncomeConcentrateDate = model.IncomeConcentrateDate,
                IncomeQuintals = model.IncomeQuintals,
                IncomeKilos = incomeKilos,
                /*
                 * Business Rule | Running Accumulated:The accumulated concentrate is calculated by adding the
                 * current income to the accumulated active concentrate previously registered for the same brood.
                */
                IncomeAccumulated = previousAccumulated + incomeKilos,
                IncomeDescription = string.IsNullOrWhiteSpace(model.IncomeDescription)
                    ? null
                    : NormalizeText(model.IncomeDescription),
                IncomeState = true,
                BroodId = model.BroodId
            };

            _context.IncomeConcentrates.Add(income);

            await _context.SaveChangesAsync();
        }

        private static string NormalizeText(string value)
        {
            return Regex.Replace(
                value.Trim(),
                @"\s+",
                " ");
        }
    }
}