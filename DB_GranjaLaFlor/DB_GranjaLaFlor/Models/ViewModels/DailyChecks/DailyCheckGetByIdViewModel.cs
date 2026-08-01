using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels
{
    /*
     * Represents the complete Daily Check information
     * required by the Details, Delete and Activate views.
     *
     * Related entity names are included to prevent the views
     * from depending directly on navigation properties.
     */
    public class DailyCheckGetByIdViewModel
    {
        /*
         * Internal identifier used by the Details, Delete,
         * Activate and Edit actions.
         */
        [Display(Name = "ID")]
        public int DailyCheckId { get; set; }

        /*
         * Daily Check information.
         */
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime DailyCheckDate { get; set; }

        [Display(Name = "Semana")]
        public string DailyCheckWeek { get; set; } = string.Empty;

        [Display(Name = "Día")]
        public string DailyCheckDay { get; set; } = string.Empty;

        /*
         * Related Broiler House and Brood information.
         */
        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;

        [Display(Name = "Camada")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Aves iniciales")]
        public int BroodBirdInitialNum { get; set; }

        /*
         * This value is obtained from the associated
         * Income Concentrate record. It is not stored in DailyCheck.
         */
        [Display(Name = "Concentrado acumulado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal IncomeAccumulated { get; set; }

        /*
         * User-entered values.
         */
        [Display(Name = "Mortalidad natural")]
        public int NaturalMortality { get; set; }

        [Display(Name = "Selección")]
        public int SelectQuantity { get; set; }

        [Display(Name = "Consumo en quintales")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionQuintals { get; set; }

        /*
         * Calculated values stored in DailyCheck.
         */
        [Display(Name = "Consumo en kilos")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionKilos { get; set; }

        [Display(Name = "Mortalidad diaria total")]
        public int TotalDailyMortality { get; set; }

        [Display(Name = "Mortalidad acumulada")]
        public int AccumulatedMortality { get; set; }

        [Display(Name = "Saldo de aves")]
        public int DailyBirdBalance { get; set; }

        [Display(Name = "Consumo acumulado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal AccumulatedConsumption { get; set; }

        [Display(Name = "Saldo de concentrado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConcentrateBalance { get; set; }

        /*
         * Additional Daily Check information.
         */
        [Display(Name = "Descripción")]
        public string? DailyCheckDescription { get; set; }

        [Display(Name = "Estado")]
        public bool DailyCheckState { get; set; }
    }
}