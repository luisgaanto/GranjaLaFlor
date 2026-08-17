using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Represents the complete information required
     * to display a generated Brood Report.
     *
     * Application metadata is combined with the historical
     * snapshot stored in BroodReportData.
     */
    public class BroodReportGetByIdViewModel
    {
        /*
         * Brood Report identification.
         */
        [Display(Name = "ID")]
        public int BroodReportId { get; set; }

        [Display(Name = "Nº")]
        public int ReportNumber { get; set; }

        /*
         * Application History
         *
         * These values describe the generated report inside
         * the application but are not necessarily printed
         * in the original report layout.
         */
        [Display(Name = "Fecha de generación")]
        [DisplayFormat(
            DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime BroodReportGeneratedAt { get; set; }

        [Display(Name = "Versión")]
        public int BroodReportVersion { get; set; }

        /*
         
        [Display(Name = "Estado")]
        public bool BroodReportState { get; set; }
         */

        /*
         * Related Brood information.
         */
        public int BroodId { get; set; }

        [Display(Name = "Camada")]
        public string BroodName { get; set; }
            = string.Empty;

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; }
            = string.Empty;

        [Display(Name = "Fecha Camada")]
        [DataType(DataType.Date)]
        public DateTime BroodDate { get; set; }

        /*
         * Historical Report Snapshot
         *
         * BroodReportService deserializes BroodReportData
         * and assigns the resulting historical information
         * to this property.
         */
        public BroodReportSnapshotViewModel Snapshot { get; set; }
            = new();
    }
}