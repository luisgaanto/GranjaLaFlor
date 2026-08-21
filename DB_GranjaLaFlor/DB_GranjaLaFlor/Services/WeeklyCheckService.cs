using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Architecture Decision | Service Layer
     * Business logic, calculations and database operations
     * are implemented inside the Service layer.
     *
     * Controllers coordinate HTTP requests and responses only.
     */
    public class WeeklyCheckService
    {
        private readonly ApplicationDbContext _context;

        /*
         * Business Values | Weekly Check Weeks
         * Defines the six fixed weeks supported by the
         * Weekly Check and Expected Value modules.
         */
        private static readonly string[] ValidWeeklyCheckWeeks =
        {
            "Semana 1",
            "Semana 2",
            "Semana 3",
            "Semana 4",
            "Semana 5",
            "Semana 6"
        };

        /*
         * Business Values | Daily Check Days
         * Defines the seven Daily Check days required
         * to generate a complete Weekly Check.
         */
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
         * Business Value | Sample Percentage
         * Represents the two percent of the current bird
         * population used by the weekly weight control.
         */
        private const decimal SamplePercentage = 0.02m;


        /*
         * Business Calculation | Three-Decimal Precision
         * Applies the precision supported by Weekly Check
         * weight and consumption database columns.
         */
        private static decimal RoundToThreeDecimals(
            decimal value)
        {
            return Math.Round(
                value,
                3,
                MidpointRounding.AwayFromZero);
        }

        /*
         * Business Calculation | Two-Decimal Precision
         * Applies the precision supported by Weekly Check
         * conversion and mortality database columns.
         */
        private static decimal RoundToTwoDecimals(
            decimal value)
        {
            return Math.Round(
                value,
                2,
                MidpointRounding.AwayFromZero);
        }

        public WeeklyCheckService(
            ApplicationDbContext context)
        {
            _context = context;
        }


        /*
         * Data Query | Active Weekly Checks
         * Retrieves a maximum of ten active Weekly Check records
         * matching the filters selected by the user.
         *
         * Results are projected into WeeklyCheckListViewModel
         * so the Index view does not depend directly on entities.
         */
        public async Task<List<WeeklyCheckListViewModel>> GetAllActiveAsync(int? broodId, int? year,int? broilerHouseId,string? weeklyCheckWeek)
        {
            var query = _context.WeeklyChecks
                .AsNoTracking()
                .Where(weeklyCheck =>
                    weeklyCheck.WeeklyCheckState);

            /*
             * Data Filter | Brood
             * Filters Weekly Checks by the selected Brood.
             */
            if (broodId.HasValue)
            {
                query = query.Where(weeklyCheck =>
                    weeklyCheck.BroodId ==
                        broodId.Value);
            }

            /*
             * Data Filter | Brood Year
             * Filters records using the year associated
             * with the selected Brood.
             */
            if (year.HasValue)
            {
                query = query.Where(weeklyCheck =>
                    weeklyCheck.Brood.BroodDate.Year ==
                        year.Value);
            }

            /*
             * Data Filter | Broiler House
             * Filters records through the Brood and
             * Broiler House relationship.
             */
            if (broilerHouseId.HasValue)
            {
                query = query.Where(weeklyCheck =>
                    weeklyCheck.Brood.BroilerHouseId ==
                        broilerHouseId.Value);
            }

            /*
             * Data Filter | Weekly Check Week
             * Filters records by the selected production week.
             */
            if (!string.IsNullOrWhiteSpace(
                weeklyCheckWeek))
            {
                query = query.Where(weeklyCheck =>
                    weeklyCheck.WeeklyCheckWeek ==
                        weeklyCheckWeek);
            }

            return await query
                .OrderByDescending(weeklyCheck =>
                    weeklyCheck.Brood.BroodDate)
                .ThenByDescending(weeklyCheck =>
                    weeklyCheck.WeeklyCheckId)
                .Select(weeklyCheck => new WeeklyCheckListViewModel
                    {
                        WeeklyCheckId = weeklyCheck.WeeklyCheckId,

                        BroilerHouseName = weeklyCheck.Brood.BroilerHouse.BroilerHouseName,

                        BroodId = weeklyCheck.BroodId,

                        BroodName = weeklyCheck.Brood.BroodName,

                        BroodYear = weeklyCheck.Brood.BroodDate.Year,

                        WeeklyCheckWeek = weeklyCheck.WeeklyCheckWeek,
                        /*
                         * Weekly Weight
                         */
                        AverageWeeklyWeight = weeklyCheck.AverageWeeklyWeight,

                        WeeklyExpectedWeight = weeklyCheck.WeeklyExpectedWeight,
                        /*
                         * Weekly Consumption
                         */
                        WeeklyRealConsumption = weeklyCheck.WeeklyRealConsumption,

                        WeeklyExpectedConsumption = weeklyCheck.WeeklyExpectedConsumption,
                        /*
                         * Weekly Conversion
                         */
                        WeeklyRealConversion = weeklyCheck.WeeklyRealConversion,

                        WeeklyExpectedConversion = weeklyCheck.WeeklyExpectedConversion,
                        /*
                         * Weekly Mortality
                        */
                        WeeklyRealMortality = weeklyCheck.WeeklyRealMortality,

                        WeeklyExpectedMortality = weeklyCheck.WeeklyExpectedMortality,

                        WeeklyCheckState = weeklyCheck.WeeklyCheckState
                    })
                .Take(10)
                .ToListAsync();
        }

        /*
         * UI Data | Weekly Check Index Filter
         * Retrieves the Weekly Check records, selected filter values
         * and dropdown options required by the Index view.
         */
        public async Task<WeeklyCheckFilterViewModel> GetFilterViewModelAsync(int? broodId, int? year, int? broilerHouseId, string? weeklyCheckWeek)
        {
            var weeklyChecks = await GetAllActiveAsync( broodId, year, broilerHouseId, weeklyCheckWeek);

            /*
             * UI Data | Available Broods
             * Retrieves active Broods associated with active
             * Broiler Houses.
             *
             * When a Broiler House is selected, only its
             * associated Broods are included.
             */
            var broodQuery = _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse
                        .BroilerHouseState);

            if (broilerHouseId.HasValue)
            {
                broodQuery = broodQuery.Where(brood =>
                    brood.BroilerHouseId ==
                        broilerHouseId.Value);
            }

            var availableBroods = await broodQuery
                .Select(brood => new
                {
                    brood.BroodId,
                    brood.BroodName,
                    Year = brood.BroodDate.Year
                })
                .ToListAsync();

            /*
             * UI Data | Brood Filter Options
             * Creates the Brood dropdown options displayed
             * as BroodName and year.
             */
            var broodOptions = availableBroods
                .OrderBy(brood =>
                    brood.BroodName)
                .ThenByDescending(brood =>
                    brood.Year)
                .Select(brood =>
                    new SelectListItem
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
             * Retrieves the distinct years associated
             * with active Broods.
             */
            var yearOptions = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse
                        .BroilerHouseState)
                .Select(brood =>
                    brood.BroodDate.Year)
                .Distinct()
                .OrderByDescending(broodYear =>
                    broodYear)
                .Select(broodYear =>
                    new SelectListItem
                    {
                        Value = broodYear.ToString(),

                        Text = broodYear.ToString(),

                        Selected =
                            year.HasValue &&
                            broodYear ==
                                year.Value
                    })
                .ToListAsync();

            /*
             * UI Data | Broiler House Filter Options
             * Retrieves all active Broiler Houses used
             * by the Index filter.
             */
            var broilerHouseOptions =
                await _context.BroilerHouses
                    .AsNoTracking()
                    .Where(broilerHouse =>
                        broilerHouse
                            .BroilerHouseState)
                    .OrderBy(broilerHouse =>
                        broilerHouse
                            .BroilerHouseName)
                    .Select(broilerHouse =>
                        new SelectListItem
                        {
                            Value = broilerHouse.BroilerHouseId
                            .ToString(),

                            Text = broilerHouse.BroilerHouseName,

                            Selected = broilerHouseId.HasValue &&
                                broilerHouse.BroilerHouseId ==
                                broilerHouseId.Value
                        })
                    .ToListAsync();

            /*
             * UI Data | Weekly Check Week Options
             * Creates the fixed production week options
             * supported by the Weekly Check module.
             */
            var weeklyCheckWeekOptions =
                ValidWeeklyCheckWeeks
                    .Select(week =>
                        new SelectListItem
                        {
                            Value = week,
                            Text = week,

                            Selected =
                                !string.IsNullOrWhiteSpace(
                                    weeklyCheckWeek) &&
                                week ==
                                    weeklyCheckWeek
                        })
                    .ToList();

            return new WeeklyCheckFilterViewModel
            {
                BroilerHouseId = broilerHouseId,

                BroodId = broodId,

                Year = year,

                WeeklyCheckWeek = weeklyCheckWeek,

                BroilerHouseOptions = broilerHouseOptions,

                BroodOptions = broodOptions,

                YearOptions = yearOptions,

                WeeklyCheckWeekOptions = weeklyCheckWeekOptions,

                WeeklyChecks = weeklyChecks
            };
        }

        /*
         * UI Data | Weekly Check Create Form
         * Creates the initial ViewModel required by the Weekly Check
         * Create view and loads its dropdown options.
         */
        public async Task<WeeklyCheckFormViewModel>
                GetCreateViewModelAsync()
        {
            var model =
                new WeeklyCheckFormViewModel();

            await PopulateFormOptionsAsync(model);

            return model;
        }

        /*
         * UI Data | Weekly Check Form Options
         * Loads the Broiler House, Brood and week options required
         * by the Weekly Check Create and Edit forms.
         *
         * When a Broiler House is already selected, only its active
         * Broods are included in the Brood dropdown.
         */
        public async Task PopulateFormOptionsAsync(WeeklyCheckFormViewModel model)
        {
            /*
             * UI Data | Broiler House Options
             * Retrieves the active Broiler Houses available
             * for the Weekly Check form.
             */
            model.BroilerHouseOptions =
                await _context.BroilerHouses
                    .AsNoTracking()
                    .Where(broilerHouse =>
                        broilerHouse.BroilerHouseState)
                    .OrderBy(broilerHouse =>
                        broilerHouse.BroilerHouseName)
                    .Select(broilerHouse =>
                        new SelectListItem
                        {
                            Value =
                                broilerHouse.BroilerHouseId
                                    .ToString(),

                            Text =
                                broilerHouse.BroilerHouseName,

                            Selected =
                                broilerHouse.BroilerHouseId ==
                                model.BroilerHouseId
                        })
                    .ToListAsync();

            /*
             * UI Data | Brood Options
             * Loads the active Broods associated with the selected
             * Broiler House.
             */
            if (model.BroilerHouseId > 0)
            {
                model.BroodOptions =
                    await _context.Broods
                        .AsNoTracking()
                        .Where(brood =>
                            brood.BroilerHouseId ==
                                model.BroilerHouseId &&
                            brood.BroodState &&
                            brood.BroilerHouse != null &&
                            brood.BroilerHouse
                                .BroilerHouseState)
                        .OrderBy(brood =>
                            brood.BroodName)
                        .ThenByDescending(brood =>
                            brood.BroodDate)
                        .Select(brood =>
                            new SelectListItem
                            {
                                Value =
                                    brood.BroodId.ToString(),

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
                model.BroodOptions =
                    new List<SelectListItem>();
            }

            /*
             * UI Data | Weekly Check Week Options
             * Creates the six fixed production week values
             * supported by the Weekly Check module.
             */
            model.WeeklyCheckWeekOptions =
                ValidWeeklyCheckWeeks
                    .Select(week =>
                        new SelectListItem
                        {
                            Value = week,
                            Text = week,

                            Selected =
                                !string.IsNullOrWhiteSpace(
                                    model.WeeklyCheckWeek) &&
                                week ==
                                    model.WeeklyCheckWeek
                        })
                    .ToList();
        }

        /*
         * UI Data | Broods by Broiler House
         * Retrieves the active Broods associated with the selected
         * active Broiler House.
         *
         * This method is used to update the Brood dropdown dynamically
         * after the user selects a Broiler House.
         */
        public async Task<List<SelectListItem>>GetBroodsByBroilerHouseAsync(int broilerHouseId)
        {
            if (broilerHouseId <= 0)
            {
                return new List<SelectListItem>();
            }

            return await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroilerHouseId ==
                        broilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse
                        .BroilerHouseState)
                .OrderBy(brood =>
                    brood.BroodName)
                .ThenByDescending(brood =>
                    brood.BroodDate)
                .Select(brood =>
                    new SelectListItem
                    {
                        Value =
                            brood.BroodId.ToString(),

                        Text =
                            brood.BroodName +
                            " - " +
                            brood.BroodDate.Year
                    })
                .ToListAsync();
        }


        /*
         * UI Data | Weekly Check Information
         * Retrieves the selected Brood, its seven active Daily Checks
         * and the Expected Value record associated with the selected week.
         *
         * The method also generates the Weekly Check calculation preview.
         * Official values are recalculated again by CreateAsync or UpdateAsync
         * before the Weekly Check is saved.
         */
        public async Task<WeeklyCheckFormViewModel?>GetWeeklyCheckInformationAsync(
                int broilerHouseId,
                int broodId,
                string weeklyCheckWeek,
                decimal totalBirdWeight = 0)
        {
            /*
             * Business Validation | Weekly Check Week
             * Confirms that the selected week belongs to the six
             * values supported by the Weekly Check module.
             */
            if (!ValidWeeklyCheckWeeks.Contains(
                weeklyCheckWeek))
            {
                throw new InvalidOperationException(
                    "La semana seleccionada no es válida.");
            }

            /*
             * Business Validation | Brood Availability
             * Confirms that the Brood exists, is active, belongs to
             * the selected Broiler House and that its Broiler House is active.
             */
            var brood = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodId == broodId &&
                    brood.BroilerHouseId ==
                        broilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse
                        .BroilerHouseState)
                .Select(brood => new
                {
                    brood.BroodId,
                    brood.BroodBirdInitialNum
                })
                .FirstOrDefaultAsync();

            if (brood == null)
            {
                return null;
            }

            /*
             * Business Validation | Expected Value Availability
             * Retrieves the Expected Value record associated with
             * the selected Weekly Check week.
             */
            var expectedValue =
                await _context.ExpectedValues
                    .AsNoTracking()
                    .Where(expectedValue =>
                        expectedValue.ExpectedValueWeek ==
                            weeklyCheckWeek)
                    .Select(expectedValue => new
                    {
                        expectedValue.ExpectedValueId,
                        expectedValue.ExpectedConsumption,
                        expectedValue.ExpectedWeight,
                        expectedValue.ExpectedConversion,
                        expectedValue.ExpectedMortality
                    })
                    .FirstOrDefaultAsync();

            if (expectedValue == null)
            {
                throw new InvalidOperationException(
                    "No existen valores esperados para la semana seleccionada.");
            }

            /*
             * Data Query | Weekly Daily Checks
             * Retrieves the active Daily Check records associated
             * with the selected Brood and week.
             */
            var dailyChecks =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId == broodId &&
                        dailyCheck.DailyCheckWeek ==
                            weeklyCheckWeek &&
                        dailyCheck.DailyCheckState)
                    .OrderBy(dailyCheck =>
                        dailyCheck.DailyCheckDate)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckDay)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckId)
                    .Select(dailyCheck => new WeeklyCheckDailyCheckViewModel
                        {
                            DailyCheckId = dailyCheck.DailyCheckId,
                            DailyCheckDate = dailyCheck.DailyCheckDate,
                            DailyCheckDay = dailyCheck.DailyCheckDay,
                            DailyCheckWeek = dailyCheck.DailyCheckWeek ?? string.Empty,
                            TotalDailyMortality = dailyCheck.TotalDailyMortality,
                            AccumulatedMortality = dailyCheck.AccumulatedMortality,
                            DailyBirdBalance = dailyCheck.DailyBirdBalance,
                            IncomeAccumulated = dailyCheck.IncomeConcentrate.IncomeAccumulated,
                            ConsumptionKilos = dailyCheck.ConsumptionKilos,
                            AccumulatedConsumption = dailyCheck.AccumulatedConsumption,
                            ConcentrateBalance = dailyCheck.ConcentrateBalance
                    })
                    .ToListAsync();

            /*
             * Business Validation | Complete Daily Check Week
             * Confirms that exactly seven active Daily Checks exist
             * for the selected Brood and week.
             */
            if (dailyChecks.Count != ValidDailyCheckDays.Length)
            {
                throw new InvalidOperationException(
                    "Debe existir un total de 7 controles diarios activos " +
                    "para generar el control semanal.");
            }

            /*
             * Business Validation | Required Daily Check Days
             * Confirms that Day 1 through Day 7 are each represented
             * exactly once in the selected Weekly Check period.
             */
            var containsAllRequiredDays =
                ValidDailyCheckDays.All(requiredDay =>
                    dailyChecks.Count(dailyCheck =>
                        dailyCheck.DailyCheckDay ==
                            requiredDay) == 1);

            if (!containsAllRequiredDays)
            {
                throw new InvalidOperationException(
                    "Los controles diarios deben incluir exactamente " +
                    "un registro desde Día 1 hasta Día 7.");
            }

            /*
             * Data Selection | Final Daily Check
             * Retrieves Día 7 because its accumulated values represent
             * the final operational status of the selected week.
             */
            var finalDailyCheck =
                dailyChecks.Single(dailyCheck =>
                    dailyCheck.DailyCheckDay ==
                        "Día 7");

            if (finalDailyCheck.DailyBirdBalance <= 0)
            {
                throw new InvalidOperationException(
                    "El saldo final de aves debe ser mayor que cero.");
            }

            /*
             * Business Calculation | Weekly Bird Sample
             * Calculates two percent of the final active bird balance.
             *
             * Decimal.Ceiling guarantees that the sample contains
             * at least two percent of the current bird population.
             */
            var sampleBirdQuantity =
                (int)decimal.Ceiling(
                    finalDailyCheck.DailyBirdBalance *
                    SamplePercentage);

            if (sampleBirdQuantity <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad de aves de la muestra debe ser mayor que cero.");
            }

            /*
             * Business Calculation | Real Weekly Consumption
             * Calculates the accumulated feed consumption per active
             * bird at the end of the selected week.
             */
            var weeklyRealConsumption =
                finalDailyCheck.AccumulatedConsumption /
                finalDailyCheck.DailyBirdBalance;

            weeklyRealConsumption =
                RoundToThreeDecimals(
                    weeklyRealConsumption);

            /*
             * Business Calculation | Weekly Consumption Difference
             * Calculates the difference between the real weekly
             * consumption and the expected consumption.
             */
            var weeklyConsumptionDifference =
                weeklyRealConsumption -
                expectedValue.ExpectedConsumption;

            weeklyConsumptionDifference =
                RoundToThreeDecimals(
                    weeklyConsumptionDifference);

            /*
             * Business Calculation | Real Weekly Mortality
             * Calculates the accumulated mortality percentage using
             * the initial bird quantity of the selected Brood.
             */
            if (brood.BroodBirdInitialNum <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad inicial de aves debe ser mayor que cero.");
            }

            var weeklyRealMortality =
                ((decimal)finalDailyCheck.AccumulatedMortality /
                    brood.BroodBirdInitialNum) *
                100;

            weeklyRealMortality =
                RoundToTwoDecimals(
                    weeklyRealMortality);

            /*
             * Business Calculation | Weekly Mortality Difference
             * Calculates the difference between the real mortality
             * percentage and the expected mortality percentage.
             */
            var weeklyMortalityDifference =
                weeklyRealMortality -
                expectedValue.ExpectedMortality;

            weeklyMortalityDifference =
                RoundToTwoDecimals(
                    weeklyMortalityDifference);

            /*
             * Business Calculation | Weight-Dependent Values
             * Weight, weight difference and conversion calculations
             * are generated only after the user enters a valid
             * total sample weight.
             */
            decimal averageWeeklyWeight = 0;
            decimal weeklyWeightDifference = 0;
            decimal weeklyRealConversion = 0;
            decimal weeklyConversionDifference = 0;

            if (totalBirdWeight > 0)
            {
                /*
                 * Business Calculation | Average Weekly Weight
                 * Divides the total sample weight by the quantity
                 * of birds included in the sample.
                 */
                averageWeeklyWeight =
                    totalBirdWeight /
                    sampleBirdQuantity;

                averageWeeklyWeight =
                    RoundToThreeDecimals(
                        averageWeeklyWeight);

                /*
                 * Business Validation | Average Weekly Weight
                 * Confirms that the calculated average weekly
                 * weight is greater than zero.
                 */
                if (averageWeeklyWeight <= 0)
                {
                    throw new InvalidOperationException(
                        "El peso promedio semanal debe ser mayor que cero.");
                }

                /*
                 * Business Calculation | Weekly Weight Difference
                 * Calculates the difference between the real average
                 * weekly weight and the expected weekly weight.
                 */
                weeklyWeightDifference =
                    averageWeeklyWeight -
                    expectedValue.ExpectedWeight;

                weeklyWeightDifference =
                    RoundToThreeDecimals(
                        weeklyWeightDifference);

                /*
                 * Business Calculation | Real Weekly Conversion
                 * Divides the real accumulated consumption per bird
                 * by the real average weekly weight.
                 */
                weeklyRealConversion =
                    weeklyRealConsumption /
                    averageWeeklyWeight;

                weeklyRealConversion =
                    RoundToTwoDecimals(
                        weeklyRealConversion);

                /*
                 * Business Calculation | Weekly Conversion Difference
                 * Calculates the difference between the real weekly
                 * conversion and the expected conversion.
                 */
                weeklyConversionDifference =
                    weeklyRealConversion -
                    expectedValue.ExpectedConversion;

                weeklyConversionDifference =
                    RoundToTwoDecimals(
                        weeklyConversionDifference);
            }

            return new WeeklyCheckFormViewModel
            {
                BroilerHouseId = broilerHouseId,
                BroodId = broodId,
                WeeklyCheckWeek = weeklyCheckWeek,
                TotalBirdWeight = totalBirdWeight,
                ExpectedValueId = expectedValue.ExpectedValueId,
                BroodBirdInitialNum = brood.BroodBirdInitialNum,
                FinalDailyBirdBalance = finalDailyCheck.DailyBirdBalance,
                FinalAccumulatedConsumption = finalDailyCheck.AccumulatedConsumption,
                FinalConcentrateBalance =finalDailyCheck.ConcentrateBalance,
                FinalAccumulatedMortality = finalDailyCheck.AccumulatedMortality,
                WeeklyExpectedConsumption = expectedValue.ExpectedConsumption,
                WeeklyExpectedWeight = expectedValue.ExpectedWeight,
                WeeklyExpectedConversion = expectedValue.ExpectedConversion,
                WeeklyExpectedMortality = expectedValue.ExpectedMortality,
                SampleBirdQuantity = sampleBirdQuantity,
                AverageWeeklyWeight = averageWeeklyWeight,
                WeeklyRealConsumption = weeklyRealConsumption,
                WeeklyConsumptionDifference = weeklyConsumptionDifference,
                WeeklyWeightDifference = weeklyWeightDifference,
                WeeklyRealConversion = weeklyRealConversion,
                WeeklyConversionDifference = weeklyConversionDifference,
                WeeklyRealMortality = weeklyRealMortality,
                WeeklyMortalityDifference = weeklyMortalityDifference,
                DailyChecks = dailyChecks
            };
        }


        /*
         * Business Calculation | Recalculate Existing Weekly Checks
         *
         * Recalculates every active Weekly Check associated with the
         * specified Brood using the current Daily Check information.
         *
         * This method is used when operational Daily Check information
         * changes after a Weekly Check has already been generated.
         *
         * The user-entered TotalBirdWeight is preserved because it
         * belongs to the Weekly Check registration.
         */
        public async Task RecalculateAffectedWeeklyChecksAsync(
            int broodId)
        {
            /*
             * Data Query | Active Weekly Checks
             *
             * Retrieves tracked Weekly Check entities because their
             * calculated values will be updated and saved.
             */
            var weeklyChecks =
                await _context.WeeklyChecks
                    .Where(weeklyCheck =>
                        weeklyCheck.BroodId == broodId &&
                        weeklyCheck.WeeklyCheckState)
                    .ToListAsync();

            if (weeklyChecks.Count == 0)
            {
                return;
            }

            /*
             * Data Query | Brood
             *
             * Retrieves the initial bird population required for
             * mortality calculations.
             */
            var brood =
                await _context.Broods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(brood =>
                        brood.BroodId == broodId &&
                        brood.BroodState);

            if (brood == null)
            {
                throw new InvalidOperationException(
                    "La camada asociada al control semanal no existe o está inactiva.");
            }

            if (brood.BroodBirdInitialNum <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad inicial de aves debe ser mayor que cero.");
            }

            foreach (var weeklyCheck in weeklyChecks)
            {
                /*
                 * Data Query | Weekly Daily Checks
                 *
                 * Retrieves the active Daily Checks associated with
                 * the Weekly Check production week.
                 */
                var dailyChecks =
                    await _context.DailyChecks
                        .AsNoTracking()
                        .Where(dailyCheck =>
                            dailyCheck.BroodId == broodId &&
                            dailyCheck.DailyCheckWeek ==
                                weeklyCheck.WeeklyCheckWeek &&
                            dailyCheck.DailyCheckState)
                        .OrderBy(dailyCheck =>
                            dailyCheck.DailyCheckDate)
                        .ThenBy(dailyCheck =>
                            dailyCheck.DailyCheckId)
                        .ToListAsync();

                /*
                 * Business Validation | Complete Week
                 */
                if (dailyChecks.Count !=
                    ValidDailyCheckDays.Length)
                {
                    throw new InvalidOperationException(
                        $"No se puede recalcular {weeklyCheck.WeeklyCheckWeek} " +
                        "porque no existen exactamente 7 controles diarios activos.");
                }

                /*
                 * Business Validation | Required Days
                 */
                var containsAllRequiredDays =
                    ValidDailyCheckDays.All(requiredDay =>
                        dailyChecks.Count(dailyCheck =>
                            dailyCheck.DailyCheckDay ==
                                requiredDay) == 1);

                if (!containsAllRequiredDays)
                {
                    throw new InvalidOperationException(
                        $"No se puede recalcular {weeklyCheck.WeeklyCheckWeek} " +
                        "porque deben existir los controles desde Día 1 hasta Día 7.");
                }

                /*
                 * Data Selection | Final Daily Check
                 *
                 * Día 7 contains the final accumulated operational
                 * values used by the Weekly Check.
                 */
                var finalDailyCheck =
                    dailyChecks.Single(dailyCheck =>
                        dailyCheck.DailyCheckDay ==
                            "Día 7");

                if (finalDailyCheck.DailyBirdBalance <= 0)
                {
                    throw new InvalidOperationException(
                        "El saldo final de aves debe ser mayor que cero.");
                }

                /*
                 * Business Calculation | Sample Bird Quantity
                 *
                 * Calculates two percent of the final active population.
                 */
                var sampleBirdQuantity =
                    (int)decimal.Ceiling(
                        finalDailyCheck.DailyBirdBalance *
                        SamplePercentage);

                if (sampleBirdQuantity <= 0)
                {
                    throw new InvalidOperationException(
                        "La cantidad de aves de la muestra debe ser mayor que cero.");
                }

                /*
                 * Business Calculation | Average Weekly Weight
                 *
                 * TotalBirdWeight is preserved from the existing
                 * Weekly Check because it was entered by the user.
                 */
                var averageWeeklyWeight =
                    weeklyCheck.TotalBirdWeight /
                    sampleBirdQuantity;

                averageWeeklyWeight =
                    RoundToThreeDecimals(
                        averageWeeklyWeight);

                if (averageWeeklyWeight <= 0)
                {
                    throw new InvalidOperationException(
                        "El peso promedio semanal debe ser mayor que cero.");
                }

                /*
                 * Business Calculation | Real Weekly Consumption
                 */
                var weeklyRealConsumption =
                    finalDailyCheck.AccumulatedConsumption /
                    finalDailyCheck.DailyBirdBalance;

                weeklyRealConsumption =
                    RoundToThreeDecimals(
                        weeklyRealConsumption);

                /*
                 * Business Calculation | Consumption Difference
                 */
                var weeklyConsumptionDifference =
                    weeklyRealConsumption -
                    weeklyCheck.WeeklyExpectedConsumption;

                weeklyConsumptionDifference =
                    RoundToThreeDecimals(
                        weeklyConsumptionDifference);

                /*
                 * Business Calculation | Weight Difference
                 */
                var weeklyWeightDifference =
                    averageWeeklyWeight -
                    weeklyCheck.WeeklyExpectedWeight;

                weeklyWeightDifference =
                    RoundToThreeDecimals(
                        weeklyWeightDifference);

                /*
                 * Business Calculation | Real Weekly Conversion
                 */
                var weeklyRealConversion =
                    weeklyRealConsumption /
                    averageWeeklyWeight;

                weeklyRealConversion =
                    RoundToTwoDecimals(
                        weeklyRealConversion);

                /*
                 * Business Calculation | Conversion Difference
                 */
                var weeklyConversionDifference =
                    weeklyRealConversion -
                    weeklyCheck.WeeklyExpectedConversion;

                weeklyConversionDifference =
                    RoundToTwoDecimals(
                        weeklyConversionDifference);

                /*
                 * Business Calculation | Real Weekly Mortality
                 */
                var weeklyRealMortality =
                    ((decimal)finalDailyCheck.AccumulatedMortality /
                        brood.BroodBirdInitialNum) *
                    100;

                weeklyRealMortality =
                    RoundToTwoDecimals(
                        weeklyRealMortality);

                /*
                 * Business Calculation | Mortality Difference
                 */
                var weeklyMortalityDifference =
                    weeklyRealMortality -
                    weeklyCheck.WeeklyExpectedMortality;

                weeklyMortalityDifference =
                    RoundToTwoDecimals(
                        weeklyMortalityDifference);

                /*
                 * Entity Update | Weekly Check Calculated Values
                 */
                weeklyCheck.SampleBirdQuantity =
                    sampleBirdQuantity;

                weeklyCheck.AverageWeeklyWeight =
                    averageWeeklyWeight;

                weeklyCheck.WeeklyRealConsumption =
                    weeklyRealConsumption;

                weeklyCheck.WeeklyConsumptionDifference =
                    weeklyConsumptionDifference;

                weeklyCheck.WeeklyWeightDifference =
                    weeklyWeightDifference;

                weeklyCheck.WeeklyRealConversion =
                    weeklyRealConversion;

                weeklyCheck.WeeklyConversionDifference =
                    weeklyConversionDifference;

                weeklyCheck.WeeklyRealMortality =
                    weeklyRealMortality;

                weeklyCheck.WeeklyMortalityDifference =
                    weeklyMortalityDifference;
            }

            /*
             * Database Operation | Save Recalculated Weekly Checks
             */
            await _context.SaveChangesAsync();
        }



        /*
         * Business Operation | Create Weekly Check
         * Validates the selected Brood, Broiler House, week, Expected Values
         * and the seven active Daily Checks before creating a Weekly Check.
         *
         * All obtained and calculated values are generated by the Service.
         * Values received from read-only form fields are not trusted when
         * saving the record.
         */
        public async Task CreateAsync(WeeklyCheckFormViewModel model)
        {
            /*
             * Business Validation | Weekly Check Week
             * Confirms that the submitted week belongs to the six
             * values supported by the Weekly Check module.
             */
            if (!ValidWeeklyCheckWeeks.Contains(
                model.WeeklyCheckWeek))
            {
                throw new InvalidOperationException(
                    "La semana seleccionada no es válida.");
            }

            /*
             * Business Validation | Total Bird Weight
             * Confirms that the total weight entered for the
             * weekly bird sample is greater than zero.
             */
            if (model.TotalBirdWeight <= 0)
            {
                throw new InvalidOperationException(
                    "El peso total de la muestra debe ser mayor que cero.");
            }

            /*
             * Business Validation | Brood Availability
             * Confirms that the selected Brood exists, is active, belongs
             * to the selected Broiler House and that the Broiler House is active.
             */
            var brood = await _context.Broods
                .AsNoTracking()
                .Where(brood =>
                    brood.BroodId ==
                        model.BroodId &&
                    brood.BroilerHouseId ==
                        model.BroilerHouseId &&
                    brood.BroodState &&
                    brood.BroilerHouse != null &&
                    brood.BroilerHouse
                        .BroilerHouseState)
                .Select(brood => new
                {
                    brood.BroodId,
                    brood.BroodBirdInitialNum
                })
                .FirstOrDefaultAsync();

            if (brood == null)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no pertenece a la pollera indicada o no está disponible.");
            }

            /*
             * Business Validation | Initial Bird Quantity
             * Confirms that the selected Brood contains a valid
             * initial bird quantity required by mortality calculations.
             */
            if (brood.BroodBirdInitialNum <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad inicial de aves debe ser mayor que cero.");
            }

            /*
             * Business Validation | Duplicate Weekly Check
             * Prevents more than one active Weekly Check from being
             * registered for the same Brood and week.
             */
            var duplicateExists =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .AnyAsync(weeklyCheck =>
                        weeklyCheck.BroodId ==
                            model.BroodId &&
                        weeklyCheck.WeeklyCheckWeek ==
                            model.WeeklyCheckWeek &&
                        weeklyCheck.WeeklyCheckState);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "El control semanal ya fue generado para la camada y semana seleccionadas.");
            }

            /*
             * Business Validation | Expected Value Availability
             * Retrieves the Expected Value record associated with
             * the selected Weekly Check week.
             */
            var expectedValue =
                await _context.ExpectedValues
                    .AsNoTracking()
                    .Where(expectedValue =>
                        expectedValue.ExpectedValueWeek ==
                            model.WeeklyCheckWeek)
                    .Select(expectedValue => new
                    {
                        expectedValue.ExpectedValueId,
                        expectedValue.ExpectedConsumption,
                        expectedValue.ExpectedWeight,
                        expectedValue.ExpectedConversion,
                        expectedValue.ExpectedMortality
                    })
                    .FirstOrDefaultAsync();

            if (expectedValue == null)
            {
                throw new InvalidOperationException(
                    "No existen valores esperados para la semana seleccionada.");
            }

            /*
             * Data Query | Weekly Daily Checks
             * Retrieves all active Daily Checks associated with
             * the selected Brood and week.
             */
            var dailyChecks =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId ==
                            model.BroodId &&
                        dailyCheck.DailyCheckWeek ==
                            model.WeeklyCheckWeek &&
                        dailyCheck.DailyCheckState)
                    .OrderBy(dailyCheck =>
                        dailyCheck.DailyCheckDate)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckDay)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckId)
                    //creates a new object instance for the selected attributes. 
                    .Select(dailyCheck => new
                    {
                        dailyCheck.DailyCheckId,
                        dailyCheck.DailyCheckDay,
                        dailyCheck.AccumulatedMortality,
                        dailyCheck.DailyBirdBalance,
                        dailyCheck.AccumulatedConsumption
                    })
                    .ToListAsync();

            /*
             * Business Validation | Complete Daily Check Week
             * Confirms that exactly seven active Daily Checks exist
             * for the selected Brood and week.
             * It uses .count to confirm existing records and then if different than count "!=", throw erros 
             */
            if (dailyChecks.Count !=
                ValidDailyCheckDays.Length)
            {
                throw new InvalidOperationException(
                    "Debe existir un total de 7 controles diarios activos " +
                    "para generar el control semanal.");
            }

            /*
             * Business Validation | Required Daily Check Days
             * Confirms that Día 1 through Día 7 are each represented
             * exactly once in the selected week.
             */
            var containsAllRequiredDays =
                ValidDailyCheckDays.All(requiredDay =>
                    dailyChecks.Count(dailyCheck =>
                        dailyCheck.DailyCheckDay ==
                            requiredDay) == 1);

            if (!containsAllRequiredDays)
            {
                throw new InvalidOperationException(
                    "Los controles diarios deben incluir exactamente " +
                    "un registro desde Día 1 hasta Día 7.");
            }

            /*
             * Data Selection | Final Daily Check
             * Retrieves Día 7 because its accumulated values represent
             * the final operational status of the selected week.
             */
            var finalDailyCheck =
                dailyChecks.Single(dailyCheck =>
                    dailyCheck.DailyCheckDay ==
                        "Día 7");

            /*
             * Business Validation | Final Bird Balance
             * Confirms that the final active bird population is
             * greater than zero before performing weekly calculations.
             */
            if (finalDailyCheck.DailyBirdBalance <= 0)
            {
                throw new InvalidOperationException(
                    "El saldo final de aves debe ser mayor que cero.");
            }

            /*
             * Business Calculation | Weekly Bird Sample
             * Calculates two percent of the final active bird balance.
             *
             * Decimal.Ceiling ensures that the calculated sample
             * represents at least two percent of the current population.
             */
            var sampleBirdQuantity =
                (int)decimal.Ceiling(
                    finalDailyCheck.DailyBirdBalance *
                    SamplePercentage);

            if (sampleBirdQuantity <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad de aves de la muestra debe ser mayor que cero.");
            }

            /*
             * Business Calculation | Average Weekly Weight
             * Divides the total sample weight by the quantity
             * of birds included in the weekly sample.
             */
            var averageWeeklyWeight =
                model.TotalBirdWeight /
                sampleBirdQuantity;

            averageWeeklyWeight =
                RoundToThreeDecimals(
                    averageWeeklyWeight);

            if (averageWeeklyWeight <= 0)
            {
                throw new InvalidOperationException(
                    "El peso promedio semanal debe ser mayor que cero.");
            }

            /*
             * Business Calculation | Real Weekly Consumption
             * Calculates the accumulated feed consumption per active
             * bird at the end of the selected week.
             */
            var weeklyRealConsumption =
                finalDailyCheck.AccumulatedConsumption /
                finalDailyCheck.DailyBirdBalance;

            weeklyRealConsumption =
                RoundToThreeDecimals(
                    weeklyRealConsumption);

            /*
             * Expected Value | Weekly Consumption
             * Copies the expected consumption associated with the
             * selected week to preserve the historical reference value.
             */
            var weeklyExpectedConsumption =
                expectedValue.ExpectedConsumption;

            /*
             * Business Calculation | Weekly Consumption Difference
             * Calculates the difference between the real weekly
             * consumption and the expected consumption.
             */
            var weeklyConsumptionDifference =
                weeklyRealConsumption -
                weeklyExpectedConsumption;

            weeklyConsumptionDifference =
                RoundToThreeDecimals(
                    weeklyConsumptionDifference);

            /*
             * Expected Value | Weekly Weight
             * Copies the expected weight associated with the
             * selected week to preserve the historical reference value.
             */
            var weeklyExpectedWeight =
                expectedValue.ExpectedWeight;

            /*
             * Business Calculation | Weekly Weight Difference
             * Calculates the difference between the real average
             * weekly weight and the expected weekly weight.
             */
            var weeklyWeightDifference =
                averageWeeklyWeight -
                weeklyExpectedWeight;

            weeklyWeightDifference =
                RoundToThreeDecimals(
                    weeklyWeightDifference);

            /*
             * Business Calculation | Real Weekly Conversion
             * Divides the real accumulated consumption per bird
             * by the real average weekly weight.
             */
            var weeklyRealConversion =
                weeklyRealConsumption /
                averageWeeklyWeight;

            weeklyRealConversion =
                RoundToTwoDecimals(
                    weeklyRealConversion);

            /*
             * Expected Value | Weekly Conversion
             * Copies the expected conversion associated with the
             * selected week to preserve the historical reference value.
             */
            var weeklyExpectedConversion = expectedValue.ExpectedConversion;

            /*
             * Business Calculation | Weekly Conversion Difference
             * Calculates the difference between the real weekly
             * conversion and the expected conversion.
             */
            var weeklyConversionDifference =
                weeklyRealConversion -
                weeklyExpectedConversion;

            weeklyConversionDifference =
                RoundToTwoDecimals(
                    weeklyConversionDifference);

            /*
             * Business Calculation | Real Weekly Mortality
             * Calculates the accumulated mortality percentage using
             * the initial bird quantity of the selected Brood.
             */
            var weeklyRealMortality =
                ((decimal)finalDailyCheck.AccumulatedMortality /
                    brood.BroodBirdInitialNum) *
                100;

            weeklyRealMortality =
                RoundToTwoDecimals(
                    weeklyRealMortality);

            /*
             * Expected Value | Weekly Mortality
             * Copies the expected mortality percentage associated
             * with the selected week to preserve the historical value.
             */
            var weeklyExpectedMortality =
                expectedValue.ExpectedMortality;

            /*
             * Business Calculation | Weekly Mortality Difference
             * Calculates the difference between the real mortality
             * percentage and the expected mortality percentage.
             */
            var weeklyMortalityDifference =
                weeklyRealMortality -
                weeklyExpectedMortality;

            weeklyMortalityDifference =
                RoundToTwoDecimals(
                    weeklyMortalityDifference);

            /*
             * Entity Mapping | Weekly Check
             * Creates the Weekly Check using the validated user-entered
             * values, obtained information and calculated business values.
             */
            var weeklyCheck = new WeeklyCheck
                {
                    SampleBirdQuantity = sampleBirdQuantity,

                    TotalBirdWeight = model.TotalBirdWeight,

                    AverageWeeklyWeight = averageWeeklyWeight,

                    WeeklyRealConsumption = weeklyRealConsumption,

                    WeeklyExpectedConsumption = weeklyExpectedConsumption,

                    WeeklyConsumptionDifference = weeklyConsumptionDifference,

                    WeeklyExpectedWeight = weeklyExpectedWeight,

                    WeeklyWeightDifference = weeklyWeightDifference,

                    WeeklyRealConversion = weeklyRealConversion,

                    WeeklyExpectedConversion = weeklyExpectedConversion,

                    WeeklyConversionDifference = weeklyConversionDifference,

                    WeeklyRealMortality = weeklyRealMortality,

                    WeeklyExpectedMortality =weeklyExpectedMortality,

                    WeeklyMortalityDifference = weeklyMortalityDifference,

                    WeeklyCheckDescription =
                        string.IsNullOrWhiteSpace(
                            model.WeeklyCheckDescription)
                            ? null
                            : model.WeeklyCheckDescription.Trim(),

                    WeeklyCheckState = true,

                    WeeklyCheckWeek = model.WeeklyCheckWeek,

                    BroodId = model.BroodId,

                    ExpectedValueId = expectedValue.ExpectedValueId
                };

            /*
             * Database Operation | Create Weekly Check
             * Adds the validated Weekly Check and saves the new
             * record in the database.
             */
            _context.WeeklyChecks.Add(
                weeklyCheck);

            await _context.SaveChangesAsync();
        }

        /*
         * Data Query | Weekly Check by ID
         * Retrieves a Weekly Check and the related information required
         * by the Details, Edit and Delete operations.
         *
         * The method also retrieves Día 7 of the corresponding Daily Check
         * week because its accumulated values represent the final operational
         * information used by the Weekly Check.
         */
        public async Task<WeeklyCheckGetByIdViewModel?>GetByIdAsync(int weeklyCheckId)
        {
            /*
             * Data Query | Weekly Check
             * Retrieves the Weekly Check and projects the stored values,
             * Brood information and Broiler House information required
             * by the GetById ViewModel.
             */
            var weeklyCheck =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .Where(weeklyCheck =>
                        weeklyCheck.WeeklyCheckId ==
                            weeklyCheckId)
                    .Select(weeklyCheck => new WeeklyCheckGetByIdViewModel
                        {
                            WeeklyCheckId = weeklyCheck.WeeklyCheckId,

                            WeeklyCheckState = weeklyCheck.WeeklyCheckState,

                            BroilerHouseId = weeklyCheck.Brood.BroilerHouseId,

                            BroilerHouseName = weeklyCheck.Brood.BroilerHouse.BroilerHouseName,

                            BroodId = weeklyCheck.BroodId,

                            BroodName = weeklyCheck.Brood.BroodName,

                            BroodDate = weeklyCheck.Brood.BroodDate,

                            BroodBirdInitialNum =weeklyCheck.Brood.BroodBirdInitialNum,

                            WeeklyCheckWeek = weeklyCheck.WeeklyCheckWeek,

                            SampleBirdQuantity = weeklyCheck.SampleBirdQuantity,

                            TotalBirdWeight = weeklyCheck.TotalBirdWeight,

                            AverageWeeklyWeight = weeklyCheck.AverageWeeklyWeight,

                            WeeklyExpectedWeight = weeklyCheck.WeeklyExpectedWeight,

                            WeeklyWeightDifference = weeklyCheck.WeeklyWeightDifference,

                            WeeklyRealConsumption = weeklyCheck.WeeklyRealConsumption,

                            WeeklyExpectedConsumption = weeklyCheck.WeeklyExpectedConsumption,

                            WeeklyConsumptionDifference = weeklyCheck.WeeklyConsumptionDifference,

                            WeeklyRealConversion = weeklyCheck.WeeklyRealConversion,

                            WeeklyExpectedConversion = weeklyCheck.WeeklyExpectedConversion,

                            WeeklyConversionDifference = weeklyCheck.WeeklyConversionDifference,

                            WeeklyRealMortality = weeklyCheck.WeeklyRealMortality,

                            WeeklyExpectedMortality = weeklyCheck.WeeklyExpectedMortality,

                            WeeklyMortalityDifference = weeklyCheck.WeeklyMortalityDifference,

                            WeeklyCheckDescription = weeklyCheck.WeeklyCheckDescription,

                            ExpectedValueId = weeklyCheck.ExpectedValueId
                        })
                    .FirstOrDefaultAsync();

            if (weeklyCheck == null)
            {
                return null;
            }

            /*
             * Data Query | Final Daily Check
             * Retrieves Día 7 from the same Brood and production week.
             *
             * Its accumulated values represent the final mortality,
             * bird balance and accumulated consumption associated
             * with the Weekly Check.
             */
            var finalDailyCheck =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId ==
                            weeklyCheck.BroodId &&
                        dailyCheck.DailyCheckWeek ==
                            weeklyCheck.WeeklyCheckWeek &&
                        dailyCheck.DailyCheckDay ==
                            "Día 7" &&
                        dailyCheck.DailyCheckState)
                    .Select(dailyCheck => new
                    {
                        dailyCheck.AccumulatedMortality,
                        dailyCheck.DailyBirdBalance,
                        dailyCheck.AccumulatedConsumption,
                        dailyCheck.ConcentrateBalance
                    })
                    .FirstOrDefaultAsync();

            /*
             * Related Data | Final Daily Check
             * Copies the final operational information when Día 7
             * is available for the Weekly Check period.
             */
            if (finalDailyCheck != null)
            {
                weeklyCheck.FinalAccumulatedMortality =
                    finalDailyCheck.AccumulatedMortality;

                weeklyCheck.FinalDailyBirdBalance =
                    finalDailyCheck.DailyBirdBalance;

                weeklyCheck.FinalAccumulatedConsumption =
                    finalDailyCheck.AccumulatedConsumption;

                weeklyCheck.FinalConcentrateBalance =
                    finalDailyCheck.ConcentrateBalance;
            }

            /*
             * Data Query | Weekly Daily Checks: Retrieves the seven active Daily Check records associated with the same Brood and production week.
             * These records are displayed in the Details view to provide the operational information used to generate the Weekly Check.
             */
            weeklyCheck.DailyChecks =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId ==
                            weeklyCheck.BroodId &&
                        dailyCheck.DailyCheckWeek ==
                            weeklyCheck.WeeklyCheckWeek &&
                        dailyCheck.DailyCheckState)
                    .OrderBy(dailyCheck =>
                        dailyCheck.DailyCheckDate)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckId)
                    .Select(dailyCheck => new WeeklyCheckDailyCheckViewModel
                        {
                            DailyCheckId = dailyCheck.DailyCheckId,
                            DailyCheckDate = dailyCheck.DailyCheckDate,
                            DailyCheckDay = dailyCheck.DailyCheckDay,
                            DailyCheckWeek = dailyCheck.DailyCheckWeek ?? string.Empty,
                            TotalDailyMortality = dailyCheck.TotalDailyMortality,
                            AccumulatedMortality = dailyCheck.AccumulatedMortality,
                            DailyBirdBalance = dailyCheck.DailyBirdBalance,
                            IncomeAccumulated = dailyCheck.IncomeConcentrate.IncomeAccumulated,
                            ConsumptionKilos = dailyCheck.ConsumptionKilos,
                            AccumulatedConsumption = dailyCheck.AccumulatedConsumption,
                            ConcentrateBalance = dailyCheck.ConcentrateBalance
                        })
                    .ToListAsync();

            return weeklyCheck;
        }

        /*
         * UI Data | Weekly Check Edit Form
         * Retrieves the selected Weekly Check and prepares the
         * WeeklyCheckFormViewModel required by the Edit view.
         *
         * The method reuses GetByIdAsync() to obtain the complete
         * Weekly Check information and then loads the dropdown options
         * required by the form.
         */
        public async Task<WeeklyCheckFormViewModel?>GetFormByIdAsync(int weeklyCheckId)
        {
            /*
             * Data Query | Weekly Check
             * Retrieves the complete Weekly Check information
             * using the shared GetByIdAsync method.
             */
            var weeklyCheck =
                await GetByIdAsync(weeklyCheckId);

            if (weeklyCheck == null)
            {
                return null;
            }

            /*
             * ViewModel Mapping | Edit Form
             * Maps the stored, obtained and calculated Weekly Check
             * information into the form model used by Edit.
             */
            var model =
                new WeeklyCheckFormViewModel
                {
                    WeeklyCheckId =
                        weeklyCheck.WeeklyCheckId,

                    BroilerHouseId =
                        weeklyCheck.BroilerHouseId,

                    BroodId =
                        weeklyCheck.BroodId,

                    WeeklyCheckWeek =
                        weeklyCheck.WeeklyCheckWeek,

                    TotalBirdWeight =
                        weeklyCheck.TotalBirdWeight,

                    WeeklyCheckDescription =
                        weeklyCheck.WeeklyCheckDescription,

                    /*
                     * Operational information.
                     */
                    BroodBirdInitialNum =
                        weeklyCheck.BroodBirdInitialNum,

                    FinalAccumulatedMortality =
                        weeklyCheck.FinalAccumulatedMortality,

                    FinalDailyBirdBalance =
                        weeklyCheck.FinalDailyBirdBalance,

                    FinalAccumulatedConsumption =
                        weeklyCheck.FinalAccumulatedConsumption,
                    FinalConcentrateBalance =
                        weeklyCheck.FinalConcentrateBalance,

                    /*
                     * Expected Values.
                     */
                    WeeklyExpectedConsumption =
                        weeklyCheck.WeeklyExpectedConsumption,

                    WeeklyExpectedWeight =
                        weeklyCheck.WeeklyExpectedWeight,

                    WeeklyExpectedConversion =
                        weeklyCheck.WeeklyExpectedConversion,

                    WeeklyExpectedMortality =
                        weeklyCheck.WeeklyExpectedMortality,

                    /*
                     * Calculated Weekly Check information.
                     */
                    SampleBirdQuantity =
                        weeklyCheck.SampleBirdQuantity,

                    AverageWeeklyWeight =
                        weeklyCheck.AverageWeeklyWeight,

                    WeeklyRealConsumption =
                        weeklyCheck.WeeklyRealConsumption,

                    WeeklyConsumptionDifference =
                        weeklyCheck.WeeklyConsumptionDifference,

                    WeeklyWeightDifference =
                        weeklyCheck.WeeklyWeightDifference,

                    WeeklyRealConversion =
                        weeklyCheck.WeeklyRealConversion,

                    WeeklyConversionDifference =
                        weeklyCheck.WeeklyConversionDifference,

                    WeeklyRealMortality =
                        weeklyCheck.WeeklyRealMortality,

                    WeeklyMortalityDifference =
                        weeklyCheck.WeeklyMortalityDifference,

                    /*
                     * Related Daily Check records displayed
                     * in the Weekly Check form.
                     */
                    DailyChecks =
                        weeklyCheck.DailyChecks,

                    /*
                     * Internal Expected Value identifier.
                     */
                    ExpectedValueId =
                        weeklyCheck.ExpectedValueId
                };

            /*
             * UI Data | Form Options
             * Loads the Broiler House, Brood and week dropdown options
             * while preserving the current selections.
             */
            await PopulateFormOptionsAsync(
                model);

            return model;
        }

        /*
 * UI Data | Reload Weekly Check Edit Form
 * Rebuilds the automatically obtained and calculated
 * information required by the Edit view after a validation
 * or business rule error.
 *
 * User-entered values such as the Weekly Check identifier,
 * total sample weight and description are preserved.
 */
        public async Task<WeeklyCheckFormViewModel>
            ReloadEditFormAsync(
                WeeklyCheckFormViewModel model)
        {
            /*
             * UI Data | Weekly Check Information
             * Attempts to regenerate the operational information,
             * Expected Values, calculations and Daily Check records
             * using the values currently selected by the user.
             */
            var reloadedModel =
                await GetWeeklyCheckInformationAsync(
                    model.BroilerHouseId,
                    model.BroodId,
                    model.WeeklyCheckWeek,
                    model.TotalBirdWeight);

            /*
             * When the selected Brood is no longer available,
             * the submitted model is preserved and only its
             * dropdown options are loaded again.
             */
            if (reloadedModel == null)
            {
                await PopulateFormOptionsAsync(
                    model);

                return model;
            }

            /*
             * User Input | Preserve Edit Information
             * Values entered directly by the user are restored
             * because GetWeeklyCheckInformationAsync generates
             * only operational and calculated information.
             */
            reloadedModel.WeeklyCheckId =
                model.WeeklyCheckId;

            reloadedModel.WeeklyCheckDescription =
                model.WeeklyCheckDescription;

            /*
             * UI Data | Form Options
             * Reloads the dropdown options while preserving
             * the current Broiler House, Brood and week selections.
             */
            await PopulateFormOptionsAsync(
                reloadedModel);

            return reloadedModel;
        }

        /*
         * Business Operation | Update Weekly Check
         * Validates the existing Weekly Check, the submitted form
         * information and the current operational data before updating
         * the record.
         *
         * Obtained and calculated values submitted by the browser are
         * not trusted. They are generated again using the current Brood,
         * Expected Value and Daily Check information before saving.
         */
        public async Task UpdateAsync(WeeklyCheckFormViewModel model)
        {
            /*
             * Business Validation | Weekly Check ID
             * Confirms that the submitted record identifier is valid.
             */
            if (model.WeeklyCheckId <= 0)
            {
                throw new InvalidOperationException(
                    "El identificador del control semanal no es válido.");
            }

            /*
             * Business Validation | Total Bird Weight
             * Confirms that the total weight entered for the
             * weekly bird sample is greater than zero.
             */
            if (model.TotalBirdWeight <= 0)
            {
                throw new InvalidOperationException(
                    "El peso total de la muestra debe ser mayor que cero.");
            }

            /*
             * Data Query | Existing Weekly Check
             * Retrieves the Weekly Check as a tracked entity because
             * its values will be updated and persisted.
             */
            var weeklyCheck =
                await _context.WeeklyChecks
                    .FirstOrDefaultAsync(
                        weeklyCheck =>
                            weeklyCheck.WeeklyCheckId ==
                                model.WeeklyCheckId);

            if (weeklyCheck == null)
            {
                throw new InvalidOperationException(
                    "El control semanal seleccionado no existe.");
            }

            /*
             * Business Validation | Weekly Check State
             * Only active Weekly Checks can be modified.
             */
            if (!weeklyCheck.WeeklyCheckState)
            {
                throw new InvalidOperationException(
                    "El control semanal seleccionado no está disponible para edición.");
            }

            /*
             * Business Validation | Duplicate Weekly Check
             * Prevents another active Weekly Check from using the
             * same Brood and production week.
             *
             * The current record is excluded from the validation.
             */
            var duplicateExists =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .AnyAsync(existingWeeklyCheck =>
                        existingWeeklyCheck.WeeklyCheckId !=
                            model.WeeklyCheckId &&
                        existingWeeklyCheck.BroodId ==
                            model.BroodId &&
                        existingWeeklyCheck.WeeklyCheckWeek ==
                            model.WeeklyCheckWeek &&
                        existingWeeklyCheck.WeeklyCheckState);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "Ya existe otro control semanal activo para la camada y semana seleccionadas.");
            }

            /*
             * Business Calculation | Weekly Check Information
             * Reuses the Weekly Check calculation process to validate
             * the submitted Broiler House, Brood and week and to
             * regenerate all operational and calculated values.
             */
            var calculatedInformation =
                await GetWeeklyCheckInformationAsync(
                    model.BroilerHouseId,
                    model.BroodId,
                    model.WeeklyCheckWeek,
                    model.TotalBirdWeight);

            if (calculatedInformation == null)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no pertenece a la pollera indicada o no está disponible.");
            }

            /*
             * Entity Mapping | Weekly Check Update
             * Updates user-entered values and replaces every obtained
             * or calculated value with the information generated
             * again by the Service layer.
             */
            weeklyCheck.SampleBirdQuantity =
                calculatedInformation.SampleBirdQuantity;

            weeklyCheck.TotalBirdWeight =
                model.TotalBirdWeight;

            weeklyCheck.AverageWeeklyWeight =
                calculatedInformation.AverageWeeklyWeight;

            /*
             * Weekly Consumption.
             */
            weeklyCheck.WeeklyRealConsumption =
                calculatedInformation.WeeklyRealConsumption;

            weeklyCheck.WeeklyExpectedConsumption =
                calculatedInformation.WeeklyExpectedConsumption;

            weeklyCheck.WeeklyConsumptionDifference =
                calculatedInformation.WeeklyConsumptionDifference;

            /*
             * Weekly Weight.
             */
            weeklyCheck.WeeklyExpectedWeight =
                calculatedInformation.WeeklyExpectedWeight;

            weeklyCheck.WeeklyWeightDifference =
                calculatedInformation.WeeklyWeightDifference;

            /*
             * Weekly Conversion.
             */
            weeklyCheck.WeeklyRealConversion =
                calculatedInformation.WeeklyRealConversion;

            weeklyCheck.WeeklyExpectedConversion =
                calculatedInformation.WeeklyExpectedConversion;

            weeklyCheck.WeeklyConversionDifference =
                calculatedInformation.WeeklyConversionDifference;

            /*
             * Weekly Mortality.
             */
            weeklyCheck.WeeklyRealMortality =
                calculatedInformation.WeeklyRealMortality;

            weeklyCheck.WeeklyExpectedMortality =
                calculatedInformation.WeeklyExpectedMortality;

            weeklyCheck.WeeklyMortalityDifference =
                calculatedInformation.WeeklyMortalityDifference;

            /*
             * Editable Weekly Check information.
             */
            weeklyCheck.WeeklyCheckDescription =
                string.IsNullOrWhiteSpace(
                    model.WeeklyCheckDescription)
                    ? null
                    : model.WeeklyCheckDescription.Trim();

            weeklyCheck.WeeklyCheckWeek =
                model.WeeklyCheckWeek;

            weeklyCheck.BroodId =
                model.BroodId;

            weeklyCheck.ExpectedValueId =
                calculatedInformation.ExpectedValueId;

            /*
             * Database Operation | Update Weekly Check
             * Persists the validated and recalculated Weekly Check.
             */
            await _context.SaveChangesAsync();
        }

        /*
         * Business Operation | Soft Delete Weekly Check
         * Logically deactivates an active Weekly Check record.
         *
         * The record remains stored in the database with its state
         * set to false.
         */
        public async Task SoftDeleteAsync(int id)
        {
            /*
             * Business Validation | Existing Weekly Check
             * Confirms that the Weekly Check exists.
             */
            var weeklyCheck =
                await _context.WeeklyChecks
                    .FirstOrDefaultAsync(
                        weeklyCheck =>
                            weeklyCheck.WeeklyCheckId ==
                                id);

            if (weeklyCheck == null)
            {
                throw new InvalidOperationException(
                    "El control semanal seleccionado no existe.");
            }

            /*
             * Business Validation | Weekly Check State
             * Prevents an inactive Weekly Check from being
             * deactivated again.
             */
            if (!weeklyCheck.WeeklyCheckState)
            {
                throw new InvalidOperationException(
                    "El control semanal seleccionado ya se encuentra inactivo.");
            }

            /*
             * Logical Deletion | Weekly Check State
             * Changes the record state to false without removing
             * the Weekly Check physically from the database.
             */
            weeklyCheck.WeeklyCheckState =
                false;

            /*
             * Database Operation | Save Changes
             * Persists the logical deletion.
             */
            await _context.SaveChangesAsync();
        }









    }
}