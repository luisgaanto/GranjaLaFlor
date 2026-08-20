namespace ProjectGranjaLaFlor.Models.ViewModels.Dashboard
{
    /*
     * ViewModel | Dashboard
     *
     * Contains the operational information required by
     * the authenticated user's Dashboard.
     */
    public class DashboardViewModel
    {
        /*
         * Active Broiler Houses and their current
         * production information.
         */
        public IEnumerable<DashboardBroilerHouseViewModel>BroilerHouses { get; set; } = new List<DashboardBroilerHouseViewModel>();
    }
}