using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.Dashboard
{
    /*
     * ViewModel | Dashboard Broiler House
     *
     * Represents the current operational information displayed
     * for one active Broiler House in the Dashboard.
     *
     * The values are obtained from the most recent active Brood
     * associated with the corresponding Broiler House.
     */
    public class DashboardBroilerHouseViewModel
    {
        /*
         * Broiler House Information
         */
        public int BroilerHouseId { get; set; }

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;

        /*
         * Current Brood Information
         */
        public int? BroodId { get; set; }

        [Display(Name = "Camada")]
        public string? BroodName { get; set; }


        /*
         * Current Operational Information
         *
         * Week, Day and Bird Balance are obtained from the
         * most recent active Daily Check of the current Brood.
         */
        [Display(Name = "Semana")]
        public string? CurrentWeek { get; set; }

        [Display(Name = "Día")]
        public string? CurrentDay { get; set; }

        [Display(Name = "Saldo de aves")]
        public int? CurrentBirdBalance { get; set; }

        /*
         * Current Concentrate Balance
         *
         * Represents the remaining concentrate available
         * in the most recent active Daily Check of the
         * current Brood.
         */
        [Display(Name = "Saldo Concentrado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? CurrentConcentrateBalance { get; set; }
    }
}