using DB_GranjaLaFlor.Data.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        /*
         * Data Query | Active Daily Checks
         * Retrieves active Daily Check records and applies the optional filters
         * selected by the user in the Index view.
         *
         * The query is projected into DailyCheckListViewModel so the view
         * receives only the information required to display the records.
         */
        public async Task<List<DailyCheckListViewModel>> GetAllActiveAsync(
            string? broodName = null,
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

            if (!string.IsNullOrWhiteSpace(broodName))
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.Brood.BroodName == broodName);
            }

            if (year.HasValue)
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.Brood.BroodDate.Year == year.Value);
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
                    dailyCheck.DailyCheckWeek == dailyCheckWeek);
            }

            if (!string.IsNullOrWhiteSpace(dailyCheckDay))
            {
                query = query.Where(dailyCheck =>
                    dailyCheck.DailyCheckDay == dailyCheckDay);
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
                    DailyCheckId = dailyCheck.DailyCheckId,

                    /*
                     * Daily Check information.
                     */
                    DailyCheckDate = dailyCheck.DailyCheckDate,
                    DailyCheckWeek = dailyCheck.DailyCheckWeek,
                    DailyCheckDay = dailyCheck.DailyCheckDay,

                    /*
                     * Broiler House and Brood information.
                     */
                    BroilerHouseName = dailyCheck.Brood.BroilerHouse.BroilerHouseName,
                    BroodId = dailyCheck.BroodId,
                    BroodName = dailyCheck.Brood.BroodName,
                    BroodYear = dailyCheck.Brood.BroodDate.Year,
                    BroodBirdInitialNum = dailyCheck.Brood.BroodBirdInitialNum,

                    /*
                     * Income Concentrate information.
                     *
                     * IncomeAccumulated is obtained from the
                     * Income Concentrate record associated with
                     * the Daily Check. It is not stored directly
                     * in the daily_checks table.
                     */
                    IncomeConcentrateId = dailyCheck.IncomeConcentrateId,
                    IncomeAccumulated = dailyCheck.IncomeConcentrate.IncomeAccumulated,

                    /*
                     * User-entered values.
                     */
                    NaturalMortality = dailyCheck.NaturalMortality,
                    SelectQuantity = dailyCheck.SelectQuantity,
                    ConsumptionQuintals = dailyCheck.ConsumptionQuintals,

                    /*
                     * Calculated values.
                     */
                    ConsumptionKilos = dailyCheck.ConsumptionKilos,
                    TotalDailyMortality = dailyCheck.TotalDailyMortality,
                    AccumulatedMortality = dailyCheck.AccumulatedMortality,
                    DailyBirdBalance = dailyCheck.DailyBirdBalance,
                    AccumulatedConsumption = dailyCheck.AccumulatedConsumption,
                    ConcentrateBalance = dailyCheck.ConcentrateBalance,
                    DailyCheckState = dailyCheck.DailyCheckState
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
            string? broodName = null,
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
                broodName,
                year,
                broilerHouseId,
                dailyCheckWeek,
                dailyCheckDay);

            /*
             * UI Data | Brood Filter Options
             * Retrieves the unique names of active Broods associated
             * with active Broiler Houses.
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
             * Converts each available Brood name into a SelectListItem
             * used by the Brood dropdown menu.
             */
            var broodOptions = availableBroodNames
                .Select(name => new SelectListItem
                {
                    Value = name,
                    Text = name,

                    /*
                     * Preserves the selected Brood name after
                     * submitting the filter form.
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

                    Selected =
                        broilerHouseId.HasValue &&
                        broilerHouse.BroilerHouseId ==
                        broilerHouseId.Value
                })
                .ToListAsync();

            /*
             * UI Data | Daily Check Week Filter Options
             * Creates the fixed Daily Check week values supported
             * by the current Daily Check design.
             */
            var dailyCheckWeeks = new List<string>
            {
                "Semana 1",
                "Semana 2",
                "Semana 3",
                "Semana 4",
                "Semana 5",
                "Semana 6",
                "Semana 7"
            };

            /*
             * Converts every Daily Check week into a SelectListItem
             * used by the Daily Check week dropdown menu.
             */
            var dailyCheckWeekOptions = dailyCheckWeeks
                .Select(week => new SelectListItem
                {
                    Value = week,
                    Text = week,

                    Selected =
                        !string.IsNullOrWhiteSpace(dailyCheckWeek) &&
                        week == dailyCheckWeek
                })
                .ToList();

            /*
             * UI Data | Daily Check Day Filter Options
             * Creates the fixed Daily Check day values used by the
             * current Daily Check design.
             */
            var dailyCheckDays = new List<string>
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
             * Converts every Daily Check day into a SelectListItem
             * used by the Daily Check day dropdown menu.
             */
            var dailyCheckDayOptions = dailyCheckDays
                .Select(day => new SelectListItem
                {
                    Value = day,
                    Text = day,

                    Selected =
                        !string.IsNullOrWhiteSpace(dailyCheckDay) &&
                        day == dailyCheckDay
                })
                .ToList();

            return new DailyCheckFilterViewModel
            {
                BroodName = broodName,
                Year = year,
                BroilerHouseId = broilerHouseId,
                DailyCheckWeek = dailyCheckWeek,
                DailyCheckDay = dailyCheckDay,
                BroodOptions = broodOptions,
                YearOptions = yearOptions,
                BroilerHouseOptions = broilerHouseOptions,
                DailyCheckWeekOptions = dailyCheckWeekOptions,
                DailyCheckDayOptions = dailyCheckDayOptions,
                DailyChecks = dailyChecks
            };
        }
    }
}