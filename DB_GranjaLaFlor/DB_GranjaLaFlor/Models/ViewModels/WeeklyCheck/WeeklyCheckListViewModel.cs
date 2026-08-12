using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck
{
    /*
     * Represents the Weekly Check information displayed
     * in the active record list.
     *
     * The ViewModel contains only the properties required
     * by the Index view and its available actions.
     */
    public class WeeklyCheckListViewModel
    {
        /*
         * Internal identifier used by the Details, Edit
         * and Delete actions. It is not displayed in Index.
         */
        public int WeeklyCheckId { get; set; }

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;

        public int BroodId { get; set; }

        [Display(Name = "Camada")]
        public string BroodName { get; set; } = string.Empty;

        /*
         * Used internally by the year filter.
         */
        [Display(Name = "Año ")]
        public int BroodYear { get; set; }

        [Display(Name = "Semana")]
        public string WeeklyCheckWeek { get; set; } = string.Empty;

        [Display(Name = "Peso Promedio (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal AverageWeeklyWeight { get; set; }

        [Display(Name = "Consumo Real (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyRealConsumption { get; set; }

        [Display(Name = "Conversión Real")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealConversion { get; set; }

        [Display(Name = "Mortalidad Real (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealMortality { get; set; }

        [Display(Name = "Estado")]
        public bool WeeklyCheckState { get; set; }

        /*
         * Expected Values | Weekly Check
         * Stores the expected production values copied into the
         * Weekly Check when the record was created.
         *
         * These values are used by the Index view to compare
         * expected and real weekly results.
         */
        [Display(Name = "Peso esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedWeight { get; set; }

        [Display(Name = "Consumo esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedConsumption { get; set; }

        [Display(Name = "Conversión esperada")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedConversion { get; set; }

        [Display(Name = "Mortalidad esperada (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedMortality { get; set; }
    }
}