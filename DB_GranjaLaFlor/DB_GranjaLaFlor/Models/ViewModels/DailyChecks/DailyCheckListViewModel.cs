using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels
{
    /*
     * Represents the Daily Check information displayed
     * in the active and inactive record lists.
     *
     * The ViewModel contains only the properties required
     * by the list views and does not expose the complete entity.
     */
    public class DailyCheckListViewModel
    {
        /*
         * Internal identifier used by the Details, Edit
         * and Delete actions. It is not displayed in Index.
         */
        public int DailyCheckId { get; set; }

        /*
         * Daily Check information.
         */
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime DailyCheckDate { get; set; }

        [Display(Name = "Semana")]
        public string DailyCheckWeek { get; set; } = string.Empty;

        [Display(Name = "Día")]
        public string DailyCheckDay { get; set; } = string.Empty;

        /*
         * Broiler House and Brood information obtained
         * through the Daily Check relationships.
         */
        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;

        /*
         * Internal Brood identifier used by filters
         * and application operations. It is not displayed.
         */
        public int BroodId { get; set; }

        [Display(Name = "Camada")]
        public string BroodName { get; set; } = string.Empty;

        /*
         * Used internally by the year filter.
         * It is not displayed as an Index column.
         */
        public int BroodYear { get; set; }

        [Display(Name = "Aves Iniciales")]
        public int BroodBirdInitialNum { get; set; }

        /*
         * Internal identifier of the Income Concentrate record
         * associated with the Daily Check. It is not displayed.
         */
        public int IncomeConcentrateId { get; set; }

        /*
         * Accumulated concentrate obtained from the Income Concentrate
         * record associated with the Daily Check.
         *
         * The value is displayed as a reference but is not stored
         * directly in the daily_checks table.
         */
        [Display(Name = "Concentrado Acumulado")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal IncomeAccumulated { get; set; }

        /*
         * User-entered values.
         */
        [Display(Name = "Mortalidad Natural")]
        public int NaturalMortality { get; set; }

        [Display(Name = "Selección")]
        public int SelectQuantity { get; set; }

        [Display(Name = "Consumo Quintales")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionQuintals { get; set; }

        /*
         * Calculated values stored in DailyCheck.
         */
        [Display(Name = "Consumo Kilos")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionKilos { get; set; }

        [Display(Name = "Mortalidad Diaria Total")]
        public int TotalDailyMortality { get; set; }

        [Display(Name = "Mortalidad Acumulada")]
        public int AccumulatedMortality { get; set; }

        [Display(Name = "Saldo Aves")]
        public int DailyBirdBalance { get; set; }

        [Display(Name = "Consumo Acumulado")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal AccumulatedConsumption { get; set; }

        [Display(Name = "Saldo Concentrado")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConcentrateBalance { get; set; }

        /*
         * Used to identify active and inactive records.
         * The active Index does not display this property.
         */
        [Display(Name = "Estado")]
        public bool DailyCheckState { get; set; }
    }
}