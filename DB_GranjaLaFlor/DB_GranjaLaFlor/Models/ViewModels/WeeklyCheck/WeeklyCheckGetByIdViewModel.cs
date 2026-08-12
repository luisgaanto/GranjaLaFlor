using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck
{
    /*
     * Represents the complete Weekly Check information
     * required by the Details, Edit and Delete operations.
     *
     * Related entity information, copied Expected Values and
     * final Daily Check values are included so the views and
     * subsequent operations do not depend directly on entities
     * or navigation properties.
     */
    public class WeeklyCheckGetByIdViewModel
    {
        /*
         * Weekly Check identification.
         */
        [Display(Name = "ID")]
        public int WeeklyCheckId { get; set; }

        [Display(Name = "Estado")]
        public bool WeeklyCheckState { get; set; }

        /*
         * General information associated with the
         * Broiler House, Brood and production week.
         */
        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; }
            = string.Empty;

        [Display(Name = "Camada")]
        public string BroodName { get; set; }
            = string.Empty;

        [Display(Name = "Fecha Camada")]
        [DataType(DataType.Date)]
        public DateTime BroodDate { get; set; }

        [Display(Name = "Cantidad Inicial Aves")]
        public int BroodBirdInitialNum { get; set; }

        [Display(Name = "Semana")]
        public string WeeklyCheckWeek { get; set; }
            = string.Empty;

        /*
         * Final operational information obtained from the
         * Día 7 Daily Check associated with the same Brood
         * and production week.
         */
        [Display(Name = "Mortalidad Acumulada Final")]
        public int FinalAccumulatedMortality { get; set; }

        [Display(Name = "Saldo Actual Aves")]
        public int FinalDailyBirdBalance { get; set; }

        [Display(Name = "Consumo Acumulado Final (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal FinalAccumulatedConsumption { get; set; }

        /*
         * Weekly bird sample and weight information.
         */
        [Display(Name = "Cantidad Aves Muestra")]
        public int SampleBirdQuantity { get; set; }

        [Display(Name = "Peso Total Muestra (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalBirdWeight { get; set; }

        [Display(Name = "Peso Promedio Semanal (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal AverageWeeklyWeight { get; set; }

        [Display(Name = "Peso Esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedWeight { get; set; }

        [Display(Name = "Diferencia Peso (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyWeightDifference { get; set; }

        /*
         * Weekly consumption information.
         */
        [Display(Name = "Consumo Real (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyRealConsumption { get; set; }

        [Display(Name = "Consumo Esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedConsumption { get; set; }

        [Display(Name = "Diferencia Consumo (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyConsumptionDifference { get; set; }

        /*
         * Weekly conversion information.
         */
        [Display(Name = "Conversión Real")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealConversion { get; set; }

        [Display(Name = "Conversión Esperada")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedConversion { get; set; }

        [Display(Name = "Diferencia Conversión")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyConversionDifference { get; set; }

        /*
         * Weekly mortality information.
         */
        [Display(Name = "Mortalidad Real (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealMortality { get; set; }

        [Display(Name = "Mortalidad Esperada (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedMortality { get; set; }

        [Display(Name = "Diferencia Mortalidad (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyMortalityDifference { get; set; }

        /*
         * Optional Weekly Check description.
         */
        [Display(Name = "Descripción")]
        public string? WeeklyCheckDescription { get; set; }

        /*
         * Internal relationship identifiers.
         *
         * These values are not required for display but are useful
         * when the retrieved record is later used by Edit or other
         * application operations.
         */
        public int BroilerHouseId { get; set; }

        public int BroodId { get; set; }

        public int ExpectedValueId { get; set; }

        /*
         * Daily Check records associated with the Weekly Check.
         *
         * These records are displayed in the Details view to show
         * the seven Daily Checks used to generate the Weekly Check.
         */
        public IEnumerable<WeeklyCheckDailyCheckViewModel> DailyChecks
        {
            get;
            set;
        } = new List<WeeklyCheckDailyCheckViewModel>();

    }
}