using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Represents the Brood Report information displayed
     * in the report history list.
     *
     * The ViewModel contains only the properties required
     * by Index and its available actions.
     */
    public class BroodReportListViewModel
    {
        /*
         * Internal identifier required by Details,
         * PDF and future logical deletion actions.
         *
         * It does not need to be displayed in Index.
         */
        public int BroodReportId { get; set; }

        [Display(Name = "Nº")]
        public int ReportNumber { get; set; }

        /*
         * Broiler House information obtained through
         * the Brood relationship.
         */
        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; }
            = string.Empty;

        /*
         * Internal Brood identifier useful for filters
         * and application operations.
         */
        public int BroodId { get; set; }

        [Display(Name = "Camada")]
        public string BroodName { get; set; }
            = string.Empty;

        /*
         * Brood production year.
         *
         * This value is useful because Brood names
         * may be reused in different production years.
         */
        [Display(Name = "Año")]
        public int BroodYear { get; set; }

        /*
         * Date and time when the historical snapshot
         * was generated.
         *
         * This information belongs to the application
         * history and is not printed in the report header.
         */
        [Display(Name = "Fecha de generación")]
        [DisplayFormat(
            DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime BroodReportGeneratedAt { get; set; }

        /*
         * Automatic historical version assigned
         * to the Brood Report.
         */
        [Display(Name = "Versión")]
        public int BroodReportVersion { get; set; }

        /*
         * Logical state of the generated report.
         
        [Display(Name = "Estado")]
        public bool BroodReportState { get; set; }
        */

    }
}