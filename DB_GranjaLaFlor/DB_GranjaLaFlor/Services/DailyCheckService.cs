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

        //Inyecting WeeklyCheckService to recalculate data when editing. to use "RecalculateAffectedWeeklyChecksAsync" method...
        private readonly WeeklyCheckService _weeklyCheckService;

        public DailyCheckService(ApplicationDbContext context, WeeklyCheckService weeklyCheckService)
        {
            _context = context;
            _weeklyCheckService = weeklyCheckService;
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
         *
         * Retrieves the information displayed in the Daily Check form
         * after the user selects a Brood and Daily Check date.
         *
         * The selected Brood must belong to the selected Broiler House.
         * The Income Concentrate used must be the most recent active
         * record available on or before the selected Daily Check date.
         */
        public async Task<DailyCheckFormViewModel?> GetBroodInformationAsync(
            int broilerHouseId,
            int broodId,
            DateTime dailyCheckDate)
        {
            /*
             * Business Validation | Brood Availability
             *
             * Confirms that the selected Brood exists, is active,
             * belongs to the selected Broiler House and that the
             * Broiler House is also active.
             */
            var brood =
                await _context.Broods
                    .AsNoTracking()
                    .Where(brood =>
                        brood.BroodId ==
                            broodId &&
                        brood.BroilerHouseId ==
                            broilerHouseId &&
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


            /*
             * UI Data | Available Income Concentrate
             *
             * Retrieves the most recent active Income Concentrate
             * available on or before the selected Daily Check date.
             *
             * This prevents the form from displaying concentrate
             * that entered the Brood after the operational date.
             */
            var incomeConcentrate =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .Where(income =>
                        income.BroodId ==
                            broodId &&
                        income.IncomeState &&
                        income.IncomeConcentrateDate <=
                            dailyCheckDate.Date)
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
             * Business Calculation | Accumulated Mortality
             *
             * Retrieves mortality from active Daily Checks
             * registered before the selected Daily Check date.
             *
             * Future Daily Checks must not affect the historical
             * preview of the selected date.
             */
            var accumulatedMortality =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId ==
                            broodId &&
                        dailyCheck.DailyCheckState &&
                        dailyCheck.DailyCheckDate <
                            dailyCheckDate.Date)
                    .SumAsync(dailyCheck =>
                        (int?)dailyCheck.TotalDailyMortality)
                    ?? 0;


            /*
             * Business Calculation | Accumulated Consumption
             *
             * Retrieves concentrate consumption from active
             * Daily Checks registered before the selected date.
             *
             * The current Daily Check consumption is not included
             * because it has not yet been created.
             */
            var accumulatedConsumption =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId ==
                            broodId &&
                        dailyCheck.DailyCheckState &&
                        dailyCheck.DailyCheckDate <
                            dailyCheckDate.Date)
                    .SumAsync(dailyCheck =>
                        (decimal?)dailyCheck.ConsumptionKilos)
                    ?? 0;


            /*
             * Business Calculation | Current Bird Balance
             */
            var dailyBirdBalance =
                brood.BroodBirdInitialNum -
                accumulatedMortality;


            /*
             * Business Calculation | Current Concentrate Balance
             *
             * Calculates the concentrate available immediately
             * before the new Daily Check is registered.
             */
            var concentrateBalance =
                incomeConcentrate.IncomeAccumulated -
                accumulatedConsumption;


            /*
             * ViewModel Mapping | Daily Check Form
             */
            return new DailyCheckFormViewModel
            {
                BroilerHouseId =
                    brood.BroilerHouseId,

                BroodId =
                    brood.BroodId,

                BroodBirdInitialNum =
                    brood.BroodBirdInitialNum,

                IncomeConcentrateId =
                    incomeConcentrate.IncomeConcentrateId,

                IncomeAccumulated =
                    incomeConcentrate.IncomeAccumulated,

                AccumulatedMortality =
                    accumulatedMortality,

                DailyBirdBalance =
                    dailyBirdBalance,

                AccumulatedConsumption =
                    accumulatedConsumption,

                ConcentrateBalance =
                    concentrateBalance
            };
        }

        /*
         * Business Operation | Create Daily Check
         * Validates the selected Brood, Broiler House, Income Concentrate,
         * week and day before creating a new Daily Check record.
         *
         * User-entered values and relationships are assigned by this method.
         * All calculated values are generated by RecalculateDailyChecksAsync.
         */
        public async Task CreateAsync(DailyCheckFormViewModel model)
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
            var broodExists = await _context.Broods
                .AsNoTracking()
                .AnyAsync(brood =>
                    brood.BroodId == model.BroodId &&
                    brood.BroilerHouseId == model.BroilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse.BroilerHouseState);

            if (!broodExists)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no pertenece a la pollera " +
                    "indicada o no está disponible.");
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
                    "Ya existe un control diario activo para la camada, " +
                    "semana y día seleccionados.");
            }

            /*
             * Business Validation | Income Concentrate Availability
             *
             * Retrieves the most recent active Income Concentrate
             * available on or before the Daily Check date.
             *
             * A Daily Check must not use concentrate that entered
             * the Brood after the date of the operational control.
             */
            var incomeConcentrate =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .Where(income =>
                        income.BroodId ==
                            model.BroodId &&
                        income.IncomeState &&
                        income.IncomeConcentrateDate <=
                            model.DailyCheckDate.Date)
                    .OrderByDescending(income =>
                        income.IncomeConcentrateDate)
                    .ThenByDescending(income =>
                        income.IncomeConcentrateId)
                    .Select(income => new
                    {
                        income.IncomeConcentrateId
                    })
                    .FirstOrDefaultAsync();

            if (incomeConcentrate == null)
            {
                throw new InvalidOperationException(
                    "No existe un ingreso de concentrado disponible " +
                    "para la fecha del control diario seleccionado.");
            }

            /*
             * Entity Mapping | Daily Check
             * Creates the Daily Check entity using the validated user-entered
             * values and the selected relationships.
             *
             * Calculated properties are initialized temporarily because their
             * database columns do not allow null values. Their official values
             * are generated by RecalculateDailyChecksAsync before committing.
             */
            var dailyCheck = new DailyCheck
            {
                DailyCheckDate = model.DailyCheckDate.Date,
                DailyCheckWeek = model.DailyCheckWeek,
                DailyCheckDay = model.DailyCheckDay,
                NaturalMortality = model.NaturalMortality,
                SelectQuantity = model.SelectQuantity,

                TotalDailyMortality = 0,
                AccumulatedMortality = 0,
                DailyBirdBalance = 0,

                ConsumptionQuintals = model.ConsumptionQuintals,
                ConsumptionKilos = 0,
                AccumulatedConsumption = 0,
                ConcentrateBalance = 0,

                DailyCheckDescription =
                    string.IsNullOrWhiteSpace(
                        model.DailyCheckDescription)
                        ? null
                        : model.DailyCheckDescription.Trim(),

                DailyCheckState = true,
                BroodId = model.BroodId,

                IncomeConcentrateId =
                    incomeConcentrate.IncomeConcentrateId
            };

            /*
             * Database Transaction | Create and Recalculate: Ensures that the Daily Check creation and recalculation are
             * completed as a single operation. If one fails, no changes are saved.
             * Microsoft Learn - EF Core Transactions: https://learn.microsoft.com/en-us/ef/core/saving/transactions
             */
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.DailyChecks.Add(dailyCheck);

                /*
                 * Saves the new Daily Check with temporary calculated values
                 * so it can be included in the complete Brood recalculation.
                 */
                await _context.SaveChangesAsync();

                /*
                 * Recalculates every active Daily Check because a user may
                 * register a control using an earlier date, week or day.
                 */
                await RecalculateDailyChecksAsync(
                    model.BroodId);

                /*
                 * Database Transaction | Commit
                 * Confirms the Daily Check creation and all related recalculations.
                 */
                await transaction.CommitAsync();
            }
            catch
            {
                /*
                 * Database Transaction | Rollback
                 * Reverts the new Daily Check and every recalculated value
                 * when any operation inside the transaction fails.
                 */
                await transaction.RollbackAsync();

                /*
                 * Error Propagation | Original Exception
                 * Sends the same exception to the Controller so it can be
                 * logged and handled without losing the original error details.
                 */
                throw;
            }
        }


        /*
         * Business Calculation | Recalculate Daily Checks
         *
         * Recalculates mortality, bird balance, consumption and
         * concentrate balance for every active Daily Check
         * belonging to the specified Brood.
         *
         * The method also verifies the Income Concentrate associated
         * with each Daily Check according to its operational date.
         *
         * The method is public because other operational Services,
         * such as IncomeConcentrateService, may modify information
         * used by Daily Check calculated values.
         */
        public async Task RecalculateDailyChecksAsync(
            int broodId)
        {
            /*
             * Database Query | Active Income Concentrates
             *
             * Retrieves all active Income Concentrates associated
             * with the Brood in chronological order.
             *
             * These records are used to determine which concentrate
             * was actually available on each Daily Check date.
             */
            var incomeConcentrates =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .Where(income =>
                        income.BroodId ==
                            broodId &&
                        income.IncomeState)
                    .OrderBy(income =>
                        income.IncomeConcentrateDate)
                    .ThenBy(income =>
                        income.IncomeConcentrateId)
                    .Select(income => new
                    {
                        income.IncomeConcentrateId,
                        income.IncomeConcentrateDate,
                        income.IncomeAccumulated
                    })
                    .ToListAsync();


            /*
             * Database Query | Active Daily Checks
             *
             * Retrieves tracked Daily Check entities because their
             * calculated values and Income Concentrate relationship
             * may be updated by this method.
             */
            var dailyChecks =
                await _context.DailyChecks
                    .Include(dailyCheck =>
                        dailyCheck.Brood)
                    .Where(dailyCheck =>
                        dailyCheck.BroodId ==
                            broodId &&
                        dailyCheck.DailyCheckState)
                    .OrderBy(dailyCheck =>
                        dailyCheck.DailyCheckDate)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckWeek)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckDay)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckId)
                    .ToListAsync();


            /*
             * Running Totals | Daily Checks
             */
            var accumulatedMortality =
                0;

            decimal accumulatedConsumption =
                0;


            foreach (var dailyCheck in dailyChecks)
            {
                /*
                 * Business Validation | Income Concentrate by Date
                 *
                 * Determines the most recent active Income Concentrate
                 * available on or before the current Daily Check date.
                 *
                 * A Daily Check must never use concentrate that entered
                 * the Brood after its operational date.
                 */
                var availableIncome =
                    incomeConcentrates
                        .Where(income =>
                            income.IncomeConcentrateDate <=
                                dailyCheck.DailyCheckDate.Date)
                        .OrderByDescending(income =>
                            income.IncomeConcentrateDate)
                        .ThenByDescending(income =>
                            income.IncomeConcentrateId)
                        .FirstOrDefault();

                if (availableIncome == null)
                {
                    throw new InvalidOperationException(
                        "No existe un ingreso de concentrado disponible " +
                        "para la fecha de uno de los controles diarios.");
                }


                /*
                 * Relationship Update | Income Concentrate
                 *
                 * Updates the Foreign Key when another active
                 * Income Concentrate has become the correct
                 * chronological reference for this Daily Check.
                 */
                dailyCheck.IncomeConcentrateId =
                    availableIncome.IncomeConcentrateId;


                /*
                 * Business Calculation | Daily Mortality
                 */
                dailyCheck.TotalDailyMortality =
                    dailyCheck.NaturalMortality +
                    dailyCheck.SelectQuantity;


                /*
                 * Business Calculation | Accumulated Mortality
                 */
                accumulatedMortality +=
                    dailyCheck.TotalDailyMortality;

                dailyCheck.AccumulatedMortality =
                    accumulatedMortality;


                /*
                 * Business Calculation | Bird Balance
                 */
                dailyCheck.DailyBirdBalance =
                    dailyCheck.Brood.BroodBirdInitialNum -
                    accumulatedMortality;

                if (dailyCheck.DailyBirdBalance < 0)
                {
                    throw new InvalidOperationException(
                        "La mortalidad acumulada no puede superar " +
                        "la cantidad inicial de aves de la camada.");
                }


                /*
                 * Business Calculation | Daily Consumption
                 *
                 * Converts quintals into kilograms using the
                 * project's standard conversion factor.
                 */
                dailyCheck.ConsumptionKilos =
                    dailyCheck.ConsumptionQuintals *
                    KilosPerQuintal;


                /*
                 * Business Calculation | Accumulated Consumption
                 */
                accumulatedConsumption +=
                    dailyCheck.ConsumptionKilos;

                dailyCheck.AccumulatedConsumption =
                    accumulatedConsumption;


                /*
                 * Business Calculation | Concentrate Balance
                 *
                 * Uses the accumulated concentrate that was available
                 * according to the current Daily Check date.
                 */
                dailyCheck.ConcentrateBalance =
                    availableIncome.IncomeAccumulated -
                    accumulatedConsumption;

                if (dailyCheck.ConcentrateBalance < 0)
                {
                    throw new InvalidOperationException(
                        "El consumo acumulado no puede superar " +
                        "el concentrado acumulado disponible.");
                }
            }


            /*
             * Database Operation | Save Recalculated Values
             *
             * Persists calculated values and any IncomeConcentrateId
             * reassignment required by the chronological relationship.
             */
            await _context.SaveChangesAsync();
        }

        /*
         * Business Calculation | Recalculate Operational Chain
         *
         * Recalculates the Daily Check calculated values and then
         * propagates those changes to any existing Weekly Checks
         * associated with the same Brood.
         *
         * This method should be used by other operational Services
         * when their changes can affect Daily Check information.
         */
        public async Task RecalculateOperationalChainAsync(
            int broodId)
        {
            /*
             * Business Calculation | Daily Checks
             *
             * Recalculates mortality, bird balance, consumption,
             * Income Concentrate relationship and concentrate balance.
             */
            await RecalculateDailyChecksAsync(
                broodId);

            /*
             * Business Calculation | Weekly Checks
             *
             * Daily Check calculated values are source information
             * for existing Weekly Checks.
             *
             * Recalculates the affected Weekly Checks after the
             * Daily Check information has been updated.
             */
            await _weeklyCheckService
                .RecalculateAffectedWeeklyChecksAsync(
                    broodId);
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

        /*
         * Data Query | Daily Check Form by Identifier: Retrieves an active Daily Check and converts it into
         * DailyCheckFormViewModel for the Edit view.
         *
         * The dropdown options are loaded after retrieving the record so the current Broiler House, Brood, week and day remain selected.
         */
        public async Task<DailyCheckFormViewModel?> GetFormByIdAsync(int id)
        {
            var model = await _context.DailyChecks
                .AsNoTracking()
                .Where(dailyCheck =>
                    dailyCheck.DailyCheckId == id &&
                    dailyCheck.DailyCheckState)
                .Select(dailyCheck => new DailyCheckFormViewModel
                {
                    DailyCheckId = dailyCheck.DailyCheckId,
                    DailyCheckDate = dailyCheck.DailyCheckDate,

                    BroilerHouseId = dailyCheck.Brood.BroilerHouseId,

                    BroodId = dailyCheck.BroodId,

                    DailyCheckWeek = dailyCheck.DailyCheckWeek,

                    DailyCheckDay =dailyCheck.DailyCheckDay,

                    NaturalMortality = dailyCheck.NaturalMortality,

                    SelectQuantity = dailyCheck.SelectQuantity,

                    ConsumptionQuintals = dailyCheck.ConsumptionQuintals,

                    DailyCheckDescription = dailyCheck.DailyCheckDescription,

                    IncomeConcentrateId =dailyCheck.IncomeConcentrateId,

                    BroodBirdInitialNum = dailyCheck.Brood.BroodBirdInitialNum,

                    IncomeAccumulated = dailyCheck.IncomeConcentrate.IncomeAccumulated,

                    TotalDailyMortality = dailyCheck.TotalDailyMortality,

                    AccumulatedMortality = dailyCheck.AccumulatedMortality,

                    DailyBirdBalance = dailyCheck.DailyBirdBalance,

                    ConsumptionKilos =dailyCheck.ConsumptionKilos,

                    AccumulatedConsumption = dailyCheck.AccumulatedConsumption,

                    ConcentrateBalance = dailyCheck.ConcentrateBalance
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateFormOptionsAsync(model);

            return model;
        }

        /*
 * Business Operation | Update Daily Check
 *
 * Validates and updates an active Daily Check record.
 *
 * User-entered values and relationships are updated by this method.
 * All calculated Daily Check and affected Weekly Check values are
 * recalculated before the transaction is committed.
 */
        public async Task UpdateAsync(
            DailyCheckFormViewModel model)
        {
            /*
             * Business Validation | Daily Check Week
             *
             * Confirms that the submitted week belongs to the values
             * supported by the Daily Check module.
             */
            if (!ValidDailyCheckWeeks.Contains(
                model.DailyCheckWeek))
            {
                throw new InvalidOperationException(
                    "La semana de control seleccionada no es válida.");
            }


            /*
             * Business Validation | Daily Check Day
             *
             * Confirms that the submitted day belongs to the values
             * supported by the Daily Check module.
             */
            if (!ValidDailyCheckDays.Contains(
                model.DailyCheckDay))
            {
                throw new InvalidOperationException(
                    "El día de control seleccionado no es válido.");
            }


            /*
             * Business Validation | Existing Daily Check
             *
             * Confirms that the Daily Check exists and is currently active.
             */
            var existingDailyCheck =
                await _context.DailyChecks
                    .FirstOrDefaultAsync(
                        dailyCheck =>
                            dailyCheck.DailyCheckId ==
                                model.DailyCheckId &&
                            dailyCheck.DailyCheckState);

            if (existingDailyCheck == null)
            {
                throw new InvalidOperationException(
                    "El control diario seleccionado no existe o está inactivo.");
            }


            /*
             * Brood Tracking | Previous Brood
             *
             * Stores the original Brood identifier because the
             * Daily Check may be moved to another Brood.
             */
            var previousBroodId =
                existingDailyCheck.BroodId;


            /*
             * Business Validation | Brood Availability
             *
             * Confirms that the selected Brood exists, is active,
             * belongs to the selected Broiler House and that the
             * Broiler House is also active.
             */
            var broodExists =
                await _context.Broods
                    .AsNoTracking()
                    .AnyAsync(
                        brood =>
                            brood.BroodId ==
                                model.BroodId &&
                            brood.BroilerHouseId ==
                                model.BroilerHouseId &&
                            brood.BroodState &&
                            brood.BroilerHouse != null &&
                            brood.BroilerHouse.BroilerHouseState);

            if (!broodExists)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no pertenece a la pollera " +
                    "indicada o no está disponible.");
            }


            /*
             * Business Validation | Duplicate Daily Check
             *
             * Prevents another active Daily Check from using the
             * same Brood, week and day combination.
             *
             * The current Daily Check is excluded.
             */
            var duplicateExists =
                await _context.DailyChecks
                    .AsNoTracking()
                    .AnyAsync(
                        dailyCheck =>
                            dailyCheck.DailyCheckId !=
                                model.DailyCheckId &&
                            dailyCheck.BroodId ==
                                model.BroodId &&
                            dailyCheck.DailyCheckWeek ==
                                model.DailyCheckWeek &&
                            dailyCheck.DailyCheckDay ==
                                model.DailyCheckDay &&
                            dailyCheck.DailyCheckState);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Ya existe otro control diario activo para la camada, " +
                    "semana y día seleccionados.");
            }


            /*
             * Business Validation | Income Concentrate Availability
             *
             * Retrieves the most recent active Income Concentrate
             * available on or before the selected Daily Check date.
             */
            var incomeConcentrate =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .Where(
                        income =>
                            income.BroodId ==
                                model.BroodId &&
                            income.IncomeState &&
                            income.IncomeConcentrateDate <=
                                model.DailyCheckDate.Date)
                    .OrderByDescending(
                        income =>
                            income.IncomeConcentrateDate)
                    .ThenByDescending(
                        income =>
                            income.IncomeConcentrateId)
                    .Select(
                        income => new
                        {
                            income.IncomeConcentrateId
                        })
                    .FirstOrDefaultAsync();

            if (incomeConcentrate == null)
            {
                throw new InvalidOperationException(
                    "No existe un ingreso de concentrado disponible " +
                    "para la fecha del control diario seleccionado.");
            }


            /*
             * Entity Mapping | Daily Check
             *
             * Updates only user-entered values and relationships.
             *
             * All calculated values are generated later by the
             * operational recalculation chain.
             */
            existingDailyCheck.DailyCheckDate =
                model.DailyCheckDate.Date;

            existingDailyCheck.DailyCheckWeek =
                model.DailyCheckWeek;

            existingDailyCheck.DailyCheckDay =
                model.DailyCheckDay;

            existingDailyCheck.NaturalMortality =
                model.NaturalMortality;

            existingDailyCheck.SelectQuantity =
                model.SelectQuantity;

            existingDailyCheck.ConsumptionQuintals =
                model.ConsumptionQuintals;

            existingDailyCheck.DailyCheckDescription =
                string.IsNullOrWhiteSpace(
                    model.DailyCheckDescription)
                    ? null
                    : model.DailyCheckDescription.Trim();

            existingDailyCheck.BroodId =
                model.BroodId;

            existingDailyCheck.IncomeConcentrateId =
                incomeConcentrate.IncomeConcentrateId;


            /*
             * Database Transaction | Update and Recalculate
             *
             * Ensures that the Daily Check update and all related
             * Daily Check and Weekly Check recalculations are
             * completed as a single operation.
             */
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                /*
                 * Database Operation | Save Updated Values
                 *
                 * Saves user-entered information before recalculating
                 * the complete operational chain.
                 */
                await _context.SaveChangesAsync();


                /*
                 * Business Calculation | Previous Brood
                 *
                 * Recalculates the complete operational chain:
                 *
                 * Daily Checks
                 *      ↓
                 * Weekly Checks
                 */
                await RecalculateOperationalChainAsync(
                    previousBroodId);


                /*
                 * Business Calculation | New Brood
                 *
                 * When the Daily Check was moved to another Brood,
                 * the new Brood must also be completely recalculated.
                 */
                if (previousBroodId !=
                    model.BroodId)
                {
                    await RecalculateOperationalChainAsync(
                        model.BroodId);
                }


                /*
                 * Database Transaction | Commit
                 */
                await transaction
                    .CommitAsync();
            }
            catch
            {
                /*
                 * Database Transaction | Rollback
                 *
                 * Reverts every change performed inside the transaction
                 * when the update or any recalculation fails.
                 */
                await transaction
                    .RollbackAsync();

                /*
                 * Error Propagation | Original Exception
                 */
                throw;
            }
        }

        /*
         * Business Operation | Soft Delete Daily Check
         * Logically deactivates an active Daily Check record.
         *
         * The record remains stored in the database with its state set to false.
         * All remaining active Daily Checks of the Brood are recalculated after
         * the record is deactivated.
         */
        public async Task SoftDeleteAsync(int id)
        {
            /*
             * Business Validation | Existing Daily Check
             * Confirms that the Daily Check exists and is currently active.
             */
            var dailyCheck = await _context.DailyChecks
                .FirstOrDefaultAsync(dailyCheck =>
                    dailyCheck.DailyCheckId == id);

            if (dailyCheck == null)
            {
                throw new InvalidOperationException(
                    "El control diario seleccionado no existe.");
            }

            /*
             * Business Validation | Daily Check State
             * Prevents an inactive Daily Check from being deactivated again.
             */
            if (!dailyCheck.DailyCheckState)
            {
                throw new InvalidOperationException(
                    "El control diario seleccionado ya se encuentra inactivo.");
            }

            /*
             * Business Validation | Weekly Check Dependency
             *
             * Prevents the Daily Check from being deactivated when an
             * active Weekly Check has already been generated for the
             * same Brood and production week.
             *
             * Weekly Checks are calculated from the seven active Daily
             * Checks belonging to the same Brood and week. Deactivating
             * one of those records would invalidate the operational
             * information used by the existing Weekly Check.
             */
            var weeklyCheckExists =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .AnyAsync(weeklyCheck =>
                        weeklyCheck.BroodId ==
                            dailyCheck.BroodId &&
                        weeklyCheck.WeeklyCheckWeek ==
                            dailyCheck.DailyCheckWeek &&
                        weeklyCheck.WeeklyCheckState);

            if (weeklyCheckExists)
            {
                throw new InvalidOperationException(
                    "El control diario no puede ser desactivado porque " +
                    "ya existe un control semanal activo asociado " +
                    "a la misma camada y semana.");
            }

            /*
             * Stores the Brood identifier before changing the record state.
             * The identifier is required to recalculate the remaining
             * active Daily Checks associated with the same Brood.
             */
            var broodId = dailyCheck.BroodId;

            /*
             * Logical Deletion | Daily Check State
             * Changes the record state to false without removing
             * the Daily Check physically from the database.
             */
            dailyCheck.DailyCheckState = false;

            /*
             * Database Transaction | Soft Delete and Recalculate
             * Ensures that the Daily Check deactivation and recalculation are
             * completed as a single operation. If one fails, no changes are saved.
             *
             * Source:
             * Microsoft Learn - EF Core Transactions.
             */
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                /*
                 * Saves the inactive state before recalculating so the
                 * deactivated Daily Check is excluded from the active records.
                 */
                await _context.SaveChangesAsync();

                /*
                 * Recalculates all remaining active Daily Checks of the Brood
                 * after excluding the deactivated record.
                 */
                await RecalculateDailyChecksAsync(
                    broodId);

                /*
                 * Database Transaction | Commit
                 * Confirms the Daily Check deactivation and all
                 * related recalculations.
                 */
                await transaction.CommitAsync();
            }
            catch
            {
                /*
                 * Database Transaction | Rollback
                 * Reverts the Daily Check state and every recalculated value
                 * when any operation inside the transaction fails.
                 */
                await transaction.RollbackAsync();

                /*
                 * Error Propagation | Original Exception
                 * Sends the same exception to the Controller so it can be
                 * logged and handled without losing the original error details.
                 */
                throw;
            }
        }



    }
}