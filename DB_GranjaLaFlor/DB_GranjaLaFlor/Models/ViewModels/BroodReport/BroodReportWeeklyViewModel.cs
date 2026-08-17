using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Represents one Weekly Check displayed in the
     * Control Semanal section of the Brood Report.
     *
     * Expected, real and difference values are copied from
     * the Weekly Check and stored in the historical snapshot.
     */
    public class BroodReportWeeklyViewModel
    {
        /*
         * Internal identifier retained for traceability.
         * It is not displayed in the printed report.
         */
        public int WeeklyCheckId { get; set; }

        [Display(Name = "Semana")]
        public string Week { get; set; }
            = string.Empty;

        /*
         * Control Semanal | Cons.
         */
        [Display(Name = "Esp.")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal ExpectedConsumption { get; set; }

        [Display(Name = "Real")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal RealConsumption { get; set; }

        [Display(Name = "Diferencia")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal ConsumptionDifference { get; set; }

        /*
         * Control Semanal | Peso
         */
        [Display(Name = "Esp.")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal ExpectedWeight { get; set; }

        [Display(Name = "Real")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal RealWeight { get; set; }

        [Display(Name = "Diferencia")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeightDifference { get; set; }

        /*
         * Control Semanal | Conv.
         */
        [Display(Name = "Esp.")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ExpectedConversion { get; set; }

        [Display(Name = "Real")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RealConversion { get; set; }

        [Display(Name = "Diferencia")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConversionDifference { get; set; }

        /*
         * Control Semanal | % Mort.
         */
        [Display(Name = "Esp.")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ExpectedMortality { get; set; }

        [Display(Name = "Real")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal RealMortality { get; set; }

        [Display(Name = "Diferencia")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MortalityDifference { get; set; }
    }
}