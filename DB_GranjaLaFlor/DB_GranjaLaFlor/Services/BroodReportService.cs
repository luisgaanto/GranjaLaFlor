using DB_GranjaLaFlor.Data.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectGranjaLaFlor.Models;
using ProjectGranjaLaFlor.Models.ViewModels.BroodReport;
using System.Text.Json;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Service Layer | Brood Report
     *
     * Contains the business logic and data access required
     * to generate and retrieve historical Brood Reports.
     *
     * Controllers coordinate HTTP requests only while this
     * Service validates the selected Brood, obtains the related
     * operational information and builds the historical snapshot.
     */
    public class BroodReportService
    {
        private readonly ApplicationDbContext _context;

        /*
         * Business Constant | Quintal Conversion
         * One quintal is equivalent to forty-six kilograms.
         */
        private const decimal KilosPerQuintal = 46m;

        public BroodReportService(ApplicationDbContext context)
        {
            _context = context;
        }


        /*
         * UI Data | Create Brood Report
         *
         * Creates the ViewModel required by the Generate/Create
         * view and loads the initial dropdown options.
         */
        public async Task<BroodReportFormViewModel>GetCreateViewModelAsync()
        {
            var model = new BroodReportFormViewModel();

            await PopulateFormOptionsAsync(model);

            return model;
        }


        /*
         * UI Data | Populate Brood Report Form Options
         *
         * Loads the active Broiler House options and, when a
         * Broiler House is already selected, loads its active
         * Broods.
         */
        public async Task PopulateFormOptionsAsync(BroodReportFormViewModel model)
        {
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
                                broilerHouse.BroilerHouseName
                        })
                    .ToListAsync();

            if (model.BroilerHouseId > 0)
            {
                model.BroodOptions =
                    await GetBroodsByBroilerHouseAsync(
                        model.BroilerHouseId);
            }
            else
            {
                model.BroodOptions =
                    new List<SelectListItem>();
            }
        }


        /*
         * UI Data | Broods by Broiler House
         *
         * Retrieves the active Broods associated with the
         * selected active Broiler House.
         */
        public async Task<IEnumerable<SelectListItem>>GetBroodsByBroilerHouseAsync(int broilerHouseId)
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
                    brood.BroilerHouse.BroilerHouseState)
                .OrderByDescending(brood =>
                    brood.BroodDate)
                .ThenBy(brood =>
                    brood.BroodName)
                .Select(brood =>
                    new SelectListItem
                    {
                        Value =
                            brood.BroodId.ToString(),

                        Text =
                            brood.BroodName +
                            " - Año " +
                            brood.BroodDate.Year
                    })
                .ToListAsync();
        }


        /*
         * Business Query | Brood Report Information
         *
         * Validates the selected Broiler House and Brood and retrieves
         * the operational information required to build the historical
         * Brood Report snapshot.
         *
         * The method does not persist any Brood Report record.
         * It only prepares and validates the information required
         * before generation.
         */
        public async Task<BroodReportSnapshotViewModel?>GetBroodReportInformationAsync(int broilerHouseId,int broodId,int reportNumber)
        {
            /*
             * Request Validation | Identifiers
             */
            if (broilerHouseId <= 0)
            {
                throw new InvalidOperationException(
                    "Debe seleccionar una pollera válida.");
            }

            if (broodId <= 0)
            {
                throw new InvalidOperationException(
                    "Debe seleccionar una camada válida.");
            }

            if (reportNumber <= 0)
            {
                throw new InvalidOperationException(
                    "El número del reporte debe ser mayor que cero.");
            }


            /*
             * Business Validation | Brood
             *
             * Retrieves the active Brood and verifies that it belongs
             * to the selected active Broiler House.
             */
            var brood =
                await _context.Broods
                    .AsNoTracking()
                    .Where(brood =>
                        brood.BroodId == broodId &&
                        brood.BroodState &&
                        brood.BroilerHouseId == broilerHouseId &&
                        brood.BroilerHouse != null &&
                        brood.BroilerHouse.BroilerHouseState)
                    .Select(brood => new
                    {
                        brood.BroodId,
                        brood.BroodName,
                        brood.BroodDate,
                        brood.BroodBirdInitialNum,
                        brood.BroilerHouseId,

                        BroilerHouseName =
                            brood.BroilerHouse!
                                .BroilerHouseName
                    })
                    .FirstOrDefaultAsync();

            if (brood == null)
            {
                return null;
            }


            /*
             * Data Query | Daily Checks
             *
             * Retrieves the active Daily Checks associated with the Brood.
             * These records provide the operational information displayed
             * in the Control de Aves and Control de Alimento sections.
             */
            var dailyChecks =
                await _context.DailyChecks
                    .AsNoTracking()
                    .Where(dailyCheck =>
                        dailyCheck.BroodId == broodId &&
                        dailyCheck.DailyCheckState)
                    .OrderBy(dailyCheck =>
                        dailyCheck.DailyCheckDate)
                    .ThenBy(dailyCheck =>
                        dailyCheck.DailyCheckId)
                    .Select(dailyCheck => new
                    {
                        dailyCheck.DailyCheckId,
                        dailyCheck.DailyCheckDate,
                        dailyCheck.DailyCheckWeek,
                        dailyCheck.DailyCheckDay,
                        dailyCheck.NaturalMortality,
                        dailyCheck.SelectQuantity,
                        dailyCheck.AccumulatedMortality,
                        dailyCheck.DailyBirdBalance,
                        dailyCheck.ConsumptionQuintals,
                        dailyCheck.AccumulatedConsumption,
                        dailyCheck.ConcentrateBalance
                    })
                    .ToListAsync();


            /*
             * Business Validation | Daily Checks
             *
             * Six complete Weekly Checks normally represent
             * forty-two Daily Check records.
             */
            if (dailyChecks.Count != 42)
            {
                throw new InvalidOperationException(
                    "La camada debe contener exactamente 42 controles diarios activos " +
                    "correspondientes a las 6 semanas antes de generar el reporte.");
            }


            /*
             * Data Query | Weekly Checks
             *
             * Retrieves the six active Weekly Checks required by
             * the Control Semanal section.
             */
            var weeklyChecks =
                await _context.WeeklyChecks
                    .AsNoTracking()
                    .Where(weeklyCheck =>
                        weeklyCheck.BroodId == broodId &&
                        weeklyCheck.WeeklyCheckState)
                    .OrderBy(weeklyCheck =>
                        weeklyCheck.WeeklyCheckWeek)
                    .Select(weeklyCheck => new BroodReportWeeklyViewModel
                        {
                            WeeklyCheckId =
                                weeklyCheck.WeeklyCheckId,

                            Week =
                                weeklyCheck.WeeklyCheckWeek,

                            ExpectedConsumption =
                                weeklyCheck.WeeklyExpectedConsumption,

                            RealConsumption =
                                weeklyCheck.WeeklyRealConsumption,

                            ConsumptionDifference =
                                weeklyCheck.WeeklyConsumptionDifference,

                            ExpectedWeight =
                                weeklyCheck.WeeklyExpectedWeight,

                            RealWeight =
                                weeklyCheck.AverageWeeklyWeight,

                            WeightDifference =
                                weeklyCheck.WeeklyWeightDifference,

                            ExpectedConversion =
                                weeklyCheck.WeeklyExpectedConversion,

                            RealConversion =
                                weeklyCheck.WeeklyRealConversion,

                            ConversionDifference =
                                weeklyCheck.WeeklyConversionDifference,

                            ExpectedMortality =
                                weeklyCheck.WeeklyExpectedMortality,

                            RealMortality =
                                weeklyCheck.WeeklyRealMortality,

                            MortalityDifference =
                                weeklyCheck.WeeklyMortalityDifference
                        })
                    .ToListAsync();


            /*
             * Business Validation | Weekly Checks
             */
            if (weeklyChecks.Count != 6)
            {
                throw new InvalidOperationException(
                    "La camada debe contener exactamente 6 controles semanales activos " +
                    "antes de generar el reporte.");
            }


            /*
             * Data Query | Income Concentrates
             *
             * Retrieves the active concentrate income records associated
             * with the Brood. These values are used to determine the
             * concentrate received on each report date.
             */
            var incomeConcentrates =
                await _context.IncomeConcentrates
                    .AsNoTracking()
                    .Where(income =>
                        income.BroodId == broodId &&
                        income.IncomeState)
                    .OrderBy(income =>
                        income.IncomeConcentrateDate)
                    .ThenBy(income =>
                        income.IncomeConcentrateId)
                    .Select(income => new
                    {
                        income.IncomeConcentrateId,
                        income.IncomeConcentrateDate,
                        income.IncomeQuintals,
                        income.IncomeAccumulated
                    })
                    .ToListAsync();


            /*
             * Business Validation | Income Concentrate
             */
            if (!incomeConcentrates.Any())
            {
                throw new InvalidOperationException(
                    "La camada no tiene ingresos de concentrado activos asociados.");
            }


            /*
             * Snapshot | Header
             *
             * The report date corresponds to the first Daily Check
             * included in the generated report.
             */
            var header =
                new BroodReportHeaderViewModel
                {
                    ReportNumber =
                        reportNumber,

                    Date =
                        dailyChecks.First()
                            .DailyCheckDate,

                    FarmName =
                        "La Flor",

                    BirdQuantity =
                        brood.BroodBirdInitialNum,

                    BroilerHouseName =
                        brood.BroilerHouseName
                };


            /*
             * Snapshot | Daily Rows
             *
             * Builds the operational rows required by the printed report.
             */
            var dailyRows =
                dailyChecks
                    .Select(
                        (dailyCheck, index) =>
                        {
                            /*
                             * Concentrate received on the current
                             * Daily Check date.
                             */
                            var dailyIncome =
                                incomeConcentrates
                                    .Where(income =>
                                        income.IncomeConcentrateDate.Date ==
                                        dailyCheck.DailyCheckDate.Date)
                                    .Sum(income =>
                                        income.IncomeQuintals);

                            /*
                             * Accumulated Concentrate | Latest Income
                             *
                             * Retrieves the latest active Income Concentrate available
                             * through the current Daily Check date.
                             *
                             * IncomeAccumulated is stored in kilograms and is converted
                             * to quintals when the Brood Report snapshot is generated.
                             */
                            var latestIncome =
                                incomeConcentrates
                                    .Where(income =>
                                        income.IncomeConcentrateDate.Date <=
                                        dailyCheck.DailyCheckDate.Date)
                                    .OrderByDescending(income =>
                                        income.IncomeConcentrateDate)
                                    .ThenByDescending(income =>
                                        income.IncomeConcentrateId)
                                    .FirstOrDefault();

                            return new BroodReportDailyRowViewModel
                            {
                                DayNumber =
                                    index + 1,

                                Date =
                                    dailyCheck.DailyCheckDate,

                                NaturalMortality =
                                    dailyCheck.NaturalMortality,

                                SelectQuantity =
                                    dailyCheck.SelectQuantity,

                                AccumulatedMortality =
                                    dailyCheck.AccumulatedMortality,

                                BirdBalance =
                                    dailyCheck.DailyBirdBalance,

                                IncomeDailyQuintals =
                                    dailyIncome > 0
                                        ? dailyIncome
                                        : null,

                                IncomeAccumulatedQuintals =
                                    latestIncome != null
                                        ? latestIncome.IncomeAccumulated /
                                            KilosPerQuintal
                                        : null,

                                ConsumptionDailyQuintals =
                                    dailyCheck.ConsumptionQuintals,

                                ConsumptionAccumulatedQuintals =
                                    dailyCheck.AccumulatedConsumption /
                                    KilosPerQuintal,

                                ConcentrateBalanceQuintals =
                                    dailyCheck.ConcentrateBalance /
                                    KilosPerQuintal
                            };
                        })
                    .ToList();


            /*
             * Historical Snapshot | Brood Report
             *
             * Combines the header, daily operational rows and
             * six Weekly Check summaries into one historical
             * representation of the report.
             */
            return new BroodReportSnapshotViewModel
            {
                Header =
                    header,

                DailyRows =
                    dailyRows,

                WeeklyChecks =
                    weeklyChecks
            };
        }

        /*
         * Business Operation | Create Brood Report
         *
         * Generates a historical Brood Report from the current
         * operational information of the selected Brood.
         *
         * The generated snapshot is serialized and stored so the
         * report remains unchanged even if operational records are
         * modified later.
         */
        public async Task<int> CreateAsync(BroodReportFormViewModel model)
        {
            /*
             * Business Validation | Report Information
             *
             * Validates the selected Broiler House, Brood and report
             * number and builds the historical report snapshot.
             */
            var snapshot =
                await GetBroodReportInformationAsync(
                    model.BroilerHouseId,
                    model.BroodId,
                    model.ReportNumber);

            if (snapshot == null)
            {
                throw new InvalidOperationException(
                    "La camada seleccionada no pertenece a la pollera indicada o no está disponible.");
            }

            /*
             * Historical Version | Brood Report
             *
             * Determines the next report version for the selected Brood.
             *
             * Each Brood keeps its own independent version sequence.
             */
            var currentVersion =
                await _context.BroodReports
                    .AsNoTracking()
                    .Where(broodReport =>
                        broodReport.BroodId ==
                            model.BroodId)
                    .Select(broodReport =>
                        (int?)broodReport.BroodReportVersion)
                    .MaxAsync();

            var nextVersion =
                (currentVersion ?? 0) + 1;

            /*
             * Serialization | Historical Snapshot
             *
             * Converts the strongly typed Brood Report snapshot
             * into JSON for persistent historical storage.
             */
            var reportData =
                JsonSerializer.Serialize(
                    snapshot);

            /*
             * Entity Mapping | Brood Report
             *
             * Creates the persistent report record.
             */
            var broodReport =
                new BroodReport
                {
                    ReportNumber =
                        model.ReportNumber,

                    BroodReportGeneratedAt =
                        DateTime.Now,

                    BroodReportVersion =
                        nextVersion,

                    BroodReportData =
                        reportData,

                    BroodId =
                        model.BroodId
                };

            /*
             * Database Operation | Create Brood Report
             */
            _context.BroodReports.Add(
                broodReport);

            await _context.SaveChangesAsync();

            /*
             * Returns the generated identifier so the Controller
             * can redirect directly to Details.
             */
            return broodReport.BroodReportId;
        }

        /*
         * Data Query | Brood Reports
         *
         * Retrieves the generated Brood Reports required by Index.
         *
         * The query includes the related Brood and Broiler House
         * information but does not deserialize BroodReportData because
         * the historical snapshot is not required by the list view.
         */
        public async Task<IEnumerable<BroodReportListViewModel>>GetAllAsync()
        {
            return await _context.BroodReports
                .AsNoTracking()
                .OrderByDescending(broodReport =>
                    broodReport.BroodReportGeneratedAt)
                .ThenByDescending(broodReport =>
                    broodReport.BroodReportId)
                .Select(broodReport => new BroodReportListViewModel
                    {
                        BroodReportId =
                            broodReport.BroodReportId,

                        ReportNumber =
                            broodReport.ReportNumber,

                        BroilerHouseName =
                            broodReport.Brood
                                .BroilerHouse!
                                .BroilerHouseName,

                        BroodId =
                            broodReport.BroodId,

                        BroodName =
                            broodReport.Brood
                                .BroodName,

                        BroodYear =
                            broodReport.Brood
                                .BroodDate.Year,

                        BroodReportGeneratedAt =
                            broodReport.BroodReportGeneratedAt,

                        BroodReportVersion =
                            broodReport.BroodReportVersion
                    })
                .ToListAsync();
        }

        /*
         * Data Query | Brood Report by ID
         *
         * Retrieves the selected Brood Report together with the
         * related Brood and Broiler House information required
         * by Details and future PDF generation.
         *
         * The historical JSON snapshot stored in BroodReportData
         * is deserialized into BroodReportSnapshotViewModel.
         */
                public async Task<BroodReportGetByIdViewModel?>GetByIdAsync(int broodReportId)
        {
            /*
             * Request Validation | Brood Report ID
             */
            if (broodReportId <= 0)
            {
                return null;
            }

            /*
             * Data Query | Brood Report
             *
             * Retrieves the stored report metadata and the
             * serialized historical snapshot.
             */
            var broodReport =
                await _context.BroodReports
                    .AsNoTracking()
                    .Where(broodReport =>
                        broodReport.BroodReportId ==
                            broodReportId)
                    .Select(broodReport => new
                    {
                        broodReport.BroodReportId,
                        broodReport.ReportNumber,
                        broodReport.BroodReportGeneratedAt,
                        broodReport.BroodReportVersion,
                        broodReport.BroodReportData,
                        broodReport.BroodId,

                        BroodName =
                            broodReport.Brood
                                .BroodName,

                        BroodDate =
                            broodReport.Brood
                                .BroodDate,

                        BroilerHouseName =
                            broodReport.Brood
                                .BroilerHouse!
                                .BroilerHouseName
                    })
                    .FirstOrDefaultAsync();

            if (broodReport == null)
            {
                return null;
            }

            /*
             * Historical Snapshot | Deserialize
             *
             * Converts the stored JSON back into the strongly typed
             * BroodReportSnapshotViewModel used by Details and PDF.
             */
            BroodReportSnapshotViewModel? snapshot;

            try
            {
                snapshot =
                    JsonSerializer
                        .Deserialize<BroodReportSnapshotViewModel>(
                            broodReport.BroodReportData);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "No se pudo recuperar la información histórica " +
                    "del reporte de camada.",
                    ex);
            }

            if (snapshot == null)
            {
                throw new InvalidOperationException(
                    "La información histórica del reporte de camada " +
                    "no está disponible.");
            }

            /*
             * ViewModel Mapping | Brood Report Details
             */
            return new BroodReportGetByIdViewModel
            {
                BroodReportId =
                    broodReport.BroodReportId,

                ReportNumber =
                    broodReport.ReportNumber,

                BroodReportGeneratedAt =
                    broodReport.BroodReportGeneratedAt,

                BroodReportVersion =
                    broodReport.BroodReportVersion,

                BroodId =
                    broodReport.BroodId,

                BroodName =
                    broodReport.BroodName,

                BroilerHouseName =
                    broodReport.BroilerHouseName,

                BroodDate =
                    broodReport.BroodDate,

                Snapshot =
                    snapshot
            };
        }




    }
}