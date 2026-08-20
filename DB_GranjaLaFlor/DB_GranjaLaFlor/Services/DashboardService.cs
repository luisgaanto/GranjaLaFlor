using DB_GranjaLaFlor.Data.Context;
using Microsoft.EntityFrameworkCore;
using ProjectGranjaLaFlor.Models.ViewModels.Dashboard;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Service Layer | Dashboard
     *
     * Provides the current operational production information
     * displayed in the authenticated user's Dashboard.
     *
     * Information is calculated independently for each active
     * Broiler House using its most recent active Brood.
     */
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }


        /*
         * UI Data | Dashboard
         *
         * Retrieves all active Broiler Houses and generates
         * their current production information.
         */
        public async Task<DashboardViewModel> GetDashboardAsync()
        {
            /*
             * Database Query | Active Broiler Houses
             */
            var broilerHouses =
                await _context.BroilerHouses
                    .AsNoTracking()
                    .Where(broilerHouse =>
                        broilerHouse.BroilerHouseState)
                    .OrderBy(broilerHouse =>
                        broilerHouse.BroilerHouseId)
                    .Select(broilerHouse => new
                    {
                        broilerHouse.BroilerHouseId,
                        broilerHouse.BroilerHouseName
                    })
                    .ToListAsync();


            var dashboardBroilerHouses =
                new List<DashboardBroilerHouseViewModel>();

            /*
             * Dashboard Information | Broiler House
             *
             * Each Broiler House is processed independently
             * to prevent production information from another
             * Broiler House from being displayed.
             */
            foreach (var broilerHouse in broilerHouses)
            {
                /*
                 * Current Brood
                 *
                 * Retrieves the most recent active Brood
                 * associated specifically with this Broiler House.
                 */
                var currentBrood =
                    await _context.Broods
                        .AsNoTracking()
                        .Where(brood =>
                            brood.BroilerHouseId ==
                                broilerHouse.BroilerHouseId &&
                            brood.BroodState)
                        .OrderByDescending(brood =>
                            brood.BroodDate)
                        .ThenByDescending(brood =>
                            brood.BroodId)
                        .Select(brood => new
                        {
                            brood.BroodId,
                            brood.BroodName
                        })
                        .FirstOrDefaultAsync();


                /*
                 * No Current Brood
                 *
                 * The Broiler House remains visible in the
                 * Dashboard but without production information.
                 */
                if (currentBrood == null)
                {
                    dashboardBroilerHouses.Add(
                        new DashboardBroilerHouseViewModel
                        {
                            BroilerHouseId =
                                broilerHouse.BroilerHouseId,

                            BroilerHouseName =
                                broilerHouse.BroilerHouseName
                        });

                    continue;
                }


                /*
                 * Current Daily Check
                 *
                 * Retrieves the most recent active Daily Check
                 * associated with the current Brood.
                 *
                 * This record provides:
                 * - Current Week
                 * - Current Day
                 * - Current Bird Balance
                 * - Current Concentrate Balance
                 */
                var currentDailyCheck =
                    await _context.DailyChecks
                        .AsNoTracking()
                        .Where(dailyCheck =>
                            dailyCheck.BroodId ==
                                currentBrood.BroodId &&
                            dailyCheck.DailyCheckState)
                        .OrderByDescending(dailyCheck =>
                            dailyCheck.DailyCheckDate)
                        .ThenByDescending(dailyCheck =>
                            dailyCheck.DailyCheckId)
                        .Select(dailyCheck => new
                        {
                            dailyCheck.DailyCheckWeek,
                            dailyCheck.DailyCheckDay,
                            dailyCheck.DailyBirdBalance,
                            dailyCheck.ConcentrateBalance
                        })
                        .FirstOrDefaultAsync();


                /*
                 * ViewModel Mapping | Broiler House
                 */
                dashboardBroilerHouses.Add(new DashboardBroilerHouseViewModel
                    {
                        BroilerHouseId =
                            broilerHouse.BroilerHouseId,

                        BroilerHouseName =
                            broilerHouse.BroilerHouseName,

                        BroodId =
                            currentBrood.BroodId,

                        BroodName =
                            currentBrood.BroodName,

                        CurrentWeek =
                            currentDailyCheck?.DailyCheckWeek,

                        CurrentDay =
                            currentDailyCheck?.DailyCheckDay,

                        CurrentConcentrateBalance = currentDailyCheck?.ConcentrateBalance,

                        CurrentBirdBalance =
                            currentDailyCheck?.DailyBirdBalance,
                    
                });
            }


            /*
             * ViewModel | Dashboard
             */
            return new DashboardViewModel
            {
                BroilerHouses =
                    dashboardBroilerHouses
            };
        }
    }
}