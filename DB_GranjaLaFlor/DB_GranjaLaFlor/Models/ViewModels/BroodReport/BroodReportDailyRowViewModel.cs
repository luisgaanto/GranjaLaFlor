using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Represents one daily operational row displayed
     * in the Brood Report.
     *
     * The information is obtained mainly from Daily Checks
     * and Income Concentrates and is stored as part of the
     * historical report snapshot.
     */
    public class BroodReportDailyRowViewModel
    {
        /*
         * Sequential day number displayed in the report.
         */
        [Display(Name = "Día")]
        public int DayNumber { get; set; }

        /*
         * Daily Check date.
         */
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /*
         * Control de Aves | Mortalidad
         */
        [Display(Name = "Nat.")]
        public int NaturalMortality { get; set; }

        [Display(Name = "Selec.")]
        public int SelectQuantity { get; set; }

        [Display(Name = "Acum.")]
        public int AccumulatedMortality { get; set; }

        /*
         * Control de Aves | Saldo Aves
         */
        [Display(Name = "Saldo Aves")]
        public int BirdBalance { get; set; }

        /*
         * Control de Alimento | Ingreso
         *
         * Represents the concentrate received and its
         * accumulated value expressed in quintals.
         */
        [Display(Name = "Día")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? IncomeDailyQuintals { get; set; }

        [Display(Name = "Acum.")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? IncomeAccumulatedQuintals { get; set; }

        /*
         * Control de Alimento | Gasto
         *
         * Daily consumption is already available in quintals.
         * Accumulated consumption is converted from kilograms
         * to quintals when the snapshot is generated.
         */
        [Display(Name = "Día")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionDailyQuintals { get; set; }

        [Display(Name = "Acum.")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConsumptionAccumulatedQuintals { get; set; }

        /*
         * Control de Alimento | Saldo
         *
         * Concentrate balance is converted from kilograms
         * to quintals when the snapshot is generated.
         */
        [Display(Name = "Saldo")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ConcentrateBalanceQuintals { get; set; }
    }
}