using DB_GranjaLaFlor.Data.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectGranjaLaFlor.Models;
using ProjectGranjaLaFlor.Models.ViewModels;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Architecture Decision | Service Layer
     * Business logic and database access are implemented inside Services.
     * Controllers should coordinate HTTP requests and delegate data operations
     * to the Service layer.
     */
    public class DailyCheckService
    {
        private readonly ApplicationDbContext _context;

        public DailyCheckService(ApplicationDbContext context)
        {
            _context = context;
        }

        private const decimal KilosPerQuintal = 46m;

        private static readonly string[] ValidDailyCheckWeeks =
        {
            "Semana 1",
            "Semana 2",
            "Semana 3",
            "Semana 4",
            "Semana 5",
            "Semana 6",
            "Semana 7"
        };

        private static readonly string[] ValidDailyCheckDays =
        {
            "Día 1",
            "Día 2",
            "Día 3",
            "Día 4",
            "Día 5",
            "Día 6",
            "Día 7"
        };

        /*
 * Data Query | Active Daily Checks
 * Retrieves active Daily Check records and applies the optional filters
 * selected by the user in the Index view.
 *
 * The query is projected into DailyCheckListViewModel so the view
 * receives only the information required to display the records.
 */
        public async Task<List<DailyCheckListViewModel>> GetAllActiveAsync(
            int? broodId = null,
            int? year = null,
            int? broilerHouseId = null,
            string? dailyCheckWeek = null,
            string? dailyCheckDay = null)
        {
            var query = _context.DailyChecks
                .AsNoTracking()
                .Where(dailyCheck =>
                    dailyCheck.DailyCheckState)
                .AsQueryable();

            if (broodId.HasValue)
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.BroodId ==
                    broodId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.Brood.BroodDate.Year ==
                    year.Value);
            }

            if (broilerHouseId.HasValue)
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.Brood.BroilerHouseId ==
                    broilerHouseId.Value);
            }

            /*
             * Applies the Daily Check week filter only when the user
             * selects a week from the Index filter dropdown menu.
             */
            if (!string.IsNullOrWhiteSpace(dailyCheckWeek))
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.DailyCheckWeek ==
                    dailyCheckWeek);
            }

            if (!string.IsNullOrWhiteSpace(dailyCheckDay))
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.DailyCheckDay ==
                    dailyCheckDay);
            }

            return await query
                .OrderByDescending(dailyCheck =>
                    dailyCheck.DailyCheckDate)
                .ThenByDescending(dailyCheck =>
                    dailyCheck.DailyCheckId)
                .Select(dailyCheck => new DailyCheckListViewModel
                {
                    /*
                     * Internal identifier required by the action buttons.
                     */
                    DailyCheckId =
                        dailyCheck.DailyCheckId,

                    /*
                     * Daily Check information.
                     */
                    DailyCheckDate =
                        dailyCheck.DailyCheckDate,

                    DailyCheckWeek =
                        dailyCheck.DailyCheckWeek,

                    DailyCheckDay =
                        dailyCheck.DailyCheckDay,

                    /*
                     * Broiler House and Brood information.
                     */
                    BroilerHouseName =
                        dailyCheck.Brood
                            .BroilerHouse
                            .BroilerHouseName,

                    BroodId =
                        dailyCheck.BroodId,

                    BroodName =
                        dailyCheck.Brood.BroodName,

                    BroodYear =
                        dailyCheck.Brood.BroodDate.Year,

                    BroodBirdInitialNum =
                        dailyCheck.Brood
                            .BroodBirdInitialNum,

                    /*
                     * Income Concentrate information.
                     *
                     * IncomeAccumulated is obtained from the
                     * Income Concentrate record associated with
                     * the Daily Check. It is not stored directly
                     * in the daily_checks table.
                     */
                    IncomeConcentrateId =
                        dailyCheck.IncomeConcentrateId,

                    IncomeAccumulated =
                        dailyCheck.IncomeConcentrate
                            .IncomeAccumulated,

                    /*
                     * User-entered values.
                     */
                    NaturalMortality =
                        dailyCheck.NaturalMortality,

                    SelectQuantity =
                        dailyCheck.SelectQuantity,

                    ConsumptionQuintals =
                        dailyCheck.ConsumptionQuintals,

                    /*
                     * Calculated values.
                     */
                    ConsumptionKilos =
                        dailyCheck.ConsumptionKilos,

                    TotalDailyMortality =
                        dailyCheck.TotalDailyMortality,

                    AccumulatedMortality =
                        dailyCheck.AccumulatedMortality,

                    DailyBirdBalance =
                        dailyCheck.DailyBirdBalance,

                    AccumulatedConsumption =
                        dailyCheck.AccumulatedConsumption,

                    ConcentrateBalance =
                        dailyCheck.ConcentrateBalance,

                    DailyCheckState =
                        dailyCheck.DailyCheckState
                })
                .Take(10)
                .ToListAsync();
        }

        /*
 * UI Data | Daily Check Index Filter
 * Creates the complete ViewModel required by the Index view.
 *
 * The method retrieves the active Daily Check records and generates
 * the Brood, year, Broiler House, Daily Check week and Daily Check day
 * options used by the filter dropdown menus.
 */
        public async Task<DailyCheckFilterViewModel> GetFilterViewModelAsync(
            int? broodId = null,
            int? year = null,
            int? broilerHouseId = null,
            string? dailyCheckWeek = null,
            string? dailyCheckDay = null)
        {
            /*
             * Retrieves the active Daily Check records matching
             * the filters selected by the user.
             */
            var dailyChecks = await GetAllActiveAsync(
                broodId,
                year,
                broilerHouseId,
                dailyCheckWeek,
                dailyCheckDay);

            /*
  * UI Data | Brood Filter Options
  * Retrieves active Broods associated with active Broiler Houses.
  *
  * When a Broiler House is selected, only its associated
  * Broods are included in the dropdown menu.
  */
            var broodQuery = _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState);

            if (broilerHouseId.HasValue)
            {
                broodQuery = broodQuery.Where(brood =>
                    brood.BroilerHouseId ==
                    broilerHouseId.Value);
            }

            /*
             * Retrieves the required Brood information before grouping.
             */
            var availableBroods = await broodQuery
                .Select(brood => new
                {
                    brood.BroodId,
                    brood.BroodName,
                    Year = brood.BroodDate.Year
                })
                .ToListAsync();

            /*
             * Groups Broods by name and year so only one option is
             * displayed for each Brood and year combination.
             */
            var broodOptions = availableBroods
                .GroupBy(brood => new
                {
                    brood.BroodName,
                    brood.Year
                })
                .Select(group => group
                    .OrderBy(brood =>
                        brood.BroodId)
                    .First())
                .OrderBy(brood =>
                    brood.BroodName)
                .ThenByDescending(brood =>
                    brood.Year)
                .Select(brood => new SelectListItem
                {
                    Value =
                        brood.BroodId.ToString(),

                    Text =
                        brood.BroodName +
                        " - " +
                        brood.Year,

                    Selected =
                        broodId.HasValue &&
                        brood.BroodId ==
                        broodId.Value
                })
                .ToList();

            /*
             * UI Data | Year Filter Options
             * Retrieves the different years from active Broods
             * associated with active Broiler Houses.
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
                    Selected =
                        year.HasValue &&
                        broodYear ==
                        year.Value
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
                    Value =
                        broilerHouse.BroilerHouseId.ToString(),

                    Text =
                        broilerHouse.BroilerHouseName,

                    Selected =
                        broilerHouseId.HasValue &&
                        broilerHouse.BroilerHouseId ==
                        broilerHouseId.Value
                })
                .ToListAsync();

            /*
             * UI Data | Daily Check Week Filter Options
             * Converts the valid Daily Check week values into
             * SelectListItem objects used by the week dropdown menu.
             */
            var dailyCheckWeekOptions = ValidDailyCheckWeeks
                .Select(week => new SelectListItem
                {
                    Value = week,
                    Text = week,

                    Selected =
                        !string.IsNullOrWhiteSpace(
                            dailyCheckWeek) &&
                        week == dailyCheckWeek
                })
                .ToList();

            /*
             * UI Data | Daily Check Day Filter Options
             * Converts the valid Daily Check day values into
             * SelectListItem objects used by the day dropdown menu.
             */
            var dailyCheckDayOptions = ValidDailyCheckDays
                .Select(day => new SelectListItem
                {
                    Value = day,
                    Text = day,

                    Selected =
                        !string.IsNullOrWhiteSpace(
                            dailyCheckDay) &&
                        day == dailyCheckDay
                })
                .ToList();

            return new DailyCheckFilterViewModel
            {
                BroilerHouseId =
                    broilerHouseId,

                BroodId =
                    broodId,

                Year =
                    year,

                DailyCheckWeek =
                    dailyCheckWeek,

                DailyCheckDay =
                    dailyCheckDay,

                BroilerHouseOptions =
                    broilerHouseOptions,

                BroodOptions =
                    broodOptions,

                YearOptions =
                    yearOptions,

                DailyCheckWeekOptions =
                    dailyCheckWeekOptions,

                DailyCheckDayOptions =
                    dailyCheckDayOptions,

                DailyChecks =
                    dailyChecks
            };
        }

        /*
         * UI Data | Daily Check Create Form
         * Creates the ViewModel required by the Create view and loads
         * the Broiler House, Brood, Daily Check week and Daily Check day options.
         */
        public async Task<DailyCheckFormViewModel> GetCreateViewModelAsync()
        {
            var model = new DailyCheckFormViewModel
            {
                DailyCheckDate = DateTime.Today
            };

            await PopulateFormOptionsAsync(model);

            return model;
        }

        /*
 * UI Data | Daily Check Form Options
 * Loads the dropdown options required by the Create and Edit forms.
 *
 * Only active Broiler Houses are available. Brood options are loaded
 * according to the selected Broiler House and must have at least one
 * active Income Concentrate record.
 */
        public async Task PopulateFormOptionsAsync(
            DailyCheckFormViewModel model)
        {
            /*
             * UI Data | Broiler House Options
             * Loads the active Broiler Houses available in the form.
             */
            model.BroilerHouseOptions = await _context.BroilerHouses
                .AsNoTracking()
                .Where(broilerHouse =>
                    broilerHouse.BroilerHouseState)
                .OrderBy(broilerHouse =>
                    broilerHouse.BroilerHouseName)
                .Select(broilerHouse => new SelectListItem
                {
                    Value = broilerHouse.BroilerHouseId.ToString(),
                    Text = broilerHouse.BroilerHouseName,
                    Selected =
                        broilerHouse.BroilerHouseId ==
                        model.BroilerHouseId
                })
                .ToListAsync();

            /*
             * UI Data | Brood Options
             * Loads only the active Broods associated with the selected
             * Broiler House and with at least one active Income Concentrate.
             */
            if (model.BroilerHouseId > 0)
            {
                model.BroodOptions = await _context.Broods
                    .AsNoTracking()
                    .Where(brood =>
                        brood.BroilerHouseId == model.BroilerHouseId &&
                        brood.BroodState &&
                        brood.BroilerHouse != null &&
                        brood.BroilerHouse.BroilerHouseState &&
                        _context.IncomeConcentrates.Any(income =>
                            income.BroodId == brood.BroodId &&
                            income.IncomeState))
                    .OrderBy(brood =>
                        brood.BroodName)
                    .ThenByDescending(brood =>
                        brood.BroodDate)
                    .Select(brood => new SelectListItem
                    {
                        Value = brood.BroodId.ToString(),
                        Text =
                            brood.BroodName +
                            " - " +
                            brood.BroodDate.Year,
                        Selected =
                            brood.BroodId ==
                            model.BroodId
                    })
                    .ToListAsync();
            }
            else
            {
                model.BroodOptions = new List<SelectListItem>();
            }

            model.DailyCheckWeekOptions = ValidDailyCheckWeeks
                .Select(week => new SelectListItem
                {
                    Value = week,
                    Text = week,
                    Selected =
                        week == model.DailyCheckWeek
                })
                .ToList();

            model.DailyCheckDayOptions = ValidDailyCheckDays
                .Select(day => new SelectListItem
                {
                    Value = day,
                    Text = day,
                    Selected =
                        day == model.DailyCheckDay
                })
                .ToList();
        }


        /*
         * UI Data | Broods by Broiler House
         * Retrieves the active Broods associated with the selected
         * Broiler House and with at least one active Income Concentrate.
         *
         * This method is used to update the Brood dropdown dynamically
         * after the user selects a Broiler House.
         */
        public async Task<List<SelectListItem>> GetBroodsByBroilerHouseAsync(
            int broilerHouseId)
        {
            if (broilerHouseId <= 0)
            {
                return new List<SelectListItem>();
            }

            return await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroilerHouseId == broilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState &&
                    _context.IncomeConcentrates.Any(income =>
                        income.BroodId == brood.BroodId &&
                        income.IncomeState))
                .OrderBy(brood =>
                    brood.BroodName)
                .ThenByDescending(brood =>
                    brood.BroodDate)
                .Select(brood => new SelectListItem
                {
                    Value = brood.BroodId.ToString(),
                    Text =
                        brood.BroodName +
                        " - " +
                        brood.BroodDate.Year
                })
                .ToListAsync();
        }

        /*
         * UI Data | Selected Brood Information
         * Retrieves the information displayed in the Daily Check form
         * after the user selects a Brood.
         *
         * The selected Brood must belong to the selected Broiler House.
         * The latest active Income Concentrate record is used as the
         * current concentrate reference for the selected Brood.
         */
        public async Task<DailyCheckFormViewModel?> GetBroodInformationAsync(
            int broilerHouseId,
            int broodId)
        {
            var brood = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodId == broodId &&
                    brood.BroilerHouseId == broilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState)
                .Select(brood => new
                {
                    brood.BroodId,
                    brood.BroilerHouseId,
                    brood.BroodBirdInitialNum
                })
                .FirstOrDefaultAsync();

            if (brood == null)
            {
                return null;
            }

            var incomeConcentrate = await _context.IncomeConcentrates
                .AsNoTracking()
                .Where(income =>
                    income.BroodId == broodId &&
                    income.IncomeState)
                .OrderByDescending(income =>
                    income.IncomeConcentrateDate)
                .ThenByDescending(income =>
                    income.IncomeConcentrateId)
                .Select(income => new
                {
                    income.IncomeConcentrateId,
                    income.IncomeAccumulated
                })
                .FirstOrDefaultAsync();

            if (incomeConcentrate == null)
            {
                return null;
            }

            /*
             * Business Calculation | Current Accumulated Mortality
             * Retrieves the mortality already registered in active
             * Daily Checks belonging to the selected Brood.
             */
            var accumulatedMortality = await _context.DailyChecks
                .AsNoTracking()
                .Where(dailyCheck =>
                    dailyCheck.BroodId == broodId &&
                    dailyCheck.DailyCheckState)
                .SumAsync(dailyCheck =>
                    (int?)dailyCheck.TotalDailyMortality) ?? 0;

            /*
             * Business Calculation | Current Accumulated Consumption
             * Retrieves the consumption already registered in active
             * Daily Checks belonging to the selected Brood.
             */
            var accumulatedConsumption = await _context.DailyChecks
                .AsNoTracking()
                .Where(dailyCheck =>
                    dailyCheck.BroodId == broodId &&
                    dailyCheck.DailyCheckState)
                .SumAsync(dailyCheck =>
                    (decimal?)dailyCheck.ConsumptionKilos) ?? 0;

            var dailyBirdBalance = brood.BroodBirdInitialNum - accumulatedMortality;

            var concentrateBalance = incomeConcentrate.IncomeAccumulated - accumulatedConsumption;

            return new DailyCheckFormViewModel
            {
                BroilerHouseId = brood.BroilerHouseId,

                BroodId = brood.BroodId,

                BroodBirdInitialNum = brood.BroodBirdInitialNum,

                IncomeConcentrateId = incomeConcentrate.IncomeConcentrateId,

                IncomeAccumulated = incomeConcentrate.IncomeAccumulated,

                AccumulatedMortality = accumulatedMortality,

                DailyBirdBalance = dailyBirdBalance,

                AccumulatedConsumption = accumulatedConsumption,

                ConcentrateBalance = concentrateBalance
            };
        }

        /*
         * Business Operation | Create Daily Check
         * Validates the selected Brood, Broiler House, Income Concentrate,
         * week and day before creating a new Daily Check record.
         *
         * All calculated values are generated by the Service. Values received
         * from read-only form fields are not trusted when saving the record.
         */
        public async Task CreateAsync( DailyCheckFormViewModel model)
        {
            /*
             * Business Validation | Daily Check Week
             * Confirms that the submitted week belongs to the values
             * supported by the Daily Check module.
             */
            if (!ValidDailyCheckWeeks.Contains(model.DailyCheckWeek))
            {
                throw new InvalidOperationException(
                    "La semana de control seleccionada no es válida.");
            }

            /*
             * Business Validation | Daily Check Day
             * Confirms that the submitted day belongs to the values
             * supported by the Daily Check module.
             */
            if (!ValidDailyCheckDays.Contains(model.DailyCheckDay))
            {
                throw new InvalidOperationException(
                    "El día de control seleccionado no es válido.");
            }

            /*
             * Business Validation | Brood Availability
             * Confirms that the selected Brood exists, is active, belongs to
             * the selected Broiler House and that the Broiler House is active.
             */
            var brood = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodId == model.BroodId &&
                    brood.BroilerHouseId == model.BroilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState)
                .Select(brood => new
                {
                    brood.BroodId,
                    brood.BroilerHouseId,
                    brood.BroodBirdInitialNum
                })
                .FirstOrDefaultAsync();

            if (brood == null)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no pertenece a la pollera indicada o no está disponible.");
            }

            /*
             * Business Validation | Duplicate Daily Check
             * Prevents more than one active Daily Check from being registered
             * for the same Brood, week and day.
             */
            var duplicateExists = await _context.DailyChecks
                .AsNoTracking()
                .AnyAsync(dailyCheck =>
                    dailyCheck.BroodId == model.BroodId &&
                    dailyCheck.DailyCheckWeek == model.DailyCheckWeek &&
                    dailyCheck.DailyCheckDay == model.DailyCheckDay &&
                    dailyCheck.DailyCheckState);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Ya existe un control diario activo para la camada, semana y día seleccionados.");
            }

            /*
             * Business Validation | Income Concentrate Availability
             * Retrieves the latest active Income Concentrate associated
             * with the selected Brood.
             */
            var incomeConcentrate = await _context.IncomeConcentrates
                .AsNoTracking()
                .Where(income =>
                    income.BroodId == model.BroodId &&
                    income.IncomeState)
                .OrderByDescending(income =>
                    income.IncomeConcentrateDate)
                .ThenByDescending(income =>
                    income.IncomeConcentrateId)
                .Select(income => new
                {
                    income.IncomeConcentrateId,
                    income.IncomeAccumulated
                })
                .FirstOrDefaultAsync();

            if (incomeConcentrate == null)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no tiene ingresos de concentrado activos.");
            }

            /*
             * Business Calculation | Daily Mortality
             * Calculates the total mortality registered in the current
             * Daily Check.
             */
            var totalDailyMortality =
                model.NaturalMortality +
                model.SelectQuantity;

            /*
             * Business Calculation | Previous Accumulated Mortality
             * Retrieves the accumulated mortality from all active
             * Daily Checks belonging to the selected Brood.
             */
            var previousAccumulatedMortality =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId == model.BroodId &&
                        dailyCheck.DailyCheckState)
                    .SumAsync(dailyCheck =>
                        (int?)dailyCheck.TotalDailyMortality) ?? 0;

            var accumulatedMortality = previousAccumulatedMortality + totalDailyMortality;

            var dailyBirdBalance = brood.BroodBirdInitialNum - accumulatedMortality;

            if (dailyBirdBalance < 0)
            {
                throw new InvalidOperationException(
                    "La mortalidad acumulada no puede superar la cantidad inicial de aves de la camada.");
            }

            /*
             * Business Calculation | Daily Consumption
             * Converts the entered quintals into kilograms using the
             * project conversion rule of 46 kilograms per quintal.
             */
            var consumptionKilos = model.ConsumptionQuintals * KilosPerQuintal;

            /*
             * Business Calculation | Previous Accumulated Consumption
             * Retrieves the accumulated consumption from all active
             * Daily Checks belonging to the selected Brood.
             */
            var previousAccumulatedConsumption =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId == model.BroodId &&
                        dailyCheck.DailyCheckState)
                    .SumAsync(dailyCheck =>
                        (decimal?)dailyCheck.ConsumptionKilos) ?? 0;

            var accumulatedConsumption = previousAccumulatedConsumption +
                consumptionKilos;

            var concentrateBalance = incomeConcentrate.IncomeAccumulated - accumulatedConsumption;

            if (concentrateBalance < 0)
            {
                throw new InvalidOperationException(
                    "El consumo acumulado no puede superar el concentrado acumulado disponible.");
            }

            /*
             * Entity Mapping | Daily Check
             * Creates the Daily Check entity using the validated user-entered
             * values and the calculated business values.
             */
            var dailyCheck = new DailyCheck
            {
                DailyCheckDate = model.DailyCheckDate.Date,
                DailyCheckWeek = model.DailyCheckWeek,
                DailyCheckDay = model.DailyCheckDay,
                NaturalMortality = model.NaturalMortality,
                SelectQuantity = model.SelectQuantity,
                TotalDailyMortality = totalDailyMortality,
                AccumulatedMortality = accumulatedMortality,
                DailyBirdBalance = dailyBirdBalance,
                ConsumptionQuintals = model.ConsumptionQuintals,
                ConsumptionKilos = consumptionKilos,
                AccumulatedConsumption = accumulatedConsumption,
                ConcentrateBalance = concentrateBalance,
                DailyCheckDescription =
                    string.IsNullOrWhiteSpace(model.DailyCheckDescription)
                        ? null
                        : model.DailyCheckDescription.Trim(),
                DailyCheckState = true,
                BroodId = model.BroodId,
                IncomeConcentrateId =
                    incomeConcentrate.IncomeConcentrateId
            };

            /*
             * Database Transaction | Create and Recalculate
             * Ensures that the Daily Check creation and recalculation are
             * completed as a single operation. If one fails, no changes are saved.
             * Microsoft Learn - EF Core Transactions: https://learn.microsoft.com/en-us/ef/core/saving/transactions
            */
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.DailyChecks.Add(dailyCheck);

                await _context.SaveChangesAsync();

                /*
                 * Recalculates every active Daily Check because a user may
                 * register a control using an earlier date, week or day.
                 */
                await RecalculateDailyChecksAsync(model.BroodId);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        /*
         * Business Calculation | Recalculate Daily Checks
         * Recalculates all accumulated and balance values for the active
         * Daily Check records belonging to the specified Brood.
         *
         * Records are processed by week, day, date and identifier to preserve
         * a consistent logical order for the accumulated calculations.
         */
        private async Task RecalculateDailyChecksAsync(int broodId)
        {
            var dailyChecks = await _context.DailyChecks
                .Include(dailyCheck =>
                    dailyCheck.Brood)
                .Include(dailyCheck =>
                    dailyCheck.IncomeConcentrate)
                .Where(dailyCheck =>
                    dailyCheck.BroodId == broodId &&
                    dailyCheck.DailyCheckState)
                .OrderBy(dailyCheck =>
                    dailyCheck.DailyCheckWeek)
                .ThenBy(dailyCheck =>
                    dailyCheck.DailyCheckDay)
                .ThenBy(dailyCheck =>
                    dailyCheck.DailyCheckDate)
                .ThenBy(dailyCheck =>
                    dailyCheck.DailyCheckId)
                .ToListAsync();

            var accumulatedMortality = 0;
            decimal accumulatedConsumption = 0;

            foreach (var dailyCheck in dailyChecks)
            {
                dailyCheck.TotalDailyMortality =
                    dailyCheck.NaturalMortality +
                    dailyCheck.SelectQuantity;

                accumulatedMortality +=
                    dailyCheck.TotalDailyMortality;

                dailyCheck.AccumulatedMortality =
                    accumulatedMortality;

                dailyCheck.DailyBirdBalance =
                    dailyCheck.Brood.BroodBirdInitialNum -
                    accumulatedMortality;

                dailyCheck.ConsumptionKilos = dailyCheck.ConsumptionQuintals * KilosPerQuintal;

                accumulatedConsumption +=
                    dailyCheck.ConsumptionKilos;

                dailyCheck.AccumulatedConsumption =
                    accumulatedConsumption;

                dailyCheck.ConcentrateBalance =
                    dailyCheck.IncomeConcentrate.IncomeAccumulated -
                    accumulatedConsumption;
            }

            await _context.SaveChangesAsync();
        }

        /*
         * Data Query | Daily Check by Identifier
         * Retrieves the complete Daily Check information required
         * by the Details, Delete and Activate views.
         *
         * Related Broiler House, Brood and Income Concentrate data
         * are projected into DailyCheckGetByIdViewModel.
         */
        public async Task<DailyCheckGetByIdViewModel?> GetByIdAsync(int id)
        {
            return await _context.DailyChecks
                .AsNoTracking()
                .Where(dailyCheck =>
                    dailyCheck.DailyCheckId == id)
                .Select(dailyCheck => new DailyCheckGetByIdViewModel
                {
                    DailyCheckId = dailyCheck.DailyCheckId,

                    DailyCheckState = dailyCheck.DailyCheckState,

                    DailyCheckDate = dailyCheck.DailyCheckDate,

                    BroilerHouseName = dailyCheck.Brood.BroilerHouse.BroilerHouseName,

                    BroodName = dailyCheck.Brood.BroodName,

                    DailyCheckWeek = dailyCheck.DailyCheckWeek,

                    DailyCheckDay = dailyCheck.DailyCheckDay,

                    BroodBirdInitialNum = dailyCheck.Brood.BroodBirdInitialNum,

                    NaturalMortality = dailyCheck.NaturalMortality,

                    SelectQuantity = dailyCheck.SelectQuantity,

                    TotalDailyMortality = dailyCheck.TotalDailyMortality,

                    AccumulatedMortality = dailyCheck.AccumulatedMortality,

                    DailyBirdBalance = dailyCheck.DailyBirdBalance,

                    IncomeAccumulated = dailyCheck.IncomeConcentrate.IncomeAccumulated,

                    ConsumptionQuintals = dailyCheck.ConsumptionQuintals,

                    ConsumptionKilos = dailyCheck.ConsumptionKilos,

                    AccumulatedConsumption = dailyCheck.AccumulatedConsumption,

                    ConcentrateBalance = dailyCheck.ConcentrateBalance,

                    DailyCheckDescription = dailyCheck.DailyCheckDescription
                })
                .FirstOrDefaultAsync();
        }



    }
}