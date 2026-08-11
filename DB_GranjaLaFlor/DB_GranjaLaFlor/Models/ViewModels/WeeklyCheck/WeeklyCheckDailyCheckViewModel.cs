using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck
{
    /*
     * Architecture Decision | Weekly Daily Check ViewModel
     * Represents each active Daily Check used to generate a Weekly Check.
     *
     * This ViewModel is required because the Weekly Check form must display
     * the seven Daily Check records associated with the selected Brood and week.
     *
     * It contains only the Daily Check information required by the Weekly Check
     * process and prevents the Razor view from depending directly on the entity.
     */
    public class WeeklyCheckDailyCheckViewModel
    {
        /*
         * Internal identifier of the Daily Check record.
         *
         * The identifier is used internally by the application
         * and does not need to be displayed in the Weekly Check form.
         */
        public int DailyCheckId { get; set; }

        /*
         * Date and day information used to identify each
         * Daily Check included in the selected week.
         */
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime DailyCheckDate { get; set; }

        [Display(Name = "Día")]
        public string DailyCheckDay { get; set; } = string.Empty;

        /*
         * Week associated with the Daily Check.
         *
         * This value is displayed in the Weekly Check form to identify
         * the production week associated with each Daily Check record.
         */
        [Display(Name = "Semana")]
        public string DailyCheckWeek { get; set; } = string.Empty;

        /*
         * Accumulated Income Concentrate associated with the Daily Check.
         *
         * The value comes from the Income Concentrate record referenced
         * by the Daily Check and is displayed for operational reference.
         */
        [Display(Name = "Ingreso Concentrado Acumulado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal IncomeAccumulated { get; set; }

        /*
         * Mortality information obtained from the Daily Check.
         *
         * The accumulated mortality from the final Daily Check
         * is used to calculate the weekly mortality percentage.
         */
        [Display(Name = "Mortalidad diaria total")]
        public int TotalDailyMortality { get; set; }

        [Display(Name = "Mortalidad acumulada")]
        public int AccumulatedMortality { get; set; }

        /*
         * Current bird balance obtained from the Daily Check.
         *
         * The final balance of the selected week is used to calculate
         * the quantity of birds included in the two percent sample.
         */
        [Display(Name = "Saldo de aves")]
        public int DailyBirdBalance { get; set; }

        /*
         * Consumption information obtained from the Daily Check.
         *
         * The accumulated consumption from the final Daily Check
         * is used to calculate the real weekly consumption per bird.
         */
        [Display(Name = "Consumo en kilos")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionKilos { get; set; }

        [Display(Name = "Consumo acumulado")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal AccumulatedConsumption { get; set; }

        /*
         * Concentrate balance obtained from the Daily Check.
         *
         * This value represents the remaining concentrate after
         * subtracting the accumulated consumption from the
         * accumulated concentrate available.
         */
        [Display(Name = "Saldo Concentrado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConcentrateBalance { get; set; }
    }
}